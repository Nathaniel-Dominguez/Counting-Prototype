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
    [SerializeField] private Slider powerMeterSlider;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    [Header("Game References")]
    [SerializeField] private Transform collectionTray;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera noGlassCamera; 
    [SerializeField] private float cameraSwitchSpeed = 2.0f;

    [Header("Game Settings")]
    [SerializeField] private int startingBalls = 10;
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
        } 
    }
    // Private method to initialize the game
    private void InitializeGame()
    {
        currentScore = 0;
        isGameOver = false;

        // Update UI 
        UpdateScoreText();
        UpdateBallsRemainingText(startingBalls);

        // Hide game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Reset power meter
        if (powerMeterSlider != null)
        {
            powerMeterSlider.value = 0;
            powerMeterSlider.gameObject.SetActive(false);
        }
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
        if (powerMeterSlider != null)
        {
            powerMeterSlider.gameObject.SetActive(true);
            powerMeterSlider.value = powerPercentage;
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

    // Check if the game is over
    public void CheckGameOver()
    {
        // Count active balls in the scene, now properly accessing ballsRemaining from the BallLauncher script
        int ballsInLauncher = ballLauncher.GetBallsRemaining();
        int activeBalls = GameObject.FindGameObjectsWithTag("Ball").Length;

        // If no balls remain in launcher and no balls are in play, end the game
        if (ballsInLauncher <= 0 && activeBalls <= 0)
        {
            StartCoroutine(EndGameSequence());
        }        
    }

    private IEnumerator EndGameSequence()
    {
        // Wait for any final scoring
        yield return new WaitForSeconds(endGameDelay);

        isGameOver = true;

        // Switch to no Glass Camera for a better view of the final state
        if (noGlassCamera != null && activeCamera != noGlassCamera)
        {
            StartCoroutine(TransitionToCamera(noGlassCamera));
        }

        // Check for high score "PlayerPrefs" is Unity's built-in system for storing and retrieving player preferences between game sessions
        int highScore = PlayerPrefs.GetInt("PachinkoHighScore", 0);
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("PachinkoHighScore", highScore);
            PlayerPrefs.Save();
        }

        // Display game over UI
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + currentScore.ToString("N0");
            }

            if (highScoreText != null)
            {
                highScoreText.text ="High Score: " + highScore.ToString("N0");
            }
        }
    }

    private IEnumerator TransitionToCamera(Camera targetCamera)
    {
        float elapsed = 0;
        float duration = 1.0f / cameraSwitchSpeed;

        Camera startCamera = activeCamera;

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