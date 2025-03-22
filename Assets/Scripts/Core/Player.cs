using UnityEngine;

public class Player : MonoBehaviour 
{
    [Header("Input Settings")]
    [SerializeField] private KeyCode launchKey = KeyCode.Space;
    [SerializeField] private KeyCode cameraToggleKey = KeyCode.V;
    [SerializeField] private bool useKeyboardControls = true;
    [SerializeField] private bool useMouseControls = true;

    [Header("References")]
    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private GameManager gameManager;

    private bool isChargingLaunch = false;
    private SFXManager sfxManager;

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
                Debug.LogError("Player: GamerManager reference not set and oculdn't be found in the scene!");
            }
        }
    }

    private void Update()
    {
        // handle camera toggle input
        if (Input.GetKeyDown(cameraToggleKey))
        {
            ToggleCameraView();
        }

        // Handle ball launcher input
        HandleBallLauncherInput();
    }

    private void HandleBallLauncherInput()
    {
        // Only process input if we have a valid ball Launcher
        if (ballLauncher == null) return;

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
        isChargingLaunch = true;

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
        isChargingLaunch = false;

        // Tell ballLauncher to release the ball
        ballLauncher.LaunchBall();
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
}