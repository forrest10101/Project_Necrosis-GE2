using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class SceneFade : MonoBehaviour
{
    // SCENE FADE, BY EVAN MONTGOMERY
    public static SceneFade Instance { get; private set; }                              // Current scene fade

    [SerializeField, Min(0f)] private float fadeDuration = 0.35f;                       // Fade duration

    private CanvasGroup overlay;                                                        // Screen overlay
    private bool busy;                                                                  // If changing rooms

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);                                                // Disable duplicate
            Destroy(gameObject);                                                        // Remove duplicate
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);                                                  // Keep when changing scenes

        overlay = GetComponent<CanvasGroup>();                                          // Get canvas group
        overlay.alpha = 1f;                                                             // Start black
        overlay.blocksRaycasts = true;                                                  // Stop clicks during fade
        busy = true;                                                                    // Wait for starting fade
    }

    private IEnumerator Start()
    {
        yield return null;                                                              // Wait one frame
        yield return FadeTo(0f);                                                        // Fade into game

        overlay.blocksRaycasts = false;                                                 // Allow button clicks
        busy = false;                                                                   // Finished starting fade
    }

    public void LoadRoom(string sceneName, bool useTransition)
    {
        if (busy) return;                                                               // Cancel if already loading

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Cannot load scene '{sceneName}'.", this);                  // Debug message for console
            return;
        }

        StartCoroutine(ChangeRoom(sceneName, useTransition));                           // Start room change
    }

    private IEnumerator ChangeRoom(string sceneName, bool useTransition)
    {
        busy = true;                                                                    // Changing rooms
        overlay.blocksRaycasts = true;                                                  // Stop extra button clicks

        if (useTransition)
            yield return FadeTo(1f);                                                    // Fade to black

        AsyncOperation loading = SceneManager.LoadSceneAsync(sceneName);                // Load next scene
        yield return loading;                                                           // Wait until loaded

        yield return null;                                                              // Wait one frame

        if (useTransition)
            yield return FadeTo(0f);                                                    // Fade back in

        overlay.alpha = 0f;                                                             // Make overlay invisible
        overlay.blocksRaycasts = false;                                                 // Allow button clicks
        busy = false;                                                                   // Finished changing rooms
    }

    private IEnumerator FadeTo(float target)
    {
        float start = overlay.alpha;                                                    // Starting opacity

        if (fadeDuration <= 0f)
        {
            overlay.alpha = target;                                                     // Change instantly if duration is zero
            yield break;
        }

        float elapsed = 0f;                                                             // Time passed

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;                                          // Count time, even when paused
            float progress = Mathf.Clamp01(elapsed / fadeDuration);                     // Calculate fade progress

            overlay.alpha = Mathf.Lerp(start, target, progress);                        // Change opacity
            yield return null;                                                          // Wait until next frame
        }

        overlay.alpha = target;                                                         // Finish at target opacity
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;                                                            // Clear reference when removed
    }
}