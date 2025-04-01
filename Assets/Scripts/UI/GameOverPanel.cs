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

    [Header("Animation")]
    [SerializeField] private float fadeInSpeed = 1.0f;
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float highScorePulseSize = 1.2f;
    
    private CanvasGroup canvasGroup;
    private GameManager gameManager;
    private bool isHighScore = false;
    
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