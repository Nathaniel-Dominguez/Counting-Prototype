using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class HowToPlayPanel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI instructionsText;
    
    [Header("Instructions Content")]
    [TextArea(5, 10)]
    [SerializeField] private string gameInstructions = "How to play:\n\n1. Fire Pachinko Balls into the Pachinko Machine Playfield\n\n2. Score points earn balls by landing shots in the score pockets \n\n3. The Jackpot is the highest score pocket, and scales based on your current score!\n\n4. The game ends when you run out of balls\n\n Good Luck!";
    
    
    
    [Header("UI Animation")]
    [SerializeField] private float panelFadeTime = 0.3f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [HideInInspector] public UnityEvent onBackClicked = new UnityEvent();
    
    private CanvasGroup howToPlayCanvasGroup;
    
    private void Awake()
    {
        // Get or add canvas group
        howToPlayCanvasGroup = GetOrAddCanvasGroup(gameObject);
        
        // Always update the instructions text on awake
        UpdateInstructionsText();
    }
    
    private void Start()
    {
        // Set up button listeners
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
    }
    
    public void UpdateInstructionsText()
    {
        if (instructionsText != null)
        {
            instructionsText.text = gameInstructions;
            Debug.Log("HowToPlayPanel: Updated instructions text to: " + gameInstructions);
        }
        else
        {
            Debug.LogWarning("HowToPlayPanel: instructionsText component is not assigned!");
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
        if (howToPlayCanvasGroup == null)
        {
            howToPlayCanvasGroup = GetOrAddCanvasGroup(gameObject);
        }
        
        // Always update the text when showing the panel
        UpdateInstructionsText();
        
        if (howToPlayCanvasGroup != null)
        {
            StopAllCoroutines();
            
            // Make sure panel is immediately interactive
            howToPlayCanvasGroup.interactable = true;
            howToPlayCanvasGroup.blocksRaycasts = true;
            
            // Start at least slightly visible to ensure it's seen
            howToPlayCanvasGroup.alpha = 0.1f;
            
            // Start fade animation
            StartCoroutine(FadeCanvasGroup(howToPlayCanvasGroup, 0, 1, panelFadeTime));
        }
    }
    
    public void HidePanel()
    {
        if (howToPlayCanvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroupAndDeactivate(howToPlayCanvasGroup, 1, 0, panelFadeTime));
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