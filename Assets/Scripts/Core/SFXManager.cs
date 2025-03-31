using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Handles all sound effect functionality for the game.
/// Manages sound effects for gameplay elements like bumpers, spinners, and UI elements.
/// Uses object pooling for efficient audio source management.
/// </summary>
public class SFXManager : MonoBehaviour
{
    // Singleton instance
    public static SFXManager Instance { get; private set; }

    #region INSPECTOR FIELDS

    [Header("Audio Mixing")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterVolumeParam = "MasterVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";

    [Header("Sound Effects")]
    [SerializeField] private AudioClip ballLaunchSound;
    [SerializeField] private AudioClip pinHitSound;
    [SerializeField] private AudioClip bumperHitSound;
    [SerializeField] private AudioClip spinnerSound;
    [SerializeField] private AudioClip lowScoreSound;
    [SerializeField] private AudioClip mediumScoreSound;
    [SerializeField] private AudioClip highScoreSound;
    [SerializeField] private AudioClip jackpotScoreSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip drainSound;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip newHighScoreSound;
    [SerializeField] private AudioClip WoodenImpactSound;

    [Header("Sound Effect Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float defaultSFXVolume = 0.7f;
    [SerializeField] private int sfxSourcePoolSize = 10;
    [SerializeField] private Transform sfxSourceParent;
    [SerializeField] private int maxSimulataneousSounds = 20;
    [SerializeField] private float spinnerSoundCooldown = 0.5f; // Cooldown time for spinner sound

    [Header("Randomization Settings")]
    [Range(0f, 0.7f)]
    [SerializeField] private float volumeRandomization = 0.05f;
    [Range(0f, 0.7f)]
    [SerializeField] private float pitchRandomization = 0.1f;

    [Header("Audio Settings Persistence")]
    [SerializeField] private bool saveAudioSettings = true;
    [SerializeField] private string masterVolumePrefKey = "MasterVolume";
    [SerializeField] private string sfxVolumePrefKey = "SFXVolume";
    #endregion

    #region PRIVATE VARIABLES

    // SFX system variables
    private Queue<AudioSource> sfxSourcePool;
    private List<AudioSource> activeSfxSources;
    private Dictionary<string, AudioClip> sfxLookup;
    private Dictionary<string, float> soundImportance = new Dictionary<string, float>();
    private float lastSpinnerSoundTime = 0f; // Track when spinner sound was last played
    
    #endregion

    #region UNITY LIFECYCLE METHODS
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Initialize all components
        InitializeSFXsystem();

        // Call priorities after initializing other systems
        InitializeSoundPriorities();

        // Load saved audio settings
        if (saveAudioSettings)
        {
            LoadAudioSettings();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Return inactive SFX sources to the pool
        if (activeSfxSources != null)
        {
            for (int i = activeSfxSources.Count - 1; i >= 0; i--)
            {
                AudioSource source = activeSfxSources[i];
                if (source != null && !source.isPlaying)
                {
                    ReturnSFXSourceToPool(source);
                    activeSfxSources.RemoveAt(i);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    #endregion

    #region INITIALIZATION METHODS

    private void InitializeSFXsystem()
    {
        // Initialize SFX parent if not set
        if (sfxSourceParent == null)
        {
            GameObject sourcesParent = new GameObject("SFX Sources");
            sourcesParent.transform.SetParent(transform);
            sfxSourceParent = sourcesParent.transform;
        }

        // Initialize SFX audio source pools
        InitializeSFXSourcePool();

        // Create SFX lookup dictionary for faster access
        CreateSFXLookup();
    }

    private void InitializeSFXSourcePool()
    {
        sfxSourcePool = new Queue<AudioSource>(sfxSourcePoolSize);
        activeSfxSources = new List<AudioSource>();

        // Create audio sources and add them to the pool
        for (int i = 0; i < sfxSourcePoolSize; i++)
        {
            AudioSource newSource = CreateNewSFXSource();
            sfxSourcePool.Enqueue(newSource);
        }
    }

    private void InitializeSoundPriorities()
    {
        // Higher values = higher priority
        soundImportance.Add("Jackpot", 10f);
        soundImportance.Add("HighScore", 8f);
        soundImportance.Add("MediumScore", 6f);
        soundImportance.Add("LowScore", 4f);
        soundImportance.Add("BumperHit", 3f);
        soundImportance.Add("Spinner", 2.5f);
        soundImportance.Add("PinHit", 2f);
        soundImportance.Add("NewHighScore", 1.1f);
        // Default importance for sounds not in this list is 1.0
    }

    private AudioSource CreateNewSFXSource()
    {
        GameObject sourceObj = new GameObject("SFX Source");
        sourceObj.transform.SetParent(sfxSourceParent);

        AudioSource source  = sourceObj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.volume = defaultSFXVolume;
        sourceObj.SetActive(false);

        return source;
    }

    private void CreateSFXLookup()
    {
        sfxLookup = new Dictionary<string, AudioClip>
        {
            { "BallLaunch", ballLaunchSound },
            { "PinHit", pinHitSound },
            { "BumperHit", bumperHitSound },
            { "Spinner", spinnerSound },
            { "LowScore", lowScoreSound },
            { "MediumScore", mediumScoreSound },
            { "HighScore", highScoreSound },
            { "Jackpot", jackpotScoreSound },
            { "ButtonClick", buttonClickSound },
            { "Drain", drainSound },
            { "GameOver", gameOverSound },
            { "NewHighScore", newHighScoreSound},
            { "WoodenBorderHit", WoodenImpactSound }
        };
    }

    #endregion

    #region AUDIO SOURCE POOL METHODS

    private void ReturnSFXSourceToPool(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
            source.clip = null;
            source.loop = false; // Ensure it's not set to loop
            source.priority = 128; // Reset to default priority
            source.gameObject.SetActive(false);
            source.transform.SetParent(sfxSourceParent);
            source.transform.localPosition = Vector3.zero;
            source.spatialBlend = 0f; // Reset to 2D
            sfxSourcePool.Enqueue(source);
        }
    }

    private AudioSource GetSFXSourceFromPool()
    {
        AudioSource source;

        if (sfxSourcePool.Count > 0)
        {
            source = sfxSourcePool.Dequeue();
        }
        else
        {
            // Create a new source if the pool is empty
            source = CreateNewSFXSource();
            Debug.LogWarning("Audio source pool exhausted. Created additional source. Consider increasing pool size");
        }

        source.gameObject.SetActive(true);
        activeSfxSources.Add(source);

        return source;
    }
    #endregion

    #region PUBLIC SFX METHODS
    
    private float RandomizeVolume(float baseVolume)
    {
        // Apply random variation to volume while keeping it in valid range
        return Mathf.Clamp01(baseVolume + Random.Range(-volumeRandomization, volumeRandomization));
    }

    private float RandomizePitch(float basePitch)
    {
        // Apply random variation to pitch
        return basePitch + Random.Range(-pitchRandomization, pitchRandomization);
    }

    /// <summary>
    /// Plays a 2D sound effect from the predefined list
    /// </summary>
    /// <param name="sfxName">Name of the sound effect</param>
    /// <param name="volume">Volume level (0-1), -1 for default</param>
    /// <param name="pitch">Pitch adjustment (default 1.0)</param>
    public void PlaySFX(string sfxName, float volume = -1f, float pitch = 1f)
    {
        Debug.Log("PlaySFX called with sfxName: " + sfxName);
        
        // Get importance of this sound (default to 1.0 if nto specified)
        float importance = 1.0f;
        soundImportance.TryGetValue(sfxName, out importance);

        // If we have too many sounds playing, only play if important or stop a less important sound
        if (activeSfxSources.Count >= maxSimulataneousSounds)
        {
            Debug.LogWarning("Too many sounds playing (" + activeSfxSources.Count + "). Checking importance for " + sfxName);
            // Try to find a less important sound to stop
            AudioSource leastImportantSource = null;
            float lowestImportance = importance; // Only replace sounds less important than this one

            foreach (AudioSource activeSource in activeSfxSources)
            {
                if (activeSource.gameObject.name.Contains("Priority"))
                    continue; // Skip explicity high-priority sounds

                string activeSfxName = activeSource.gameObject.name;
                float activeImportance = 1.0f;
                soundImportance.TryGetValue(activeSfxName, out activeImportance);

                if (activeImportance < lowestImportance)
                {
                    lowestImportance = activeImportance;
                    leastImportantSource = activeSource;
                }
            }

            // If we found a less important sound, stop it and reuse its source
            if (leastImportantSource != null)
            {
                leastImportantSource.Stop();
                leastImportantSource.clip = sfxLookup[sfxName];
                leastImportantSource.gameObject.name = sfxName;
                
                // Calculate final volume and pitch with randomization
                float finalVolume = volume > 0 ? volume : defaultSFXVolume;
                finalVolume = RandomizeVolume(finalVolume);
                
                float finalPitch = RandomizePitch(pitch);
                
                leastImportantSource.pitch = finalPitch;
                leastImportantSource.volume = finalVolume;
                leastImportantSource.Play();
                return;
            }

            // If the sound is not important enough and we have no less important sounds to stop, just skip it
            if (importance < 3.0f) // adjust threshold as needed
            {
                Debug.LogWarning("Sound " + sfxName + " skipped due to low importance: " + importance);
                return;
            }
        }

        // Normal sound logic
        if (sfxLookup.TryGetValue(sfxName, out AudioClip clip) && clip != null)
        {
            Debug.Log("Found audio clip for " + sfxName + ": " + clip.name);
            AudioSource source = GetSFXSourceFromPool();
            source.gameObject.name = sfxName; // Name the GameObject for debugging
            source.clip = clip;
            source.spatialBlend = 0f; // 2D sound
            
            // Calculate final volume and pitch with randomization
            float finalVolume = volume > 0 ? volume : defaultSFXVolume;
            finalVolume = RandomizeVolume(finalVolume);
            
            float finalPitch = RandomizePitch(pitch);
            
            source.pitch = finalPitch;
            source.volume = finalVolume;
            source.Play();
            Debug.Log("Playing " + sfxName + " with volume: " + source.volume + ", pitch: " + source.pitch);
        }
        else
        {
            Debug.LogError("Sound effect '" + sfxName + "' not found or is null. Lookup result: " + (clip != null ? "clip exists" : "clip is null"));
        }
    }

    /// <summary>
    /// Plays a 3D sound effect at a specific position in the world
    /// </summary>
    /// <param name="sfxName">Name of the sound effect</param>
    /// <param name="position">Position in 3D space</param>
    /// <param name="volume">Volume level (0-1), -1 for default</param>
    /// <param name="pitch">Pitch adjustment (default 1.0)</param>
    /// <param name="minDistance">Minimum distance for 3D sound attenuation</param>
    /// <param name="maxDistance">Maximum distance for 3D sound attenuation</param>
    public void PlaySFX3D(string sfxName, Vector3 position, float volume = -1f, float pitch = 1f, float minDistance = 1f, float maxDistance = 50f)
    {
        if (sfxLookup.TryGetValue(sfxName, out AudioClip clip) && clip != null)
        {
            AudioSource source = GetSFXSourceFromPool();
            source.clip = clip;
            source.transform.position = position;
            source.spatialBlend = 1f; // 3D sound
            
            // Calculate final volume and pitch with randomization
            float finalVolume = volume > 0 ? volume : defaultSFXVolume;
            finalVolume = RandomizeVolume(finalVolume);
            
            float finalPitch = RandomizePitch(pitch);
            
            source.pitch = finalPitch;
            source.volume = finalVolume;
            source.minDistance = minDistance;
            source.maxDistance = maxDistance;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.Play();
        }
        else
        {
        Debug.LogWarning($"Sound effect '{sfxName}' not found.");
        }
    }

    /// <summary>
    /// Play a sound effect directly from an AudioClip
    /// </summary>
    /// <param name="clip">AudioClip to play</param>
    /// <param name="position">Optional 3D position (null for 2D sound)</param>
    /// <param name="volume">Volume level (0-1), -1 for default</param>
    /// <param name="pitch">Pitch adjustment (default 1.0)</param>
    public void PlaySound(AudioClip clip, Vector3? position = null, float volume = -1f, float pitch = 1f)
    {
        if (clip == null) return;

        AudioSource source = GetSFXSourceFromPool();
        source.clip = clip;

        if (position.HasValue)
        {
            source.transform.position = position.Value;
            source.spatialBlend = 1f; // 3D sound
        }
        else
        {
            source.spatialBlend = 0f; // 2D sound
        }

        // Calculate final volume and pitch with randomization
        float finalVolume = volume > 0 ? volume : defaultSFXVolume;
        finalVolume = RandomizeVolume(finalVolume);
        
        float finalPitch = RandomizePitch(pitch);
        
        source.pitch = finalPitch;
        source.volume = finalVolume;
        source.Play();
    }

    /// <summary>
    /// Play a sound for scoring based on the point value
    /// </summary>
    /// <param name="points">Score points</param>
    /// <param name="position">Position in 3D space</param>
    public void PlayScoreSound(int points, Vector3 position)
    {
        string sfxName;

        if (points >= 5000)
        {
            sfxName = "Jackpot";
        }
        else if (points >= 1000)
        {
            sfxName = "HighScore";
        }
        else if (points >= 500)
        {
            sfxName = "MediumScore";
        }
        else
        {
            sfxName = "LowScore";
        }

        Debug.Log("Playing score sound: " + sfxName);
        PlaySFX3D(sfxName, position);
    }
    
    /// <summary>
    /// Play the ball launcher sound with power-based variations
    /// </summary>
    /// <param name="launchPower">Power level (0-1)</param>
    public void PlayBallLaunchSound(float launchPower = 1.0f)
    {
        // Adjust pitch based on launch power
        float pitch = Mathf.Lerp(0.8f, 1.2f, launchPower);
        PlaySFX("BallLaunch", -1f, pitch);
    }

    /// <summary>
    /// Play a pin hit sound with force-based variations
    /// </summary>
    /// <param name="position">Position in 3D space</param>
    /// <param name="hitForce">Force of the hit (affects volume/pitch)</param>
    public void PlayPinHitSound(Vector3 position, float hitForce = 1.0f, int material = 0)
    {
        PlaySFX3D("PinHit", position, 0.2f, 1f, 30f);
    }

    /// <summary>
    /// Play bumper hit sound with force-based variations
    /// </summary>
    /// <param name="position">Position in 3D space</param>
    /// <param name="hitforce">Force of the hit (affects pitch)</param>
    public void PlayBumperHitSound(Vector3 position, float hitforce = 1.0f)
    {
        Debug.Log($"Playing bumper hit sound at position {position}");
        PlaySFX3D("BumperHit", position, -1f, 1f, 30f);
    }

    /// <summary>
    /// Play spinner sound with speed-based variations
    /// </summary>
    /// <param name="position">Position in 3D space</param> 
    /// <param name="rotationSpeed">Speed of rotation (affects pitch)</param>
    public void PlaySpinnerSound(Vector3 position, float rotationSpeed = 1.0f)
    {
        Debug.Log($"Playing spinner sound at position {position}");
        
        // Check if enough time has passed since the last spinner sound
        if (Time.time - lastSpinnerSoundTime < spinnerSoundCooldown)
        {
            Debug.Log("Spinner sound cooldown active, skipping sound");
            return;
        }
        
        PlaySFX3D("Spinner", position, -1f, 1f, 25f);
        lastSpinnerSoundTime = Time.time; // Update the last play time
    }

    /// <summary>
    /// Play UI button click sound
    /// </summary>
    public void PlayButtonClickSound()
    {
        PlaySFX("ButtonClick");
    }

    /// <summary
    /// Play ball drain from playfield sound
    /// </summary>
    public void PlayDrainSound()
    {
        PlaySFX3D("Drain", Vector3.zero, -1f, 1f, 1f, 30f);
    }

    /// <summary>
    /// Play game over sounds
    /// </summary>
    public void PlayGameOverSound()
    {
        Debug.Log("PlayGameOverSound method called");
        
        // Check if it's in the lookup dictionary
        if (sfxLookup.ContainsKey("GameOver"))
        {
            if (sfxLookup["GameOver"] == null)
            {
                Debug.LogError("GameOver entry exists in sfxLookup but the clip is null");
            }
            else
            {
                Debug.Log("GameOver sound found in sfxLookup: " + sfxLookup["GameOver"].name);
            }
        }
        else
        {
            Debug.Log("Game over sound clip exists: " + gameOverSound.name);
            // Play sound directly instead of through lookup which might be null
            PlaySFX("GameOver");
        }
    }

    /// <summary>
    /// Play new high score sound
    /// </summary>
    public void PlayNewHighScoreSound()
    {
        if (sfxLookup.ContainsKey("NewHighScore"))
        {
            PlaySFX("NewHighScore");
        }
        else
        {
            Debug.LogError("NewHighScore sound not found in sfxLookup");
        }
    }

    private IEnumerator PlayDelayedSound(string sfxName, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlaySFX(sfxName);
    }

    /// <summary>
    /// Play wooden border hit sound with force-based variations
    /// </summary>
    /// <param name="position">Position in 3D space</param>
    /// <param name="hitForce">Force of the hit (affects volume/pitch)</param>
    public void PlayWoodenImpactSound(Vector3 position, float hitForce = 1.0f)
    {
        PlaySFX3D("WoodenBorderHit", position, -1f, 1f, 30f);
    }
    #endregion

    #region VOLUME CONTROL METHODS

    /// <summary>
    /// Check if an AudioMixer parameter exists
    /// </summary>
    private bool AudioMixerParameterCheck(string paramName)
    {
        if (audioMixer == null) return false;

        float value;
        return audioMixer.GetFloat(paramName, out value);
    }

    /// <summary>
    /// Set the master volume level 
    /// </summary>
    /// <param name="volume">Volume level (0-1)</param>
    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        // Convert to decibels: Mathf.Log10(value) * 20f
        // Silence when volume is 0: -80dB
        float decibelValue = volume > 0 ? Mathf.Log10(volume) * 20f : -80f;

        if (audioMixer != null && AudioMixerParameterCheck(masterVolumeParam))
        {
            audioMixer.SetFloat(masterVolumeParam, decibelValue);
        }
        else if (audioMixer != null)
        {
            Debug.LogWarning($"AudioMixer parameter '{masterVolumeParam}' not found. Make sure to expose this parameter in the AudioMixer.");
        }

        if (saveAudioSettings)
        {
            PlayerPrefs.SetFloat(masterVolumePrefKey, volume);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Set the SFX Volume level
    /// </summary>
    /// <param name="volume">Volume level (0-1)</param>
    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);
        defaultSFXVolume = volume;

        float decibelValue = volume > 0 ? Mathf.Log10(volume) * 20f : -80f;

        if (audioMixer != null)
        {
            audioMixer.SetFloat(sfxVolumeParam, decibelValue);
        }

        if (saveAudioSettings)
        {
            PlayerPrefs.SetFloat(sfxVolumePrefKey, volume);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Loads audio settings from player pref
    /// </summary>
    private void LoadAudioSettings()
    {
        // Use default values if not fouond
        float masterVolume = PlayerPrefs.GetFloat(masterVolumePrefKey, 1.0f);
        float sfxVolume = PlayerPrefs.GetFloat(sfxVolumePrefKey, defaultSFXVolume);

        // Apply saved values without saving them again
        bool originalSaveSetting = saveAudioSettings;
        saveAudioSettings = false;

        SetMasterVolume(masterVolume);
        SetSFXVolume(sfxVolume);

        saveAudioSettings = originalSaveSetting;
    }
    #endregion
}
