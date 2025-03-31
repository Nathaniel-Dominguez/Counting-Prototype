using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerMeterUI : MonoBehaviour
{
    [Header("Power Meter Settings")]
    [SerializeField] private Slider powerSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient powerGradient;
    // [SerializeField] private float fadeSpeed = 5.0f;
    [SerializeField] private TextMeshProUGUI powerValueText;

    private CanvasGroup canvasGroup;
    // private float targetAlpha = 1f;
    private float currentPowerValue = 0f;
    
    private void Awake()
    {
        // Get the canvas group component or add one if missing
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.Log("Adding CanvasGroup to PowerMeterUI");
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Ensure we have reference to the slider
        if (powerSlider == null)
        {
            powerSlider = GetComponent<Slider>();
        }

        // Initialize slider to zero
        if (powerSlider != null)
        {
            powerSlider.value = 0;
        }
        
        // Make sure the power meter is visible at start
        canvasGroup.alpha = 1f;
        Debug.Log("PowerMeterUI initialized. Canvas alpha set to 1.");

        // Initialize power text
        UpdatePowerText(0);
    }

    // Update is called once per frame to check if text needs synchronizing with slider
    void Update()
    {
        // Update the fill color based on the power level
        if (powerGradient != null && fillImage != null)
        {
            fillImage.color = powerGradient.Evaluate(powerSlider.value);
        }
        
        // Make sure text always matches the slider value
        if (powerValueText != null && Mathf.Abs(currentPowerValue - powerSlider.value) > 0.001f)
        {
            // Text and slider are out of sync, update text
            currentPowerValue = powerSlider.value;
            UpdatePowerText(currentPowerValue);
        }
    }

    //Method called by GameManager to update the power meter
    public void UpdatePower(float powerPercentage)
    {
        if (powerSlider != null)
        {
            Debug.Log($"PowerMeterUI: Updating power to {powerPercentage}");
            
            // Make sure the value is valid (sometimes during rapid state changes we can get invalid values)
            powerPercentage = Mathf.Clamp01(powerPercentage);
            
            // Update the stored value and slider
            currentPowerValue = powerPercentage;
            powerSlider.value = powerPercentage;
            
            // Update the power text
            UpdatePowerText(powerPercentage);
            
            // Force immediate UI update
            Canvas.ForceUpdateCanvases();
        }
    }

    // Method to hide the power meter - now disabled, meter remains visible
    public void HidePowerMeter()
    {
        // Do nothing - we want the power meter to stay visible
        // But we keep the method to avoid breaking existing code
        // targetAlpha = 0f;
        // We no longer reset the slider value to 0, maintaining its position while fading out
        Debug.Log("PowerMeterUI: HidePowerMeter called but ignored - power meter remains visible");
    }
    
    // Force refresh the power text based on current slider value
    public void ForceRefreshText()
    {
        // Get the current value directly from the slider
        float currentValue = powerSlider != null ? powerSlider.value : 0f;
        
        // Ensure the value is valid
        currentValue = Mathf.Clamp01(currentValue);
        
        Debug.Log($"PowerMeterUI: Force refreshing text with value {currentValue}");
        
        // Update the power text and synchronize the stored value
        currentPowerValue = currentValue;
        UpdatePowerText(currentValue);
        
        // Force immediate UI update
        Canvas.ForceUpdateCanvases();
    }
    
    // Update the power text display
    private void UpdatePowerText(float powerPercentage)
    {
        if (powerValueText != null)
        {
            // Format power as percentage with 0 decimal places
            string newText = $"Power: {powerPercentage * 100:F0}%";
            powerValueText.text = newText;
            Debug.Log($"PowerMeterUI: Updated text to '{newText}'");
            
            // Force text to update immediately
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            Debug.LogWarning("PowerMeterUI: Cannot update power text - TextMeshProUGUI reference is null");
        }
    }
}
