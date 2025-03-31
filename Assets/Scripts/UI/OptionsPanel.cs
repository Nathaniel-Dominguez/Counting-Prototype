using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class OptionsPanel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button backButton;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;

    [Header("Settings")]
    [SerializeField] private float defaultMasterVolume = 1.0f;
    [SerializeField] private float defaultMusicVolume = 0.7f;
    [SerializeField] private float defaultSFXVolume = 0.8f;

    [Header("UI Animation")]
    [SerializeField] private float panelFadeTime = 0.3f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [HideInInspector] public UnityEvent onBackClicked = new UnityEvent();

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

        // Initialize volume sliders
        SetupVolumeSliders();
    }

    private void Update()
    {
        // Check for ESC key to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
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
            text.text = $"{(int)(value * 100)}%";
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
        gameObject.SetActive(true);
        if (optionsCanvasGroup != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeCanvasGroup(optionsCanvasGroup, 0, 1, panelFadeTime));
            optionsCanvasGroup.interactable = true;
            optionsCanvasGroup.blocksRaycasts = true;
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