using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button toggleCameraButton;
    [SerializeField] private Button mainMenuButton;
    
    [Header("Scene References")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    
    [Header("Text References")]
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI ballsEarnedText;
    [SerializeField] private TextMeshProUGUI ballsEarnedHighScoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Animation")]
    [SerializeField] private float fadeInSpeed = 1.0f;
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float highScorePulseSize = 1.2f;
    
    private CanvasGroup canvasGroup;
    private GameManager gameManager;
    private bool isHighScore = false;
    private bool isNewBallsHighScore = false;
    private int ballsEarned = 0;
    
    private void Awake()
    {
        // Get the canvas group component or add one if missing
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.Log("Adding CanvasGroup to GameOverPanel");
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Initialize panel to be invisible
        canvasGroup.alpha = 0f;
    }
    
    private void Start()
    {
        // Get reference to GameManager
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogError("GameOverPanel: GameManager instance not found!");
        }
        
        // Setup the button listeners
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
        }
        else
        {
            Debug.LogError("GameOverPanel: Restart button reference not set!");
        }
        
        if (toggleCameraButton != null)
        {
            toggleCameraButton.onClick.AddListener(OnToggleCameraButtonClicked);
        }
        else
        {
            Debug.LogError("GameOverPanel: Toggle camera button reference not set!");
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }
        else
        {
            Debug.LogWarning("GameOverPanel: Main Menu button reference not set!");
        }
        
        // Initially hide the panel
        gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        // Start fade-in animation when panel is enabled
        StartFadeIn();
        
        // Play a sound effect if available
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySFX("ButtonClick", 0.7f);
        }

        // Start high score effects if it's a high score
        if (isHighScore && highScoreText != null)
        {
            // Set the initial scale to normal before starting the animation
            highScoreText.transform.localScale = Vector3.one;
            StartCoroutine(HighScoreCelebrationEffect());
        }
        
        // Start balls high score effect if it's a new balls high score
        if (isNewBallsHighScore && ballsEarnedHighScoreText != null)
        {
            ballsEarnedHighScoreText.transform.localScale = Vector3.one;
            StartCoroutine(BallsHighScoreCelebrationEffect());
        }
    }

    public void StartFadeIn()
    {
        // Reset alpha to 0 before starting animation
        canvasGroup.alpha = 0f;
        
        // Start the fade-in coroutine
        StartCoroutine(FadeInCoroutine());
    }
    
    private IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0f;
        float duration = 1.0f / fadeInSpeed;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;
            
            // Use animation curve for smoother effect
            float alpha = fadeInCurve.Evaluate(normalizedTime);
            canvasGroup.alpha = alpha;
            
            yield return null;
        }
        
        // Ensure we end at full opacity
        canvasGroup.alpha = 1f;
    }
    
    // Public method to set high score status, call will be made from GameManager
    public void SetHighScore(bool isNewHighScore)
    {
        isHighScore = isNewHighScore;
        Debug.Log("High score flag set to: " + isHighScore);
        
        // Update high score text display format based on whether it's a new high score
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("PachinkoHighScore", 0);
            string highScoreLabel = isNewHighScore ? "NEW HIGH SCORE: " : "High Score: ";
            highScoreText.text = highScoreLabel + highScore.ToString("N0");
            
            // Change the color of the high score text to make it more visible
            if (isNewHighScore)
            {
                highScoreText.color = new Color(1f, 0.8f, 0f); // Golden color
                highScoreText.fontStyle = FontStyles.Bold;
            }
            else
            {
                highScoreText.color = Color.white;
                highScoreText.fontStyle = FontStyles.Normal;
            }
        }
    }

    // Public method to set balls earned, call will be made from GameManager
    public void SetBallsEarned(int amount)
    {
        ballsEarned = amount;
        
        if (ballsEarnedText != null)
        {
            ballsEarnedText.text = $"Balls Earned: {ballsEarned}";
            Debug.Log($"Set balls earned to: {ballsEarned}");
        }
        else
        {
            Debug.LogWarning("GameOverPanel: ballsEarnedText reference not set!");
        }
    }

    // Public method to set final score, call will be made from GameManager
    public void SetFinalScore(int score)
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + score.ToString("N0");
            Debug.Log($"Set final score to: {score}");
        }
        else
        {
            Debug.LogWarning("GameOverPanel: finalScoreText reference not set!");
        }
    }

    // Public method to set balls earned high score
    public void SetBallsEarnedHighScore(int highScore, bool isNewRecord)
    {
        isNewBallsHighScore = isNewRecord;
        
        if (ballsEarnedHighScoreText != null)
        {
            string label = isNewRecord ? "NEW BALLS HIGH SCORE: " : "Balls Earned High Score: ";
            ballsEarnedHighScoreText.text = label + highScore.ToString();
            
            // Apply styling similar to score high score
            if (isNewRecord)
            {
                ballsEarnedHighScoreText.color = new Color(1f, 0.8f, 0f); // Golden color
                ballsEarnedHighScoreText.fontStyle = FontStyles.Bold;
                
                // Set initial scale to normal before starting the animation
                ballsEarnedHighScoreText.transform.localScale = Vector3.one;
                
                // Start the celebration effect if panel is already enabled
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(BallsHighScoreCelebrationEffect());
                }
            }
            else
            {
                ballsEarnedHighScoreText.color = Color.white;
                ballsEarnedHighScoreText.fontStyle = FontStyles.Normal;
            }
        }
        else
        {
            Debug.LogWarning("GameOverPanel: ballsEarnedHighScoreText reference not set!");
        }
    }

    private IEnumerator HighScoreCelebrationEffect()
    {
        // Pulse the high score text size a few times
        if (highScoreText != null)
        {
            Debug.Log("Starting high score celebration effect");

            // Wait a short time before starting the animation
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < 3; i++)
            {
                // Scale up
                float time = 0;
                Vector3 originalScale = Vector3.one;
                Vector3 targetScale = originalScale * highScorePulseSize;

                while (time < 0.5f)
                {
                    time += Time.deltaTime;
                    float t = time / 0.5f;
                    highScoreText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                    yield return null;
                }

                // Scale back down
                time = 0;
                while (time < 0.5f)
                {
                    time += Time.deltaTime;
                    float t = time / 0.5f;
                    highScoreText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                    yield return null;
                }

                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private IEnumerator BallsHighScoreCelebrationEffect()
    {
        // Pulse the balls high score text size a few times
        if (ballsEarnedHighScoreText != null)
        {
            Debug.Log("Starting balls high score celebration effect");

            // Wait a short time before starting the animation
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < 3; i++)
            {
                // Scale up
                float time = 0;
                Vector3 originalScale = Vector3.one;
                Vector3 targetScale = originalScale * highScorePulseSize;

                while (time < 0.5f)
                {
                    time += Time.deltaTime;
                    float t = time / 0.5f;
                    ballsEarnedHighScoreText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                    yield return null;
                }

                // Scale back down
                time = 0;
                while (time < 0.5f)
                {
                    time += Time.deltaTime;
                    float t = time / 0.5f;
                    ballsEarnedHighScoreText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                    yield return null;
                }

                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private void OnRestartButtonClicked()
    {
        // Play button click sound
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClickSound();
        }
        
        // Call the GameManager's restart method
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        else
        {
            // Fallback if GameManager reference is missing
            UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    private void OnToggleCameraButtonClicked()
    {
        // Play button click sound
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClickSound();
        }
        
        // Call the GameManager's toggle camera method
        if (gameManager != null)
        {
            gameManager.ToggleCameraView();
        }
    }

    private void OnMainMenuButtonClicked()
    {
        // Play button click sound
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClickSound();
        }
        
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }
} 