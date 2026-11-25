using System.Collections.Generic;
using DZAssets.AudioManager;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[RequireComponent(typeof(AudioManager))]
public class GameMusicManager : MonoBehaviour
{
    [SerializeField] private AudioData bgmData;
    [SerializeField] private string[] gameplayTracks = { "GameMusic1", "GameMusic2", "GameMusic3" };

    private const string MenuTrack = "MenuMusic";

    private enum MusicMode { Silent, Menu, Gameplay }

    private readonly List<AudioData.AudioEntry> playlist = new();
    private AudioManager audioManager;
    private AudioData.AudioEntry menuTrack;
    private AudioSource gameplaySource;
    private MusicMode mode;
    private int nextTrack;
    private AudioClip lastClip;

    private void Start()
    {
        audioManager = GetComponent<AudioManager>();
        if (audioManager != AudioManager.Instance)
            return;

        menuTrack = FindTrack(MenuTrack);
        foreach (string trackName in gameplayTracks)
        {
            AudioData.AudioEntry track = FindTrack(trackName);
            if (track != null && track.audioClip != menuTrack?.audioClip &&
                !playlist.Exists(entry => entry.audioClip == track.audioClip))
                playlist.Add(track);
        }

        if (menuTrack == null || playlist.Count != 3)
            Debug.LogWarning("Music needs MenuMusic and three distinct gameplay tracks in BGM Data.", this);

        nextTrack = playlist.Count;
        SceneManager.activeSceneChanged += OnSceneChanged;
        ApplyScene(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private AudioData.AudioEntry FindTrack(string trackName)
    {
        if (bgmData == null || string.IsNullOrWhiteSpace(trackName))
            return null;

        return bgmData.audioList.FindLast(entry => entry.nameAudio == trackName && entry.audioClip != null);
    }

    private void OnSceneChanged(Scene previous, Scene current)
    {
        ApplyScene(current);
    }

    private void ApplyScene(Scene scene)
    {
        MusicMode nextMode = scene.name switch
        {
            "MENU" or "SETTINGS" or "CREDITS" => MusicMode.Menu,
            "LEVEL ONE" => MusicMode.Gameplay,
            _ => MusicMode.Silent
        };

        if (nextMode == mode)
            return;

        mode = nextMode;
        gameplaySource = null;

        switch (mode)
        {
            case MusicMode.Menu:
                if (menuTrack == null)
                    break;

                AudioSource menuSource = PlayTrack(menuTrack, true);
                if (menuSource != null)
                    menuSource.timeSamples = 0;
                break;

            case MusicMode.Gameplay:
                nextTrack = playlist.Count;
                lastClip = null;
                PlayNextTrack();
                break;

            default:
                audioManager.StopBGM();
                break;
        }
    }

    private void Update()
    {
        if (gameplaySource != null && !AudioListener.pause && !gameplaySource.isPlaying)
            PlayNextTrack();
    }

    private void PlayNextTrack()
    {
        if (playlist.Count == 0)
        {
            audioManager.StopBGM();
            return;
        }

        if (nextTrack == playlist.Count)
        {
            // Shuffle a fresh set and avoid a repeat across sets.
            for (int i = playlist.Count - 1; i > 0; i--)
                Swap(i, Random.Range(0, i + 1));

            if (playlist.Count > 1 && playlist[0].audioClip == lastClip)
                Swap(0, Random.Range(1, playlist.Count));

            nextTrack = 0;
        }

        AudioData.AudioEntry track = playlist[nextTrack++];
        lastClip = track.audioClip;
        gameplaySource = PlayTrack(track, false);
    }

    private void Swap(int first, int second)
    {
        (playlist[first], playlist[second]) = (playlist[second], playlist[first]);
    }

    private AudioSource PlayTrack(AudioData.AudioEntry track, bool loop)
    {
        audioManager.PlayBGM(track.nameAudio);

        foreach (AudioSource source in audioManager.GetComponentsInChildren<AudioSource>())
        {
            if (source.clip == track.audioClip)
            {
                source.loop = loop;
                return source;
            }
        }

        Debug.LogWarning($"Music could not play '{track.nameAudio}'. Check the BGM Data assigned to DZ Audio Manager.", this);
        return null;
    }
}
