using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplaySettingsButton : MonoBehaviour
{
    // DISPLAY SETTINGS, WRITTEN BY EVAN MONTGOMERY
    // This was sourced and altered as part of another project. It alters the resolution of the game using a button in the settings menu

    [SerializeField] private TMP_Text buttonText;                                   // Text showing the selected display preset

    private const string WidthKey = "DisplayWindowWidth";                           // Saved window width, with zero meaning fullscreen
    private const string HeightKey = "DisplayWindowHeight";                         // Saved window height

    private List<Vector2Int> presets;                                               // Available display presets
    private int selectedIndex;                                                      // Currently selected preset

    private static bool IsDesktop =>
        Application.isEditor
        || Application.platform == RuntimePlatform.WindowsPlayer
        || Application.platform == RuntimePlatform.OSXPlayer
        || Application.platform == RuntimePlatform.LinuxPlayer;                     // Only offer display presets on desktop

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplyOnStartup()
    {
        if (!IsDesktop || Application.isEditor)
        {
            return;                                                                 // Apply display changes only in desktop builds
        }

        List<Vector2Int> availablePresets = BuildPresets();
        int index = FindSavedPreset(availablePresets);                              // Find the saved choice or fall back to fullscreen

        ApplyPreset(availablePresets[index]);                                       // Restore settings without opening the settings scene
    }

    private void OnEnable()
    {
        if (!IsDesktop)
        {
            gameObject.SetActive(false);                                            // Hide this button on mobile
            return;
        }

        presets = BuildPresets();                                                   // Create presets that fit the current monitor
        selectedIndex = FindSavedPreset(presets);                                   // Restore the button's saved selection

        UpdateButtonText();                                                         // Display the current preset
    }

    public void CycleDisplay()
    {
        if (!IsDesktop || presets == null)
        {
            return;                                                                 // Ignore unavailable display controls
        }

        selectedIndex = (selectedIndex + 1) % presets.Count;                        // Advance and wrap back to fullscreen

        Vector2Int preset = presets[selectedIndex];

        PlayerPrefs.SetInt(WidthKey, preset.x);                                     // Store the selected width
        PlayerPrefs.SetInt(HeightKey, preset.y);                                    // Store the selected height
        PlayerPrefs.Save();                                                         // Keep the choice for the next launch

        ApplyPreset(preset);                                                        // Apply the new display mode
        UpdateButtonText();                                                         // Update the button label
    }

    private static Vector2Int GetNativeSize()
    {
        int width = Display.main.systemWidth;
        int height = Display.main.systemHeight;                                     // Read the display size independently of the game window

        if (width <= 0 || height <= 0)
        {
            Resolution resolution = Screen.currentResolution;
            width = resolution.width;
            height = resolution.height;                                             // Fall back to the current display resolution
        }

        return new Vector2Int(width, height);
    }

    private static List<Vector2Int> BuildPresets()
    {
        Vector2Int native = GetNativeSize();

        List<Vector2Int> result = new List<Vector2Int>
        {
            Vector2Int.zero                                                         // First preset represents native borderless fullscreen
        };

        Vector2Int[] windowSizes =
        {
            new Vector2Int(2560, 1440),
            new Vector2Int(1920, 1080),
            new Vector2Int(1600, 900),
            new Vector2Int(1280, 720),
            new Vector2Int(960, 540)
        };

        foreach (Vector2Int size in windowSizes)
        {
            if (size.x < native.x && size.y < native.y)
            {
                result.Add(size);                                                   // Only include windows smaller than the display
            }
        }

        return result;
    }

    private static int FindSavedPreset(List<Vector2Int> availablePresets)
    {
        Vector2Int saved = new Vector2Int(
            PlayerPrefs.GetInt(WidthKey, 0),
            PlayerPrefs.GetInt(HeightKey, 0));                                      // Default to native fullscreen on first launch

        int index = availablePresets.IndexOf(saved);

        return index >= 0 ? index : 0;                                              // Fall back safely if the monitor has changed
    }

    private static void ApplyPreset(Vector2Int preset)
    {
        if (Application.isEditor)
        {
            return;                                                                 // Leave the Unity Editor window unchanged
        }

        if (preset == Vector2Int.zero)
        {
            Vector2Int native = GetNativeSize();

            Screen.SetResolution(
                native.x,
                native.y,
                FullScreenMode.FullScreenWindow);                                   // Use native borderless fullscreen
        }
        else
        {
            Screen.SetResolution(
                preset.x,
                preset.y,
                FullScreenMode.Windowed);                                           // Use the selected window size
        }
    }

    private void UpdateButtonText()
    {
        Vector2Int preset = presets[selectedIndex];

        buttonText.text = preset == Vector2Int.zero
            ? "Fullscreen Window — Native"
            : $"Windowed — {preset.x} × {preset.y}";                                // Show the selected mode and resolution
    }
}