using UnityEngine;
using System.Collections.Generic;

namespace DZAssets.AudioManager
{
    // AUDIO MANAGER BY DANY ZAKY
    // This audio manager is sourced online from the Unity Asset store, with light changes from Evan Montgomery

    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Data")]
        [SerializeField] private AudioData bgmData;
        [SerializeField] private AudioData sfxData;

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgmAudioSource;
        [SerializeField] private AudioSource sfxAudioSource;

        [Header("Settings")]
        [SerializeField] private bool useLog = true;
        [SerializeField, Min(0f)] private float bgmFadeDuration = 1f;

        public float BGMVolume => bgmVolume;
        public float SFXVolume => sfxAudioSource.volume;

        private AudioSource secondaryBgmSource;
        private AudioSource activeBgmSource;
        private float bgmVolume;
        private float primaryGain;
        private float secondaryGain;
        private float primaryStartGain;
        private float secondaryStartGain;
        private float fadeElapsed;
        private bool fading;
        private bool bgmPaused;

        private static AudioManager instance;

        // Cache untuk menghindari pencarian berulang
        private Dictionary<string, AudioClip> bgmCache = new Dictionary<string, AudioClip>();
        private Dictionary<string, AudioClip> sfxCache = new Dictionary<string, AudioClip>();
        private bool cacheInitialized = false;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<AudioManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("AudioManager");
                        instance = go.AddComponent<AudioManager>();
                    }
                }
                return instance;
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
                if (useLog) Debug.Log("[DZAssets] AudioManager: Initialized successfully");
            }
            else if (instance != this)
            {
                if (useLog) Debug.LogWarning("[DZAssets] AudioManager: Duplicate instance destroyed");
                Destroy(gameObject);
            }
        }

        void InitializeAudioSources()
        {
            if (bgmAudioSource == null)
            {
                bgmAudioSource = gameObject.AddComponent<AudioSource>();
                bgmAudioSource.loop = true;
                bgmAudioSource.playOnAwake = false;
            }

            if (sfxAudioSource == null)
            {
                sfxAudioSource = gameObject.AddComponent<AudioSource>();
                sfxAudioSource.loop = false;
                sfxAudioSource.playOnAwake = false;
            }

            secondaryBgmSource = gameObject.AddComponent<AudioSource>();
            secondaryBgmSource.outputAudioMixerGroup = bgmAudioSource.outputAudioMixerGroup;
            secondaryBgmSource.priority = bgmAudioSource.priority;
            secondaryBgmSource.pitch = bgmAudioSource.pitch;
            secondaryBgmSource.spatialBlend = bgmAudioSource.spatialBlend;
            secondaryBgmSource.ignoreListenerPause = bgmAudioSource.ignoreListenerPause;
            secondaryBgmSource.loop = bgmAudioSource.loop;
            secondaryBgmSource.playOnAwake = false;
            bgmVolume = bgmAudioSource.volume;
            primaryGain = bgmAudioSource.isPlaying ? 1f : 0f;
            activeBgmSource = bgmAudioSource.isPlaying ? bgmAudioSource : null;
            ApplyBGMVolumes();
            InitializeCache();
        }

        void InitializeCache()
        {
            if (cacheInitialized) return;

            // Cache BGM
            if (bgmData?.audioList != null)
            {
                bgmCache.Clear();
                foreach (var entry in bgmData.audioList)
                {
                    if (!string.IsNullOrEmpty(entry.nameAudio) && entry.audioClip != null)
                        bgmCache[entry.nameAudio] = entry.audioClip;
                }
            }

            // Cache SFX
            if (sfxData?.audioList != null)
            {
                sfxCache.Clear();
                foreach (var entry in sfxData.audioList)
                {
                    if (!string.IsNullOrEmpty(entry.nameAudio) && entry.audioClip != null)
                        sfxCache[entry.nameAudio] = entry.audioClip;
                }
            }

            cacheInitialized = true;
        }

        #region BGM Methods
        public void PlayBGM(string audioName)
        {
            if (string.IsNullOrEmpty(audioName)) return;

            if (!cacheInitialized) InitializeCache();

            if (bgmCache.TryGetValue(audioName, out AudioClip clip))
            {
                if (useLog) Debug.Log($"[DZAssets] AudioManager: Playing BGM '{audioName}'");
                if (activeBgmSource != null && activeBgmSource.clip == clip &&
                    (activeBgmSource.isPlaying || bgmPaused)) return;

                // Reuse a fading song without restarting it.
                AudioSource next = null;
                if (bgmAudioSource.clip == clip && (bgmAudioSource.isPlaying || bgmPaused))
                    next = bgmAudioSource;
                else if (secondaryBgmSource.clip == clip && (secondaryBgmSource.isPlaying || bgmPaused))
                    next = secondaryBgmSource;

                if (next == null)
                {
                    next = primaryGain <= secondaryGain ? bgmAudioSource : secondaryBgmSource;
                    next.Stop();
                    if (next == bgmAudioSource) primaryGain = 0f;
                    else secondaryGain = 0f;
                    next.volume = 0f;
                    next.clip = clip;
                    next.Play();
                    if (bgmPaused) next.Pause();
                }
                activeBgmSource = next;
                BeginBGMFade();
            }
            else
            {
                if (useLog) Debug.LogWarning($"[DZAssets] AudioManager: BGM '{audioName}' not found in BGM cache!");
            }
        }

        public void StopBGM()
        {
            if (bgmAudioSource != null)
            {
                if (useLog) Debug.Log("[DZAssets] AudioManager: Stopping BGM");
                activeBgmSource = null;
                BeginBGMFade();
            }
        }

        public void PauseBGM()
        {
            if (bgmAudioSource != null)
            {
                bgmPaused = true;
                bgmAudioSource.Pause();
                secondaryBgmSource.Pause();
            }
        }

        public void ResumeBGM()
        {
            if (bgmAudioSource != null)
            {
                bgmPaused = false;
                bgmAudioSource.UnPause();
                secondaryBgmSource.UnPause();
            }
        }
        #endregion

        private void BeginBGMFade()
        {
            primaryStartGain = primaryGain;
            secondaryStartGain = secondaryGain;
            fadeElapsed = 0f;
            fading = true;
            if (bgmFadeDuration <= 0f) UpdateBGMFade(1f);
        }

        private void Update()
        {
            if (!fading || bgmPaused) return;
            fadeElapsed += Time.unscaledDeltaTime;
            UpdateBGMFade(bgmFadeDuration <= 0f ? 1f :
                Mathf.Clamp01(fadeElapsed / bgmFadeDuration));
        }

        private void UpdateBGMFade(float progress)
        {
            primaryGain = Mathf.Lerp(primaryStartGain,
                activeBgmSource == bgmAudioSource ? 1f : 0f, progress);
            secondaryGain = Mathf.Lerp(secondaryStartGain,
                activeBgmSource == secondaryBgmSource ? 1f : 0f, progress);
            ApplyBGMVolumes();
            if (progress < 1f) return;

            fading = false;
            StopInactiveBGM(bgmAudioSource);
            StopInactiveBGM(secondaryBgmSource);
        }

        private void ApplyBGMVolumes()
        {
            bgmAudioSource.volume = bgmVolume * primaryGain;
            secondaryBgmSource.volume = bgmVolume * secondaryGain;
        }

        private void StopInactiveBGM(AudioSource source)
        {
            if (source == activeBgmSource) return;
            source.Stop();
            source.clip = null;
        }

        #region SFX Methods
        public void PlaySFX(string audioName)
        {
            if (string.IsNullOrEmpty(audioName)) return;

            if (!cacheInitialized) InitializeCache();

            if (sfxCache.TryGetValue(audioName, out AudioClip clip))
            {
                if (useLog) Debug.Log($"[DZAssets] AudioManager: Playing SFX '{audioName}'");
                sfxAudioSource.PlayOneShot(clip);
            }
            else
            {
                if (useLog) Debug.LogWarning($"[DZAssets] AudioManager: SFX '{audioName}' not found in SFX cache!");
            }
        }

        public void StopSFX()
        {
            if (sfxAudioSource != null)
            {
                if (useLog) Debug.Log("[DZAssets] AudioManager: Stopping SFX");
                sfxAudioSource.Stop();
            }
        }
        #endregion

        #region Helper Methods
        // Method ini masih diperlukan untuk GetAvailableAudioNames
        private AudioClip GetAudioClip(AudioData audioData, string audioName)
        {
            if (audioData == null || audioData.audioList == null) return null;

            foreach (var entry in audioData.audioList)
            {
                if (entry.nameAudio == audioName)
                {
                    return entry.audioClip;
                }
            }
            return null;
        }

        public void RefreshCache()
        {
            cacheInitialized = false;
            InitializeCache();
            if (useLog) Debug.Log("[DZAssets] AudioManager: Cache refreshed");
        }

        public void SetBGMVolume(float volume)
        {
            if (bgmAudioSource != null)
            {
                bgmVolume = Mathf.Clamp01(volume);
                ApplyBGMVolumes();
            }
        }

        public void SetSFXVolume(float volume)
        {
            if (sfxAudioSource != null)
            {
                sfxAudioSource.volume = Mathf.Clamp01(volume);
            }
        }

        private string GetAvailableAudioNames(AudioData audioData)
        {
            if (audioData == null || audioData.audioList == null || audioData.audioList.Count == 0)
                return "None";

            System.Collections.Generic.List<string> names = new System.Collections.Generic.List<string>();
            foreach (var entry in audioData.audioList)
            {
                if (!string.IsNullOrEmpty(entry.nameAudio))
                    names.Add(entry.nameAudio);
            }
            return names.Count > 0 ? string.Join(", ", names) : "None";
        }
        #endregion
    }
}