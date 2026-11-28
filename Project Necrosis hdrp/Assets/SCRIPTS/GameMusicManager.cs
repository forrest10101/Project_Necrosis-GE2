using System.Collections.Generic;
using DZAssets.AudioManager;
using UnityEngine;
using UnityEngine.SceneManagement;

// GAME MUSIC MANAGER, written by Evan Montgomery
[DisallowMultipleComponent]                                                 // Prevent more than one music manager on this object
[RequireComponent(typeof(AudioManager))]                                    // Require an Audio Manager on this object
public class GameMusicManager : MonoBehaviour
{
    [SerializeField] private AudioData bgmData;                                                                 // Audio data containing the music tracks
    [SerializeField] private string[] gameplayTracks = { "GameMusic1", "GameMusic2", "GameMusic3" };            // Gameplay track names

    private const string MenuTrack = "MenuMusic";                           // Name of the menu music track

    private enum MusicMode { Silent, Menu, Gameplay }                       // The type of music currently playing

    private readonly List<AudioData.AudioEntry> playlist = new();           // Gameplay tracks in shuffled order
    private AudioManager audioManager;                                      // The Audio Manager used to play music
    private AudioData.AudioEntry menuTrack;                                 // The menu music track
    private AudioSource gameplaySource;                                     // The currently playing gameplay track
    private MusicMode mode;                                                 // The current music mode
    private int nextTrack;                                                  // Index of the next gameplay track
    private AudioClip lastClip;                                             // The last gameplay clip that played

    private void Start()
    {
        audioManager = GetComponent<AudioManager>();                        // Get the Audio Manager on this object
        if (audioManager != AudioManager.Instance)                          // Check that this is the active Audio Manager
            return;                                                         // Exit if another Audio Manager is active

        menuTrack = FindTrack(MenuTrack);                                   // Find the menu music in the audio data
        foreach (string trackName in gameplayTracks)                        // Find each gameplay track
        {
            AudioData.AudioEntry track = FindTrack(trackName);                      // Look up the track by name
            if (track != null && track.audioClip != menuTrack?.audioClip &&         // Check the track exists and isn't menu music
                !playlist.Exists(entry => entry.audioClip == track.audioClip))      // Check it isn't already in the playlist
                playlist.Add(track);                                                // Add the track to the gameplay playlist
        }

        if (menuTrack == null || playlist.Count != 3)                                                               // Check that all required music tracks were found
            Debug.LogWarning("Music needs MenuMusic and three distinct gameplay tracks in BGM Data.", this);        // Show a setup warning

        nextTrack = playlist.Count;                                         // Start by creating a shuffled playlist
        SceneManager.activeSceneChanged += OnSceneChanged;                  // Listen for scene changes
        ApplyScene(SceneManager.GetActiveScene());                          // Set music for the current scene
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;                  // Stop listening for scene changes
    }

    private AudioData.AudioEntry FindTrack(string trackName)
    {
        if (bgmData == null || string.IsNullOrWhiteSpace(trackName))        // Check that audio data and a track name were provided
            return null;                                                    // Return nothing if either is missing

        return bgmData.audioList.FindLast(entry => entry.nameAudio == trackName && entry.audioClip != null);        // Find the last matching track with a clip
    }

    private void OnSceneChanged(Scene previous, Scene current)
    {
        ApplyScene(current);                                                // Update the music for the new scene
    }

    private void ApplyScene(Scene scene)
    {
        MusicMode nextMode = scene.name switch                              // Choose the music mode based on the scene name
        {
            "MENU" or "SETTINGS" or "CREDITS" or "HOW TO PLAY" => MusicMode.Menu,           // Play menu music in menu scenes
            "LEVEL ONE" => MusicMode.Gameplay,                                              // Play gameplay music in the level
            _ => MusicMode.Silent                                                           // Stop music in other scenes
        };

        if (nextMode == mode)                                               // Check if the music mode is already correct
            return;                                                         // Skip changing the music

        mode = nextMode;                                                    // Save the new music mode
        gameplaySource = null;                                              // Clear the current gameplay track reference

        switch (mode)                                                       // Start the music for the new mode
        {
            case MusicMode.Menu:
                if (menuTrack == null)                                      // Check that the menu track was found
                    break;                                                  // Skip playing music if it wasn't found

                AudioSource menuSource = PlayTrack(menuTrack, true);        // Play the menu track on a loop
                if (menuSource != null)                                     // Check that the track started playing
                    menuSource.timeSamples = 0;                             // Start the menu track from the beginning
                break;                                                      // Finish handling menu music

            case MusicMode.Gameplay:
                nextTrack = playlist.Count;                                 // Start with a fresh shuffled playlist
                lastClip = null;                                            // Clear the previous gameplay track
                PlayNextTrack();                                            // Start the first gameplay track
                break;                                                      // Finish handling gameplay music

            default:
                audioManager.StopBGM();                                     // Stop background music in silent scenes
                break;                                                      // Finish handling silent mode
        }
    }

    private void Update()
    {
        if (gameplaySource != null && !AudioListener.pause && !gameplaySource.isPlaying)            // Check if gameplay music has finished
            PlayNextTrack();                                                                        // Play the next track
    }

    private void PlayNextTrack()
    {
        if (playlist.Count == 0)                                            // Check if any gameplay tracks were found
        {
            audioManager.StopBGM();                                         // Stop music if the playlist is empty
            return;                                                         // Exit without trying to play a track
        }

        if (nextTrack == playlist.Count)                                    // Check if the playlist needs to be shuffled again
        {
                                                                            // Shuffle the tracks and avoid repeating the last track.
            for (int i = playlist.Count - 1; i > 0; i--)                    // Go through the playlist backwards
                Swap(i, Random.Range(0, i + 1));                            // Swap each track with a random track

            if (playlist.Count > 1 && playlist[0].audioClip == lastClip)            // Check if the first track repeats the last clip
                Swap(0, Random.Range(1, playlist.Count));                           // Swap it with another track

            nextTrack = 0;                                                  // Reset the playlist index
        }

        AudioData.AudioEntry track = playlist[nextTrack++];                 // Get the next track and advance the index
        lastClip = track.audioClip;                                         // Save this clip to avoid repeating it
        gameplaySource = PlayTrack(track, false);                           // Play the track without looping
    }

    private void Swap(int first, int second)
    {
        (playlist[first], playlist[second]) = (playlist[second], playlist[first]);      // Swap the two playlist entries
    }

    private AudioSource PlayTrack(AudioData.AudioEntry track, bool loop)
    {
        audioManager.PlayBGM(track.nameAudio);                              // Ask the Audio Manager to play this track

        foreach (AudioSource source in audioManager.GetComponentsInChildren<AudioSource>())         // Find the Audio Source playing it
        {
            if (source.clip == track.audioClip)                             // Check if this source has the requested clip
            {
                source.loop = loop;                                         // Set whether the track should loop
                return source;                                              // Return the source that is playing the track
            }
        }

        Debug.LogWarning($"Music could not play '{track.nameAudio}'. Check the BGM Data assigned to DZ Audio Manager.", this);      // Show a warning if the track wasn't found
        return null;                                                                                                                // Return nothing if no matching source was found
    }
}