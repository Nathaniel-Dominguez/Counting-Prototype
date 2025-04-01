using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OptionsButton : MonoBehaviour
{
    [SerializeField] private Player player;
    
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    
    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(ShowOptions);
        }
        
        // Try to find player if not assigned
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
        }
    }
    
    private void ShowOptions()
    {
        // Play button sound
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClickSound();
        }
        
        // Show options and pause game
        if (player != null)
        {
            player.TogglePause();
        }
        else
        {
            Debug.LogWarning("OptionsButton: No Player reference found. Cannot toggle pause.");
        }
    }
    
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(ShowOptions);
        }
    }
} 