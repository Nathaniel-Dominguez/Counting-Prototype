using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class BonusBallNotification : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI bonusText;
    [SerializeField] private Image backgroundImage;
    
    [Header("Appearance Settings")]
    [SerializeField] private Color textColor = new Color(1f, 0.8f, 0.2f); // Gold color
    [SerializeField] private Color backgroundColor = new Color(0.2f, 0.2f, 0.4f, 0.8f); // Semi-transparent dark blue
    [SerializeField] private float displayDuration = 2.5f;
    
    private static BonusBallNotification instance;
    
    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Initialize UI elements
        if (bonusText != null)
        {
            bonusText.color = textColor;
        }
        
        if (backgroundImage != null)
        {
            backgroundImage.color = backgroundColor;
        }
        
        // Hide notification initially
        gameObject.SetActive(false);
    }
    
    public static void ShowNotification(int ballsAwarded)
    {
        if (instance == null)
        {
            Debug.LogWarning("BonusBallNotification: Instance is null. Cannot show notification.");
            return;
        }
        
        instance.ShowBonusNotification(ballsAwarded);
    }
    
    private void ShowBonusNotification(int ballsAwarded)
    {
        // Make sure the notification is active
        gameObject.SetActive(true);
        
        // Set the text
        if (bonusText != null)
        {
            bonusText.text = $"+{ballsAwarded} BONUS BALL{(ballsAwarded > 1 ? "S" : "")}!";
        }
        
        // Start the auto-hide coroutine
        StopAllCoroutines();
        StartCoroutine(HideAfterDelay());
    }
    
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        
        // Hide the notification
        gameObject.SetActive(false);
    }
}