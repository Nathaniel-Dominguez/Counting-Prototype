using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CooldownBarUI : MonoBehaviour
{
    [Header("Cooldown Bar Settings")]
    [SerializeField] private Slider cooldownSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient cooldownGradient;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private string readyText = "READY";
    [SerializeField] private string cooldownLabel = "COOLDOWN";

    private CanvasGroup canvasGroup;
    private float currentCooldownValue = 0f;
    private BallLauncher ballLauncher;

    private void Awake()
    {
        // Get the canvas group component or add one if missing
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.Log("Adding CanvasGroup to CooldownBarUI");
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Ensure we have reference to the slider
        if (cooldownSlider == null)
        {
            cooldownSlider = GetComponent<Slider>();
        }

        // Initialize slider to full value (ready state)
        if (cooldownSlider != null)
        {
            cooldownSlider.value = 1;
        }
        
        // Make sure the cooldown bar is visible at start
        canvasGroup.alpha = 1f;
        Debug.Log("CooldownBarUI initialized. Canvas alpha set to 1.");

        // Initialize cooldown text to ready state
        UpdateCooldownText(1);
    }

    private void Start()
    {
        // Get reference to the ball launcher
        ballLauncher = FindFirstObjectByType<BallLauncher>();
        
        if (ballLauncher == null)
        {
            Debug.LogError("CooldownBarUI: Could not find BallLauncher in scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ballLauncher != null)
        {
            // Check cooldown state from BallLauncher
            if (ballLauncher.IsCoolingDown())
            {
                // Calculate cooldown progress based on time (from 0 to 1)
                float cooldownProgress = Mathf.Clamp01(1 - (ballLauncher.GetRemainingCooldownTime() / ballLauncher.GetCooldownTime()));
                
                // Update the cooldown slider
                UpdateCooldown(cooldownProgress);
            }
            else if (cooldownSlider.value < 1)
            {
                // If not cooling down but bar isn't full, set it to full (ready state)
                UpdateCooldown(1);
            }
        }
        
        // Update the fill color based on the cooldown level
        if (cooldownGradient != null && fillImage != null)
        {
            fillImage.color = cooldownGradient.Evaluate(cooldownSlider.value);
        }
    }

    // Method called to update the cooldown bar
    public void UpdateCooldown(float cooldownPercentage)
    {
        if (cooldownSlider != null)
        {
            // Make sure the value is valid
            cooldownPercentage = Mathf.Clamp01(cooldownPercentage);
            
            // Update the stored value and slider
            currentCooldownValue = cooldownPercentage;
            cooldownSlider.value = cooldownPercentage;
            
            // Update the cooldown text
            UpdateCooldownText(cooldownPercentage);
            
            // Force immediate UI update
            Canvas.ForceUpdateCanvases();
        }
    }
    
    // Reset the cooldown to zero (empty) when ball is fired
    public void ResetCooldown()
    {
        UpdateCooldown(0);
    }
    
    // Set cooldown to ready (full)
    public void SetReady()
    {
        UpdateCooldown(1);
    }
    
    // Update the cooldown text display
    private void UpdateCooldownText(float cooldownPercentage)
    {
        if (cooldownText != null)
        {
            // If cooldown is complete (100%), show READY text
            if (Mathf.Approximately(cooldownPercentage, 1f))
            {
                cooldownText.text = readyText;
            }
            else
            {
                // Otherwise show cooldown text with percentage
                cooldownText.text = $"{cooldownLabel}: {cooldownPercentage * 100:F0}%";
            }
            
            // Force text to update immediately
            Canvas.ForceUpdateCanvases();
        }
    }
} 