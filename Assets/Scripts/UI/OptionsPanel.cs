using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

public class OptionsPanel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button saveSettingsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    [SerializeField] private TextMeshProUGUI saveConfirmationText;

    [Header("Settings")]
    [SerializeField] private float defaultMasterVolume = 1.0f;
    [SerializeField] private float defaultMusicVolume = 0.7f;
    [SerializeField] private float defaultSFXVolume = 0.8f;
    [SerializeField] private float saveConfirmationDisplayTime = 1.5f;
    [SerializeField] public bool disableEscKeyHandling = false;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("UI Animation")]
    [SerializeField] private float panelFadeTime = 0.3f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [HideInInspector] public UnityEvent onBackClicked = new UnityEvent();
    [HideInInspector] public UnityEvent onMainMenuClicked = new UnityEvent();

    private CanvasGroup optionsCanvasGroup;
    private bool isPaused = false;

    private void Awake()
    {
        // Get or add canvas group
        optionsCanvasGroup = GetOrAddCanvasGroup(gameObject);
    }

    private void Start()
    {
        // Set up button listeners
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
        if (saveSettingsButton != null)
            saveSettingsButton.onClick.AddListener(OnSaveSettingsClicked);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        
        // Hide save confirmation text initially
        if (saveConfirmationText != null)
            saveConfirmationText.gameObject.SetActive(false);

        // Initialize volume sliders
        SetupVolumeSliders();

        // Load current values from the audio managers
        LoadCurrentAudioValues();
    }

    private void Update()
    {
        // Only check for ESC key if not disabled
        if (!disableEscKeyHandling && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void LoadCurrentAudioValues()
    {
        // Try to get current values from the audio managers
        if (AudioManager.Jukebox.Instance != null && AudioManager.Jukebox.Instance.audioMixer != null)
        {
            float masterVolume = defaultMasterVolume;
            float musicVolume = defaultMusicVolume;

            // Get master volume from the audio mixer
            if (AudioManager.Jukebox.Instance.audioMixer.GetFloat("MasterVolume", out float masterDB))
            {
                // Convert from decibels to linear scale 0-1 range
                masterVolume = masterDB <= -80f ? 0f : Mathf.Pow(10f, masterDB / 20f);
                if (masterVolumeSlider != null)
                {
                    masterVolumeSlider.value = masterVolume;
                    UpdateVolumeText(masterVolumeText, masterVolume);
                }
            }

            if (AudioManager.Jukebox.Instance.audioMixer.GetFloat("MusicVolume", out float musicDB))
            {
                musicVolume = musicDB <= -80f ? 0f : Mathf.Pow(10f, musicDB / 20f);
                if (musicVolumeSlider != null)
                {
                    musicVolumeSlider.value = musicVolume;
                    UpdateVolumeText(musicVolumeText, musicVolume);
                }
            }
        }

        if (SFXManager.Instance != null && SFXManager.Instance.audioMixer != null)
        {
            float sfxVolume = defaultSFXVolume;

            if (SFXManager.Instance.audioMixer.GetFloat("SFXVolume", out float sfxDB))
            {
                sfxVolume = sfxDB <= -80f ? 0f : Mathf.Pow(10f, sfxDB / 20f);
                if (sfxVolumeSlider != null)
                {
                    sfxVolumeSlider.value = sfxVolume;
                    UpdateVolumeText(sfxVolumeText, sfxVolume);
                }
            }
        }
    }

    private void OnSaveSettingsClicked()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClickSound();
        }

        // Save the settings
        SaveSettings();

        // Display save confirmation
        if (saveConfirmationText != null)
        {
            saveConfirmationText.gameObject.SetActive(true);
            StartCoroutine(HideSaveConfirmation());
        }
    }
    

    private void SaveSettings()
    {
        if (AudioManager.Jukebox.Instance != null)
        {
            // Save master volume
            if (masterVolumeSlider != null)
            {
                AudioManager.Jukebox.Instance.SetMasterVolume(masterVolumeSlider.value);
            }

            // Save music volume
            if (musicVolumeSlider != null)
            {
                AudioManager.Jukebox.Instance.SetMusicVolume(musicVolumeSlider.value);
            }
        }

        if (SFXManager.Instance != null)
        {
            // Save SFX volume
            SFXManager.Instance.SetSFXVolume(sfxVolumeSlider.value);
        }

        // Save the settings to PlayerPrefs
        PlayerPrefs.Save();
    }

    private IEnumerator HideSaveConfirmation()
    {
        yield return new WaitForSecondsRealtime(saveConfirmationDisplayTime);
        if (saveConfirmationText != null)
        {
            saveConfirmationText.gameObject.SetActive(false);
        }
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

    private void SetupVolumeSliders()
    {
        // Set up master volume slider
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = defaultMasterVolume;
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            UpdateVolumeText(masterVolumeText, defaultMasterVolume);
        }

        // Set up music volume slider
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = defaultMusicVolume;
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            UpdateVolumeText(musicVolumeText, defaultMusicVolume);
        }

        // Set up SFX volume slider
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = defaultSFXVolume;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            UpdateVolumeText(sfxVolumeText, defaultSFXVolume);
        }
    }

    private void OnMasterVolumeChanged(float value)
    {
        if (AudioManager.Jukebox.Instance != null)
        {
            AudioManager.Jukebox.Instance.SetMasterVolume(value);
            UpdateVolumeText(masterVolumeText, value);
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Jukebox.Instance != null)
        {
            AudioManager.Jukebox.Instance.SetMusicVolume(value);
            UpdateVolumeText(musicVolumeText, value);
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.SetSFXVolume(value);
            UpdateVolumeText(sfxVolumeText, value);
        }
    }

    private void UpdateVolumeText(TextMeshProUGUI text, float value)
    {
        if (text != null)
        {
            string volumeType = "";
            
            if (text == masterVolumeText)
                volumeType = "Master Volume: ";
            else if (text == musicVolumeText)
                volumeType = "Music Volume: ";
            else if (text == sfxVolumeText)
                volumeType = "SFX Volume: ";
                
            text.text = $"{volumeType}{(int)(value * 100)}%";
        }
    }

    private void OnBackClicked()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClickSound();
        }

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            HidePanel();
            onBackClicked.Invoke();
        }
    }

    private void OnMainMenuClicked()
    {
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayButtonClickSound();
        }
        
        // Save settings before returning to menu
        SaveSettings();
        
        // Ensure time scale is reset
        Time.timeScale = 1f;
        
        // Invoke event so external scripts can respond (like Player.cs)
        onMainMenuClicked.Invoke();
        
        // Load the main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        ShowPanel();
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        HidePanel();
    }

    public void ShowPanel()
    {
        Debug.Log($"ShowPanel called on {gameObject.name}, gameObject.activeSelf: {gameObject.activeSelf}");
        
        // Make sure the game object is active
        gameObject.SetActive(true);
        
        // Get or add canvas group if needed
        if (optionsCanvasGroup == null)
        {
            Debug.LogWarning("OptionsPanel: Canvas group is null, creating one");
            optionsCanvasGroup = GetOrAddCanvasGroup(gameObject);
        }
        
        if (optionsCanvasGroup != null)
        {
            StopAllCoroutines();
            
            // Make sure panel is immediately interactive
            optionsCanvasGroup.interactable = true;
            optionsCanvasGroup.blocksRaycasts = true;
            
            // Start at least slightly visible to ensure it's seen
            optionsCanvasGroup.alpha = 0.1f;
            
            // Start fade animation
            StartCoroutine(FadeCanvasGroup(optionsCanvasGroup, 0, 1, panelFadeTime));
            
            Debug.Log($"Started fade in for {gameObject.name}, current alpha: {optionsCanvasGroup.alpha}");
        }
        else
        {
            Debug.LogError($"Failed to get or create canvas group for {gameObject.name}");
        }
    }

    public void HidePanel()
    {
        if (optionsCanvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroupAndDeactivate(optionsCanvasGroup, 1, 0, panelFadeTime));
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