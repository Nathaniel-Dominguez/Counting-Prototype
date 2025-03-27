using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverPanel : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button toggleCameraButton;
    
    [Header("Animation")]
    [SerializeField] private float fadeInSpeed = 1.0f;
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private CanvasGroup canvasGroup;
    private GameManager gameManager;
    
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
    }
    
    public void StartFadeIn()
    {
        // Reset alpha to 0 before starting animation
        canvasGroup.alpha = 0f;
        
        // Start the fade-in coroutine
        StartCoroutine(FadeInCoroutine());
    }
    
    private System.Collections.IEnumerator FadeInCoroutine()
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
} 