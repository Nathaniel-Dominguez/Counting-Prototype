using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // The Singleton pattern is a design pattern that restricts a class to having only one instance in memory at any time, while providing a global access point to that instance
    // This is how we set it that instance "public static GameManager Instance { get; private set; }"
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private PowerMeterUI powerMeterUI;
    [SerializeField] private CooldownBarUI cooldownBarUI;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pauseButton; // Reference to the pause button
    [SerializeField] private Player player; // Reference to the player for handling ESC key

    [Header("Game References")]
    [SerializeField] private Transform collectionTray;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera noGlassCamera; 
    [SerializeField] private Camera highCamera; // Higher perspective with glass
    [SerializeField] private Camera highNoGlassCamera; // Higher perspective without glass
    [SerializeField] private float cameraSwitchSpeed = 2.0f;

    [Header("Game Settings")]
    [SerializeField] private float endGameDelay = 2.0f;

    public int currentScore = 0;
    private bool isGameOver = false;
    private Camera activeCamera;
    private int totalBallsEarned = 0; // Track total balls earned during gameplay

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Set up the initial game state
        InitializeGame();

        // Set active camera
        activeCamera = mainCamera;
        if (noGlassCamera != null)
        {
            // Both Cameras exist in the same position, but we only activate one at a time
            noGlassCamera.gameObject.SetActive(false);
            
            // Ensure audio listener is disabled on the inactive camera
            if (noGlassCamera.GetComponent<AudioListener>() != null)
            {
                noGlassCamera.GetComponent<AudioListener>().enabled = false;
            }
        }
        
        // Initialize the high perspective cameras
        if (highCamera != null)
        {
            highCamera.gameObject.SetActive(false);
            if (highCamera.GetComponent<AudioListener>() != null)
            {
                highCamera.GetComponent<AudioListener>().enabled = false;
            }
        }
        
        if (highNoGlassCamera != null)
        {
            highNoGlassCamera.gameObject.SetActive(false);
            if (highNoGlassCamera.GetComponent<AudioListener>() != null)
            {
                highNoGlassCamera.GetComponent<AudioListener>().enabled = false;
            }
        }
    }

    // Private method to initialize the game
    private void InitializeGame()
    {
        currentScore = 0;
        isGameOver = false;

        // Update UI 
        UpdateScoreText();

        // Initialize ball launcher
        if (ballLauncher != null)
        {
            ballLauncher.ResetLauncher(ballLauncher.GetBallsRemaining());
        }
        else
        {
            Debug.LogError("BallLauncher not found in GameManager");
        }

        // Hide game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Check if the game should end periodically
        StartCoroutine(GameOverCheckRoutine());
    }

    // Public method to add points to the current score
    public void AddPoints(int points)
    {
        if (isGameOver) return;

        currentScore += points;
        UpdateScoreText();
    }

    // Add balls to the launcher
    public void AddBalls(int ballsToAdd)
    {
        if (isGameOver || ballLauncher == null) return;
        
        // Add balls to the launcher
        ballLauncher.AddBalls(ballsToAdd);
        
        // Track total balls earned
        totalBallsEarned += ballsToAdd;
        
        // Show bonus ball notification
        BonusBallNotification.ShowNotification(ballsToAdd);
    }

    // Public method to update the power meter
    public void UpdatePowerMeter(float powerPercentage)
    {
        if (powerMeterUI != null)
        {
            powerMeterUI.UpdatePower(powerPercentage);
            
            // Force an immediate canvas update to ensure UI is refreshed
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            Debug.LogError("GameManager: PowerMeterUI reference is null! Make sure it's assigned in the Inspector.");
        }
    }

    // Public method to force refresh the power meter text
    public void ForceRefreshPowerMeter()
    {
        if (powerMeterUI != null)
        {
            powerMeterUI.ForceRefreshText();
        }
    }

    // Public method to hide the power meter
    public void HidePowerMeter()
    {
        if (powerMeterUI != null)
        {
            Debug.Log("GameManager: Hiding PowerMeterUI");
            powerMeterUI.HidePowerMeter();
        }
        else
        {
            Debug.LogError("GameManager: PowerMeterUI reference is null! Make sure it's assigned in the Inspector.");
        }
    }

    // Private method to update the score text (.ToString("N0")) Converts the number to a formatted string
    // The "N0" specifier formats the number with thousand seperators and zero decimal places
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString("N0");
        }
    }

    public Transform GetCollectionTray()
    {
        return collectionTray;
    }

    // Public accessor for game over state
    public bool IsGameOver()
    {
        return isGameOver;
    }

    // Check if the game is over
    public IEnumerator CheckGameOver()
    {
        // Don't check if already game over
        if (isGameOver)
        {
            Debug.Log("CheckGameOver called but game is already over");
            yield return null;
        }
        
        // Ensure we have the BallLauncher reference
        if (ballLauncher == null)
        {
            Debug.LogError("CheckGameOver called but ballLauncher is null");
            yield return null;
        }
        // Delay the game over check to allow for any pending physics calculations to complete
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();

        // Count active balls in the scene
        int ballsInLauncher = ballLauncher.GetBallsRemaining();
        int activeBalls = GameObject.FindGameObjectsWithTag("Ball").Length;
        
        Debug.Log($"CheckGameOver - Balls in launcher: {ballsInLauncher}, Active balls: {activeBalls}");

        // If no balls remain in launcher and no balls are in play, end the game
        if (ballsInLauncher <= 0 && activeBalls <= 0)
        {
            Debug.Log("Game over condition met! Starting EndGameSequence");
            StartCoroutine(EndGameSequence());
        }
        else
        {
            Debug.Log("Game over condition not met - still have balls in play or launcher");
        }
    }

    // Coroutine to check if the game should end periodically
    private IEnumerator GameOverCheckRoutine()
    {
        // Wait for a small delay before starting to check
        yield return new WaitForSeconds(1.0f);

        // Keep checking until game is over
        while (!isGameOver)
        {
            // Wait for all pending physics operations to complete
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();

            // Only check if game is not already over
            if (!isGameOver && ballLauncher != null)
            {
                int ballsInLauncher = ballLauncher.GetBallsRemaining();

                // Use FindGameObjectsWithTag only once and store result
                GameObject[] activeBallObjects = GameObject.FindGameObjectsWithTag("Ball");
                int activeBalls = activeBallObjects.Length;

                Debug.Log($"Periodic check - Balls in Launcher {ballsInLauncher}, Active Balls: {activeBalls}");

                // Check if all balls are gone
                if (ballsInLauncher <= 0 && activeBalls <= 0)
                {
                    Debug.Log("Game over condition met in periodic check!");
                    StartCoroutine(EndGameSequence());
                    yield break; // Exit the coroutine
                }
                
                // Check for stuck balls when no balls left in launcher but active balls exist
                if (ballsInLauncher <= 0 && activeBalls > 0)
                {
                    bool allBallsStuck = CheckForStuckBalls(activeBallObjects);
                    if (allBallsStuck)
                    {
                        Debug.Log("Game over: All remaining balls are stuck!");
                        StartCoroutine(EndGameSequence());
                        yield break; // Exit the coroutine
                    }
                }
            }

            // Wait before checking again
            yield return new WaitForSeconds(2.0f); // Check every 2 seconds
        }
    }

    // Checks if all active balls are stuck (not moving)
    private bool CheckForStuckBalls(GameObject[] activeBalls)
    {
        if (activeBalls.Length == 0) return false;

        int stuckTimeThreshold = 3; // Seconds to consider a ball stuck
        
        bool allBallsStuck = true;
        int stuckBallCount = 0;
        
        Debug.Log($"Checking {activeBalls.Length} balls for stuck status...");
        
        foreach (GameObject ball in activeBalls)
        {
            if (ball == null) 
            {
                Debug.Log("Found null ball reference, skipping");
                continue;
            }
            
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb == null) 
            {
                Debug.Log($"Ball {ball.name} has no Rigidbody, skipping");
                continue;
            }
            
            // Check if ball is moving
            float velocity = rb.linearVelocity.magnitude;
            
            // Check if ball has component to track stuck time, add if not
            StuckBallTracker stuckTracker = ball.GetComponent<StuckBallTracker>();
            if (stuckTracker == null)
            {
                stuckTracker = ball.AddComponent<StuckBallTracker>();
                Debug.Log($"Added StuckBallTracker to {ball.name}");
            }
            
            // The StuckBallTracker component now handles updating stuck time in its Update method
            // We just need to check if it's considered stuck
            if (stuckTracker.IsStuck(stuckTimeThreshold))
            {
                stuckBallCount++;
                Debug.Log($"Ball {ball.name} considered STUCK: {stuckTracker.GetStuckTime():F1} seconds > {stuckTimeThreshold} threshold");
            }
            else
            {
                Debug.Log($"Ball {ball.name} is not stuck (velocity: {velocity:F3}, stuck time: {stuckTracker.GetStuckTime():F1}/{stuckTimeThreshold}s)");
                allBallsStuck = false;
            }
        }
        
        Debug.Log($"Stuck ball check result: {stuckBallCount}/{activeBalls.Length} balls stuck, allBallsStuck={allBallsStuck}");
        return allBallsStuck;
    }
    
    // The StuckBallTracker class has been moved to its own file
    
    private IEnumerator EndGameSequence()
    {
        // Set game over flag immediately to prevent further ball launches
        isGameOver = true;
        Debug.Log("Game over sequence started");
        
        // Hide pause button when game is over
        if (pauseButton != null)
        {
            pauseButton.SetActive(false);
        }
        
        // Disable ESC key pause functionality
        if (player != null)
        {
            player.DisablePauseInput();
        }
        
        // Wait for any final scoring
        yield return new WaitForSeconds(endGameDelay);

        // Make sure to clean up any remaining remaining active balls
        GameObject[] remainingBalls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in remainingBalls)
        {
            if (ball != null && ball.activeInHierarchy)
            {
                // Return any remaining balls to the BallPool
                Debug.Log("Cleaning up remaining ball: " + ball.name);
                ball.SetActive(false);
            }
        }

        // Check for high score "PlayerPrefs" Unity's built-in system for storing and retrieving player preferences between game sessions
        int highScore = PlayerPrefs.GetInt("PachinkoHighScore", 0);
        bool isNewHighScore = currentScore > highScore;

        // Check for balls earned high score
        int ballsEarnedHighScore = PlayerPrefs.GetInt("BallsEarnedHighScore", 0);
        bool isNewBallsHighScore = totalBallsEarned > ballsEarnedHighScore;
       
        if (isNewHighScore)
        {
            Debug.Log("New high score achieved! Old: " + highScore + ", New:" + currentScore);
            highScore = currentScore;
            PlayerPrefs.SetInt("PachinkoHighScore", highScore);
            PlayerPrefs.Save();

            // Play high score sound instead of game over sound
            if (SFXManager.Instance != null)
            {
                Debug.Log("Playing new high score sound");
                SFXManager.Instance.PlayNewHighScoreSound();
            }
        }
        else
        {
            // Play normal game over sound if no high score
            if (SFXManager.Instance != null)
            {
                Debug.Log("Playing normal game over sound");
                SFXManager.Instance.PlaySFX("GameOver", 1f);
            }
        }

        // Save balls earned high score if new record
        if (isNewBallsHighScore)
        {
            Debug.Log("New balls earned high score! Old: " + ballsEarnedHighScore + ", New: " + totalBallsEarned);
            ballsEarnedHighScore = totalBallsEarned;
            PlayerPrefs.SetInt("BallsEarnedHighScore", ballsEarnedHighScore);
            PlayerPrefs.Save();
        }

        // Switch to no Glass Camera for a better view of the final state
        if (noGlassCamera != null && activeCamera != noGlassCamera && activeCamera != highNoGlassCamera)
        {
            // If we're in a glass camera view, switch to the corresponding no-glass camera
            if (activeCamera == mainCamera)
            {
                StartCoroutine(TransitionToCamera(noGlassCamera));
            }
            else if (activeCamera == highCamera)
            {
                StartCoroutine(TransitionToCamera(highNoGlassCamera));
            }
        }

        // Display game over UI
        if (gameOverPanel != null)
        {
            Debug.Log("Activating game over panel");
            
            // Check if the panel has the GameOverPanel component
            GameOverPanel panelComponent = gameOverPanel.GetComponent<GameOverPanel>();
            if (panelComponent != null)
            {
                // Set all the data in the panel before activating it
                panelComponent.SetHighScore(isNewHighScore);
                panelComponent.SetFinalScore(currentScore);
                panelComponent.SetBallsEarned(totalBallsEarned);
                panelComponent.SetBallsEarnedHighScore(ballsEarnedHighScore, isNewBallsHighScore);
                
                // Enable the panel
                gameOverPanel.SetActive(true);
                
                // Let the component handle the fade in
                panelComponent.StartFadeIn();
            }
            else
            {
                Debug.LogError("GameOverPanel component not found on game over panel");
                // Fallback - just enable the panel
                gameOverPanel.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("Game over panel not found in GameManager");
        }
    }

    private IEnumerator TransitionToCamera(Camera targetCamera)
    {
        float elapsed = 0;
        float duration = 1.0f / cameraSwitchSpeed;

        Camera startCamera = activeCamera;

        // Before enabling target camera, disable its audio listener to prevent multiple listeners
        AudioListener targetAudioListener = targetCamera.GetComponent<AudioListener>();
        if (targetAudioListener != null)
        {
            targetAudioListener.enabled = false;
        }
        
        targetCamera.gameObject.SetActive(true);
        targetCamera.depth = startCamera.depth -1; // keep camera behind until transition completes

        // Blend between cameras
        while (elapsed < duration)
        {
            // Increment the elapsed time
            elapsed += Time.deltaTime;
            float transitionTime = Mathf.Clamp01(elapsed / duration); // Clamps the transition time between 0 and 1

            // Crossfade effect by adjusting cameras' clear flags and backgrounds
            // "startColor.a" is the alpha transparency
            Color startColor = startCamera.backgroundColor;
            startColor.a = 1 - transitionTime;
            startCamera.backgroundColor = startColor;

            yield return null;
        }

        // Complete transition
        activeCamera = targetCamera;
        activeCamera.depth = 0;
        
        // Enable audio listener on new active camera and disable on previous camera
        AudioListener activeAudioListener = activeCamera.GetComponent<AudioListener>();
        if (activeAudioListener != null)
        {
            activeAudioListener.enabled = true;
        }
        
        startCamera.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void ToggleCameraView()
    {
        // Cycle through the four camera views
        if (activeCamera == mainCamera)
        {
            // Switch to no glass camera
            if (noGlassCamera != null)
            {
                StartCoroutine(TransitionToCamera(noGlassCamera));
            }
        }
        else if (activeCamera == noGlassCamera)
        {
            // Switch to high camera with glass
            if (highCamera != null)
            {
                StartCoroutine(TransitionToCamera(highCamera));
            }
            else if (mainCamera != null)
            {
                // If high camera isn't available, go back to main
                StartCoroutine(TransitionToCamera(mainCamera));
            }
        }
        else if (activeCamera == highCamera)
        {
            // Switch to high camera without glass
            if (highNoGlassCamera != null)
            {
                StartCoroutine(TransitionToCamera(highNoGlassCamera));
            }
            else if (mainCamera != null)
            {
                // If high no-glass camera isn't available, go back to main
                StartCoroutine(TransitionToCamera(mainCamera));
            }
        }
        else if (activeCamera == highNoGlassCamera)
        {
            // Switch back to main camera
            if (mainCamera != null)
            {
                StartCoroutine(TransitionToCamera(mainCamera));
            }
        }
    }

    // Public method to reset the cooldown bar (called when ball is launched)
    public void ResetCooldownBar()
    {
        if (cooldownBarUI != null)
        {
            cooldownBarUI.ResetCooldown();
        }
        else
        {
            Debug.LogError("GameManager: CooldownBarUI reference is null! Make sure it's assigned in the Inspector.");
        }
    }

    // Public method to set the cooldown bar to ready state
    public void SetCooldownReady()
    {
        if (cooldownBarUI != null)
        {
            cooldownBarUI.SetReady();
        }
        else
        {
            Debug.LogError("GameManager: CooldownBarUI reference is null! Make sure it's assigned in the Inspector.");
        }
    }

    // Public method to update the cooldown bar directly
    public void UpdateCooldownBar(float cooldownPercentage)
    {
        if (cooldownBarUI != null)
        {
            cooldownBarUI.UpdateCooldown(cooldownPercentage);
        }
        else
        {
            Debug.LogError("GameManager: CooldownBarUI reference is null! Make sure it's assigned in the Inspector.");
        }
    }
}