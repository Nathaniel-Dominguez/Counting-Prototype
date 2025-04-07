using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private OptionsPanel optionsPanel;
    [SerializeField] private CreditsPanel creditsPanel;
    [SerializeField] private HowToPlayPanel howToPlayPanel;
    
    [Header("Main Menu Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button howToPlayButton;
    
    [Header("Scene Management")]
    [SerializeField] private string mainGameSceneName = "MainGame";
    [SerializeField] private float transitionTime = 1.0f;
    
    [Header("UI Animation")]
    [SerializeField] private float panelFadeTime = 0.3f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    private CanvasGroup mainMenuCanvasGroup;
    
    private void Awake()
    {
        // Get or add canvas groups to panels
        mainMenuCanvasGroup = GetOrAddCanvasGroup(mainMenuPanel);
        
        // Initialize UI state
        ShowMainMenu();
    }
    
    private void Start()
    {
        // Set up button listeners
        newGameButton.onClick.AddListener(OnNewGameClicked);
        optionsButton.onClick.AddListener(OnOptionsClicked);
        creditsButton.onClick.AddListener(OnCreditsClicked);
        howToPlayButton.onClick.AddListener(OnHowToPlayClicked);
            
        // Subscribe to options panel back button
        if (optionsPanel != null)
            optionsPanel.onBackClicked.AddListener(ShowMainMenu);
            
        // Subscribe to credits panel back button
        if (creditsPanel != null)
            creditsPanel.onBackClicked.AddListener(ShowMainMenu);
            
        // Subscribe to how to play panel back button
        if (howToPlayPanel != null)
            howToPlayPanel.onBackClicked.AddListener(ShowMainMenu);
    }
    
    private CanvasGroup GetOrAddCanvasGroup(GameObject panel)
    {
        if (panel == null) return null;
        
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = panel.AddComponent<CanvasGroup>();
        }
        return canvasGroup;
    }
    
    #region Button Handlers
    
    private void OnNewGameClicked()
    {
        // Play sound
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClickSound();
        }
        
        // Start transition to game scene
        StartCoroutine(LoadGameScene());
    }
    
    private void OnOptionsClicked()
    {
        // Play sound
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClickSound();
        }
        
        if (optionsPanel != null)
        {
            optionsPanel.ShowPanel();
            HidePanel(mainMenuPanel, mainMenuCanvasGroup);
        }
    }
    
    private void OnCreditsClicked()
    {
        // Play sound
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClickSound();
        }
        
        if (creditsPanel != null)
        {
            creditsPanel.ShowPanel();
            HidePanel(mainMenuPanel, mainMenuCanvasGroup);
        }
    }
    
    private void OnHowToPlayClicked()
    {
        // Play sound
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClickSound();
        }
        
        if (howToPlayPanel != null)
        {
            howToPlayPanel.ShowPanel();
            HidePanel(mainMenuPanel, mainMenuCanvasGroup);
        }
    }
    #endregion
    
    #region Panel Management
    
    private void ShowMainMenu()
    {
        ShowPanel(mainMenuPanel, mainMenuCanvasGroup);
        if (optionsPanel != null)
        {
            optionsPanel.HidePanel();
        }
        if (creditsPanel != null)
        {
            creditsPanel.HidePanel();
        }
        if (howToPlayPanel != null)
        {
            howToPlayPanel.HidePanel();
        }
    }
    
    private void ShowPanel(GameObject panel, CanvasGroup canvasGroup)
    {
        if (panel == null || canvasGroup == null) return;
        
        panel.SetActive(true);
        StartCoroutine(FadeCanvasGroup(canvasGroup, 0, 1, panelFadeTime));
    }
    
    private void HidePanel(GameObject panel, CanvasGroup canvasGroup)
    {
        if (panel == null || canvasGroup == null) return;
        
        StartCoroutine(FadeCanvasGroupAndDeactivate(canvasGroup, 1, 0, panelFadeTime, panel));
    }
    
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;
            float evaluatedTime = fadeCurve.Evaluate(normalizedTime);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, evaluatedTime);
            yield return null;
        }
        
        canvasGroup.alpha = endAlpha;
    }
    
    private IEnumerator FadeCanvasGroupAndDeactivate(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration, GameObject panelToDeactivate)
    {
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, startAlpha, endAlpha, duration));
        
        // Deactivate panel when fully faded out
        if (panelToDeactivate != null)
        {
            panelToDeactivate.SetActive(false);
        }
    }
    
    #endregion
    
    #region Scene Management
    
    private IEnumerator LoadGameScene()
    {
        // Fade out the entire screen
        CanvasGroup mainCanvas = GetComponentInParent<CanvasGroup>();
        if (mainCanvas != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(mainCanvas, 1, 0, transitionTime));
        }
        else
        {
            yield return new WaitForSeconds(transitionTime);
        }
        
        // Load the game scene
        SceneManager.LoadScene(mainGameSceneName);
    }
    
    #endregion
} 