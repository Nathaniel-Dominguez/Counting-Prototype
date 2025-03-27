using UnityEngine;
using UnityEngine.UI;

public class PowerMeterUI : MonoBehaviour
{
    [Header("Power Meter Settings")]
    [SerializeField] private Slider powerSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient powerGradient;
    [SerializeField] private float fadeSpeed = 5.0f;

    private CanvasGroup canvasGroup;
    private float targetAlpha = 0f;
    
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
        
        // Hide at start
        canvasGroup.alpha = 0f;
        Debug.Log("PowerMeterUI initialized. Canvas alpha set to 0.");
    }

    void Update()
    {
        // Smoothly fade the meter in/out
        float previousAlpha = canvasGroup.alpha;
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);

        // Log significant alpha changes for debugging
        if (Mathf.Abs(previousAlpha - canvasGroup.alpha) > 0.05f)
        {
            Debug.Log($"PowerMeterUI: Alpha changing from {previousAlpha:F2} to {canvasGroup.alpha:F2}, target={targetAlpha}");
        }

        // Update the fill color based on the power level
        if (powerGradient != null && fillImage != null)
        {
            fillImage.color = powerGradient.Evaluate(powerSlider.value);
        }
    }

    //Method called by GameManager to update the power meter
    public void UpdatePower(float powerPercentage)
    {
        if (powerSlider != null)
        {
            Debug.Log($"PowerMeterUI: Updating power to {powerPercentage}");
            powerSlider.value = powerPercentage;

            // Show the meter when it's being used
            targetAlpha = 1f;
        }
    }

    // Method to hide the power meter - this will fade out over time
    public void HidePowerMeter()
    {
        Debug.Log("PowerMeterUI: Hiding meter, setting targetAlpha to 0");
        targetAlpha = 0f;
        // We no longer reset the slider value to 0, maintaining its position while fading out
    }   
}
