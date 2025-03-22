using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

namespace AudioManager
{
    /// <summary>
    /// Handles all music-related functionality for the game.
    /// Manages playing songs, controlling intensity, and transitioning between songs
    /// Courtesy of Ovani Sound
    /// </summary>
    public class Jukebox : MonoBehaviour
    {
        // Singleton instance
        public static Jukebox Instance { get; private set; }

        #region INSPECTOR FIELDS

        [Header("Audio Mixing")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string masterVolumeParam = "MasterVolume";
        [SerializeField] private string musicVolumeParam = "MusicVolume";

        [Header("Music Settings")]
        [Tooltip("The audio source prefab for music playback")] 
        [SerializeField] private GameObject musicSourcePrefab;
        [Tooltip("Should the current song loop?")] 
        [SerializeField] private bool loopCurrentSong = true;
        [Tooltip("Should the manager play the first song on awake?")] 
        [SerializeField] private bool playMusicOnAwake = true;
        [Tooltip("The maximum volume for music playback.")] 
        [Range(0f, 1f)] [SerializeField] private float maxMusicVolume = 1f;
        [Tooltip("The amount of time for songs to blend between one-another.")] 
        [SerializeField] private float defaultSongBlendDuration = 1f;
        [Tooltip("The amount of time for intensities to blend between one-another.")] 
        [SerializeField] private float defaultIntensityBlendDuration = 0.5f;

        [Header("Music Library")]
        [Tooltip(
            "The list of available songs for the manager to play.\n" +
            "-To create a new song:\n" +
            "  -Right Click and select:\n" +
            "    Create->AudioManager->Song\n" +
            "  -Add the intensity clips or any other desired clips\n" +
            "  -Set the reverb tail time\n" +
            "    (Seconds before the end of a clip to loop it)\n" +
            "  -Add a descriptive song name\n" +
            "  -Drag & Drop your songs onto this list")] 
        [SerializeField] private List<Song> songs = new List<Song>();

        [Header("Audio Settings Persistence")]
        [SerializeField] private bool saveAudioSettings = true;
        [SerializeField] private string masterVolumePrefKey = "MasterVolume";
        [SerializeField] private string musicVolumePrefKey = "MusicVolume";

        #endregion

        #region PRIVATE VARIABLES

        // Music system variables
        private List<AudioSource> musicSourcePool = new List<AudioSource>();
        private int currentMusicSourceIndex = -1;
        private AudioSource CurrentMusicSource => (currentMusicSourceIndex == -1 || musicSourcePool == null || musicSourcePool.Count == 0 || musicSourcePool.Count <= currentMusicSourceIndex) ? null : musicSourcePool[currentMusicSourceIndex];
        private Coroutine musicLoopCoroutine;
        private Song.Data currentSongData;
        private List<Song.Data> songDataCollection = new List<Song.Data>();
        private int currentSongIndex = 0;
        private int currentIntensityIndex = 0;
        private double nextLoopStartTime = 0;
        
        #endregion

        #region UNITY LIFECYCLE METHODS
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Optional: persist across scenes
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            // Initialize all components
            InitializeMusicSystem();

            // Load saved audio settings
            if (saveAudioSettings)
            {
                LoadAudioSettings();

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

        private void InitializeMusicSystem()
        {
            // Prepare song data collection
            UpdateSongData();

            // Begin music playback if configured
            if (playMusicOnAwake && songs.Count > 0)
            {
                StartCoroutine(DelayedMusicStart());
            }
        }

        private IEnumerator DelayedMusicStart()
        {
            yield return new WaitForSecondsRealtime(0.25f);
            PlaySong(0);
        }

        #endregion

        #region MUSIC SYSTEM METHODS

        // Updates the song data collection from song assets
        // foreach song (s) it calls s.ToSongData() to dcreate a new Song.Data struct
        private void UpdateSongData()
        {
            songDataCollection.Clear();
            songs.ForEach(s => songDataCollection.Add(s.ToSongData()));
        }

        // Gets the next available music source that is not playing
        private int GetNextMusicSourceIndex()
        {
            AudioSource next = musicSourcePool.Find(s => !s.isPlaying);
            if (next == null)
            {
                next = Instantiate(musicSourcePrefab, transform).GetComponent<AudioSource>();
                musicSourcePool.Add(next);
            }
            next.gameObject.SetActive(true);
            return musicSourcePool.IndexOf(next);
        }

        // Force an audio source to fade in or out
        private IEnumerator FadeMusicVolume(AudioSource source, float startVolume, float endVolume, float duration)
        {
            duration = Mathf.Max(duration, 0f);
            float elapsed = 0f;

            // Perform volume fade
            while (elapsed < duration)
            {
                yield return new WaitForEndOfFrame();
                elapsed += Time.unscaledDeltaTime;
                source.volume = Mathf.SmoothStep(startVolume, endVolume, elapsed / duration);
            }

            // Ensure volume is at desired volume
            source.volume = endVolume;

            // If the volume is 0 or
            // The fade was between teh same values
            // (this is used when looping to allow for the previous clip to play without fading)
            if (source.volume == 0f || endVolume == startVolume)
            {
                // If the fade is for the current audio source
                if (source == CurrentMusicSource)
                {
                    musicSourcePool.ForEach(s => s.Stop());
                    if (musicLoopCoroutine != null)
                    {
                        StopCoroutine(musicLoopCoroutine);
                    }
                    currentMusicSourceIndex = -1;
                    nextLoopStartTime = 0f;
                }
                source.volume = 0f;
                source.Stop();
                source.gameObject.SetActive(false);
            }
        }

        // Plays the provided song after a wait period 
        //(.dspTime) returns the current time of the audio system
        private IEnumerator LoopMusicCoroutine(float startTime)
        {
            float fullLength = currentSongData.intensityClips[currentIntensityIndex].length;
            float waitTime = fullLength - currentSongData.reverbTail - startTime;
            nextLoopStartTime = AudioSettings.dspTime + waitTime;
            yield return new WaitForSecondsRealtime(waitTime);

            if (!loopCurrentSong)
            {
                // Queue the current audio source to play out for the remainder of the duration
                AudioSource current = CurrentMusicSource;
                StartCoroutine(FadeMusicVolume(current, current.volume, current.volume, currentSongData.reverbTail));
                yield break;
            }

            // If looping, play the song
            PlaySong(new PlaySongOptions
            {
                song = currentSongIndex,
                intensity = currentIntensityIndex,
                blendOutTime = -1f,
                blendInTime = 0.01f
            });
        }

        #endregion

        #region PUBLIC MUSIC METHODS
            /// <summary>
            /// We're using a new design concept here, Method Overloading
            /// By creating two methods with the same name but different parameters.
            /// This provides a simpler interface when default values are sufficent
            /// Set the intensity level for the current song
            /// </summary>
            /// <param name="intensity">Intensity level index</param>
            public void SetIntensity(int intensity)
            {
                SetIntensity(intensity, defaultIntensityBlendDuration, defaultIntensityBlendDuration);
            }
            /// <summary>
            /// Sets the intensity level for the current song with custom blend durations
            /// </summary>
            /// <param name="intensity">Intensity level index</param>
            /// <param name="blendOutDuration">Duration to blend out the current intensity</param>
            /// <param name="blendInDuration">Duration to blend in the new intensity</param>
            public void SetIntensity(int intensity, float blendOutDuration, float blendInDuration)
            {
                if (currentSongData.intensityClips.Count > Mathf.Max(intensity, 0))
                {
                    PlaySong(new PlaySongOptions
                    {
                        song = currentSongIndex,
                        intensity = Mathf.Max(intensity, 0),
                        startTime = musicSourcePool[currentMusicSourceIndex].time,
                        blendOutTime = blendOutDuration,
                        blendInTime = blendInDuration
                    });
                }
            }
            /// <summary>
            /// Options structure for playing songs with detailed range control
            /// </summary>
            public struct PlaySongOptions
            {
                public int song;
                public int intensity;
                public float startTime;
                public float blendOutTime;
                public float blendInTime;
            }
            /// <summary>
            /// Plays the specified song and attempts to mathch the current intensity
            /// </summary>
            /// <param name="songIndex">Index of the song to play</param>
            public void PlaySong(int songIndex)
            {
                PlaySong(new PlaySongOptions
                {
                    song = songIndex,
                    intensity = currentIntensityIndex,
                    startTime = 0f,
                    blendOutTime = defaultSongBlendDuration,
                    blendInTime = defaultSongBlendDuration
                });
            }

            /// <summary>
            /// Plays the specified song with custom options
            /// </summary>
            /// <param name="options">Options for controlling song playback</param>
            public void PlaySong(PlaySongOptions options)
            {
                // Updates the data collection
                UpdateSongData();

                // Confirms song exists, clip exists
                if (songDataCollection == null || songDataCollection.Count == 0 || songDataCollection.Count <= options.song) return;
                if (songDataCollection[options.song].intensityClips == null || songDataCollection[options.song].intensityClips.Count == 0) return;

                // Do our best to match intensity
                if (songDataCollection[options.song].intensityClips.Count <= options.intensity)
                {
                    options.intensity = songDataCollection[options.song].intensityClips.Count -1;
                }

                // Get the next available audio source
                if (currentMusicSourceIndex != -1)
                {
                    AudioSource current = CurrentMusicSource;
                    // Passing in -1 blendOutTime will let the source play the audio until the end of the track at its current volume
                    float endVolume = (options.blendOutTime == -1f) ? current.volume : 0f;
                    float fadeTime = (options.blendOutTime == -1f) ? currentSongData.reverbTail : options.blendOutTime;
                    StartCoroutine(FadeMusicVolume(current, current.volume, endVolume, fadeTime));
                }

                // Change the current source to the next available source
                currentMusicSourceIndex = GetNextMusicSourceIndex();
                AudioSource nextSource = CurrentMusicSource;

                // Apply the provided options
                currentSongIndex = options.song;
                currentIntensityIndex = options.intensity;
                currentSongData = songDataCollection[currentSongIndex];
                AudioClip clip = currentSongData.intensityClips[currentIntensityIndex];
                nextSource.gameObject.name = clip.name;

                // Kill the previous loop and start a new loop routine with the updated song info
                if (musicLoopCoroutine != null) StopCoroutine(musicLoopCoroutine);
                musicLoopCoroutine = StartCoroutine(LoopMusicCoroutine(options.startTime));
                StartCoroutine(FadeMusicVolume(nextSource, 0f, maxMusicVolume, options.blendInTime));
                nextSource.clip = clip;
                nextSource.time = options.startTime;
                nextSource.Play();
            }

            /// <summary>
            /// Stops the current song playing with an optional fade out
            /// </summary>
            /// <param name="fadeOutDuration">Duration of the fade out in seconds</param>
            public void StopSong(float fadeOutDuration = 1.0f)
            {
                if (CurrentMusicSource != null)
                {
                    AudioSource current = CurrentMusicSource;
                    StartCoroutine(FadeMusicVolume(current, current.volume, 0f, fadeOutDuration));
                }
            }

            /// <summary>
            /// Gets the time before either a non-looping clip ends or the next loop of a looping clip begins
            /// </summary>
            /// <returns>Time in seconds</returns>
            public float GetClipTimeRemaining()
            {
                if (nextLoopStartTime != 0f)
                {
                    return (float)((nextLoopStartTime - AudioSettings.dspTime) + ((!loopCurrentSong) ? currentSongData.reverbTail : 0f));
                }
                return 0f;
            }

        #endregion

        #region VOLUME CONTROL METHODS
        /// <summary>
        /// Set the master volume level for all audio
        /// </summary>
        /// <param name="volume">Volume level 0-1</param>
        public void SetMasterVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);

            // Convert to decibels: Mathf.Log10(value) * 20f
            // Silence when volume is 0: -80dB
            float decibelValue = volume > 0 ? Mathf.Log10(volume) * 20f : -80f;

            if (audioMixer != null)
            {
                audioMixer.SetFloat(masterVolumeParam, decibelValue);
            }

            if (saveAudioSettings)
            {
                PlayerPrefs.SetFloat(masterVolumePrefKey, volume);
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// Set the music volume level
        /// </summary>
        /// <param name="volume">Volume level (0-1)</param>
        public void SetMusicVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);

            // Convert to decvbels: Mathf.Log10(value) * 20f
            // Silence when volume is 0: -80dB
            float decibelValue = volume > 0 ? Mathf.Log10(volume) * 20f : -80f;

            if (audioMixer != null)
            {
                audioMixer.SetFloat(musicVolumeParam, decibelValue);
            }

            if (saveAudioSettings)
            {
                PlayerPrefs.SetFloat(musicVolumePrefKey, volume);
                PlayerPrefs.Save();
            }
        }

        // Load audio settings from player prefs
        private void LoadAudioSettings()
        {
            // Use default values if not fouond
            float masterVolume = PlayerPrefs.GetFloat(masterVolumePrefKey, 1.0f);
            float musicVolume = PlayerPrefs.GetFloat(musicVolumePrefKey, maxMusicVolume);

            // Apply saved values without saving them again
            bool originalSaveSetting = saveAudioSettings;
            saveAudioSettings = false;

            SetMasterVolume(masterVolume);
            SetMusicVolume(musicVolume);

            saveAudioSettings = originalSaveSetting;
        }
        #endregion
    }
}
