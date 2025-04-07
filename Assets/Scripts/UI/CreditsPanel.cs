using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class CreditsPanel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI creditsText;
    
    [Header("Credits Content")]
    [SerializeField] private string voiceActorName = "Voice Actor Name";
    [SerializeField] private string assetPackName = "Asset Pack Name";
    [SerializeField] private string additionalCredits = "";
    
    [Header("UI Animation")]
    [SerializeField] private float panelFadeTime = 0.3f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [HideInInspector] public UnityEvent onBackClicked = new UnityEvent();
    
    private CanvasGroup creditsCanvasGroup;
    
    private void Awake()
    {
        // Get or add canvas group
        creditsCanvasGroup = GetOrAddCanvasGroup(gameObject);
        
        // Set default credits text if not already set
        if (creditsText != null && string.IsNullOrEmpty(creditsText.text))
        {
            UpdateCreditsText();
        }
    }
    
    private void Start()
    {
        // Set up button listeners
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
    }
    
    public void UpdateCreditsText()
    {
        if (creditsText != null)
        {
            string text = "Voice Acting: " + voiceActorName + "\n\n" + "Asset Pack: " + assetPackName;
            
            if (!string.IsNullOrEmpty(additionalCredits))
            {
                text += "\n\n" + additionalCredits;
            }
            
            text += "\n\nThank you for your contribution!";
            
            creditsText.text = text;
        }
    }
    
    private void OnBackClicked()
    {
        // Play sound
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClickSound();
        }
        
        HidePanel();
        onBackClicked.Invoke();
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
    
    public void ShowPanel()
    {
        // Make sure the game object is active
        gameObject.SetActive(true);
        
        // Get or add canvas group if needed
        if (creditsCanvasGroup == null)
        {
            creditsCanvasGroup = GetOrAddCanvasGroup(gameObject);
        }
        
        if (creditsCanvasGroup != null)
        {
            StopAllCoroutines();
            
            // Make sure panel is immediately interactive
            creditsCanvasGroup.interactable = true;
            creditsCanvasGroup.blocksRaycasts = true;
            
            // Start at least slightly visible to ensure it's seen
            creditsCanvasGroup.alpha = 0.1f;
            
            // Start fade animation
            StartCoroutine(FadeCanvasGroup(creditsCanvasGroup, 0, 1, panelFadeTime));
        }
    }
    
    public void HidePanel()
    {
        if (creditsCanvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroupAndDeactivate(creditsCanvasGroup, 1, 0, panelFadeTime));
        }
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
    
    private IEnumerator FadeCanvasGroupAndDeactivate(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        // Ensure interactivity is disabled right away
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, startAlpha, endAlpha, duration));
        
        // Deactivate panel when fully faded out
        gameObject.SetActive(false);
    }
} 