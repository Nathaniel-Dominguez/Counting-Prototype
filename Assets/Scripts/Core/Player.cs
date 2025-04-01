using UnityEngine;
using System;
using System.Collections;

public class Player : MonoBehaviour 
{
    // Ball launched event delegate
    public delegate void BallLaunchedHandler(float powerPercentage);
    public event BallLaunchedHandler OnBallLaunched;

    [Header("Input Settings")]
    [SerializeField] private KeyCode launchKey = KeyCode.Space;
    [SerializeField] private KeyCode cameraToggleKey = KeyCode.V;
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
    [SerializeField] private bool useKeyboardControls = true;
    [SerializeField] private bool useMouseControls = true;
    [SerializeField] private float inputDebounceTime = 0.1f; // Debounce time between input actions

    [Header("References")]
    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private OptionsPanel optionsPanel;
    [SerializeField] private GameObject gameplayElements; // Optional: parent object containing gameplay elements to disable during pause

    private bool isChargingLaunch = false;
    private SFXManager sfxManager;
    private bool inputLocked = false; // Prevents rapid input
    private bool isPaused = false;

    private void Start()
    {
        // Validate references
        sfxManager = SFXManager.Instance;
        // Validate the SFX Manager
        if (sfxManager == null)
        {
            Debug.LogWarning("Player: SFXManager.Instance is null. SFX features will be disabled.");
        }
        if (ballLauncher == null)
        {
            ballLauncher = FindFirstObjectByType<BallLauncher>();
            if (ballLauncher == null)
            {
                Debug.LogError("Player: BallLauncher reference not set and couldn't be found in the scene!");
            }
        }

        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
            if (gameManager == null)
            {
                Debug.LogError("Player: GamerManager reference not set and couldn't be found in the scene!");
            }
        }
        
        // Setup options panel if it exists
        if (optionsPanel != null)
        {
            // Make sure it starts hidden
            optionsPanel.gameObject.SetActive(false);
            
            // Set the ESC key handling to disabled since we'll handle it here
            optionsPanel.disableEscKeyHandling = true;
            
            // Subscribe to events
            optionsPanel.onBackClicked.AddListener(ResumeGame);
            optionsPanel.onMainMenuClicked.AddListener(OnReturnToMainMenu);
        }
    }

    private void Update()
    {
        // Check for pause input
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
            return; // Skip other inputs when pausing/unpausing
        }
        
        // Only process other inputs if game is not paused
        if (!isPaused)
        {
            // handle camera toggle input
            if (Input.GetKeyDown(cameraToggleKey))
            {
                ToggleCameraView();
            }

            // Handle ball launcher input
            HandleBallLauncherInput();
        }
    }
    
    private void OnDestroy()
    {
        // Ensure we clean up our event subscriptions
        if (optionsPanel != null)
        {
            optionsPanel.onBackClicked.RemoveListener(ResumeGame);
            optionsPanel.onMainMenuClicked.RemoveListener(OnReturnToMainMenu);
        }
        
        // Make sure we reset time scale when destroyed
        Time.timeScale = 1f;
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    
    private void PauseGame()
    {
        // Pause time
        Time.timeScale = 0f;
        
        // Optionally disable gameplay elements
        if (gameplayElements != null)
        {
            gameplayElements.SetActive(false);
        }
        
        // Show options panel
        if (optionsPanel != null)
        {
            Debug.Log("Showing options panel");
            optionsPanel.ShowPanel();
        }
        else
        {
            Debug.LogError("Options panel reference is missing on Player script!");
        }
    }
    
    public void ResumeGame()
    {
        // Resume time
        Time.timeScale = 1f;
        isPaused = false;
        
        // Re-enable gameplay elements
        if (gameplayElements != null)
        {
            gameplayElements.SetActive(true);
        }
        
        // Hide options panel
        if (optionsPanel != null)
        {
            optionsPanel.HidePanel();
        }
    }

    private void HandleBallLauncherInput()
    {
        // Only process input if we have a valid ball Launcher and input is not locked
        if (ballLauncher == null || inputLocked) return;

        // Keyboard input
        if (useKeyboardControls)
        {
            // Start charging when launch key is pressed
            if (Input.GetKeyDown(launchKey) && !isChargingLaunch)
            {
                StartCharging();
            }

            // Release and launch when key is released
            if (Input.GetKeyUp(launchKey) && isChargingLaunch)
            {
                LaunchBall();
            }
        }

        // Mouse input
        if (useMouseControls)
        {
            // Start charging when left mouse button is pressed
            if (Input.GetMouseButtonDown(0) && !isChargingLaunch)
            {
                StartCharging();
            }

            // Release and launch when mouse button is released
            if (Input.GetMouseButtonUp(0) && isChargingLaunch)
            {
                LaunchBall();
            }
        }
    }

    private void StartCharging()
    {
        // Validate the ball launcher is ready
        if (ballLauncher == null)
        {
            Debug.LogWarning("Player: Cannot start charging - launcher is null");
            return;
        }
        
        // Check for cooldown separately to provide feedback
        if (ballLauncher.IsCoolingDown())
        {
            Debug.LogWarning("Player: Cannot start charging - launcher is cooling down");
            
            // Visual feedback for cooldown state
            if (SFXManager.Instance != null)
            {
                // Play a different sound for cooldown feedback
                SFXManager.Instance.PlayButtonClickSound();
            }
            
            // Force refresh power meter to show current state
            GameManager.Instance.ForceRefreshPowerMeter();
            
            return;
        }
        
        // Lock input to prevent rapid input issues
        LockInput();
        
        // Set charging state BEFORE calling StartCharging on the launcher
        isChargingLaunch = true;
        
        // Log the state change
        Debug.Log("Player: Starting charging sequence");

        // Tell BallLauncher to start charging
        ballLauncher.StartCharging();

        // Play button click sound for feedback
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClickSound();
        }
    }

    private void LaunchBall()
    {
        // Make sure we have a valid ball launcher
        if (ballLauncher == null)
        {
            Debug.LogWarning("Player: Cannot launch ball - launcher is null");
            isChargingLaunch = false;
            return;
        }
        
        // Lock input to prevent rapid input issues
        LockInput();
        
        // Calculate the power level (0-1) from the ball launcher
        float powerPercentage = (ballLauncher.GetCurrentLaunchForce() - ballLauncher.GetMinLaunchForce()) / (ballLauncher.GetMaxLaunchForce() - ballLauncher.GetMinLaunchForce());
        
        // Log the launch
        Debug.Log($"Player: Launching ball at power: {powerPercentage:F2}");
        
        // Set charging state to false BEFORE calling LaunchBall
        isChargingLaunch = false;

        // Tell ballLauncher to release the ball
        ballLauncher.LaunchBall();

        // Trigger the ball launched event
        OnBallLaunched?.Invoke(powerPercentage);
    }

    // Locks input for a short time to prevent rapid clicking issues
    private void LockInput()
    {
        inputLocked = true;
        StartCoroutine(UnlockInputAfterDelay());
    }

    // Coroutine to unlock input after a short delay
    private IEnumerator UnlockInputAfterDelay()
    {
        yield return new WaitForSeconds(inputDebounceTime);
        inputLocked = false;
    }

    private void ToggleCameraView()
    {
        // Tell GameManager to toggle camera view
        if (gameManager != null)
        {
            gameManager.ToggleCameraView();
        }
    }

    // Public method to enable/disable input types
    public void SetInputMode(bool useKeyboard, bool useMouse)
    {
        useKeyboardControls = useKeyboard;
        useMouseControls = useMouse;
    }

    private void OnReturnToMainMenu()
    {
        // Clean up any resources or state before returning to main menu
        isPaused = false;
        Time.timeScale = 1f;
        
        // Re-enable gameplay elements if they were disabled
        if (gameplayElements != null)
        {
            gameplayElements.SetActive(true);
        }
        
        Debug.Log("Returning to main menu from Player script");
    }
}