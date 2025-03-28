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
    [SerializeField] private TextMeshProUGUI ballsRemainingText;
    [SerializeField] private PowerMeterUI powerMeterUI;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Game References")]
    [SerializeField] private Transform collectionTray;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera noGlassCamera; 
    [SerializeField] private float cameraSwitchSpeed = 2.0f;

    [Header("Game Settings")]
    [SerializeField] private int startingBalls = 200;
    [SerializeField] private float endGameDelay = 2.0f;

    private int currentScore = 0;
    private bool isGameOver = false;
    private Camera activeCamera;

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
    }
    // Private method to initialize the game
    private void InitializeGame()
    {
        currentScore = 0;
        isGameOver = false;

        // Update UI 
        UpdateScoreText();

        // Initialize ball launcher with the starting balls count
        if (ballLauncher != null)
        {
            ballLauncher.ResetLauncher(startingBalls);
        }
        else
        {
            Debug.LogError("BallLauncher not found in GameManager");
            // fallback to default starting balls
            UpdateBallsRemainingText(startingBalls);
        }

        // Hide game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Reset power meter
        if (powerMeterUI != null)
        {
            powerMeterUI.HidePowerMeter();
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

    // Public method to update the balls remaining text
    public void UpdateBallsRemainingText(int ballsRemaining)
    {
        if (ballsRemainingText != null)
        {
            ballsRemainingText.text = "Balls: " + ballsRemaining.ToString();
        }
    }

    // Public method to update the power meter
    public void UpdatePowerMeter(float powerPercentage)
    {
        if (powerMeterUI != null)
        {
            Debug.Log($"GameManager: Updating PowerMeterUI to {powerPercentage}");
            powerMeterUI.UpdatePower(powerPercentage);
        }
        else
        {
            Debug.LogError("GameManager: PowerMeterUI reference is null! Make sure it's assigned in the Inspector.");
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

        // Update UI to always show correct ball count
        UpdateBallsRemainingText(ballsInLauncher);

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

                // Update UI
                UpdateBallsRemainingText(ballsInLauncher);

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

        // Switch to no Glass Camera for a better view of the final state
        if (noGlassCamera != null && activeCamera != noGlassCamera)
        {
            StartCoroutine(TransitionToCamera(noGlassCamera));
        }

        // Display game over UI
        if (gameOverPanel != null)
        {
            Debug.Log("Activating game over panel");
            // Set the text values first before activating the panel
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + currentScore.ToString("N0");
            }

            if (highScoreText != null)
            {
                highScoreText.text = "High Score: " + highScore.ToString("N0");
                // Add an indicator if it's a new high score
                string highScoreLabel = isNewHighScore ? "NEW HIGH SCORE: " : "High Score: ";
                highScoreText.text = highScoreLabel + highScore.ToString("N0");

                // Change the color of the high score text to make it more visible
                if (isNewHighScore && highScoreText.TryGetComponent<TMPro.TextMeshProUGUI>(out var tmpText))
                {
                    tmpText.color = new Color(1f, 0.8f, 0f); // Golden color
                    tmpText.fontStyle = TMPro.FontStyles.Bold;
                }
            }
            
            // Enable the panel - GameOverPanel component will handle the fade-in animation
            gameOverPanel.SetActive(true);
            
            // Check if the panel has the GameOverPanel component
            GameOverPanel panelComponent = gameOverPanel.GetComponent<GameOverPanel>();
            if (panelComponent != null)
            {

                // Tell the panel if this is a new high score
                panelComponent.SetHighScore(isNewHighScore);
                // Let the component handle the fade in
                panelComponent.StartFadeIn();
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
        // Toggle between main (with glass) and no-glass cameras
        if (activeCamera == mainCamera && noGlassCamera != null)
        {
            StartCoroutine(TransitionToCamera(noGlassCamera));
        }
        else if (activeCamera == noGlassCamera && mainCamera != null)
        {
            StartCoroutine(TransitionToCamera(mainCamera));
        }
    }
}