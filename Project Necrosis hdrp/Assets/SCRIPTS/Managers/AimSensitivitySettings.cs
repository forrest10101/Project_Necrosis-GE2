using ElmanGameDevTools.PlayerSystem;
using UnityEngine;
using UnityEngine.UI;

// PLAYER SENSITIVITY, BY EVAN MONTGOMERY
public class AimSensitivitySettings : MonoBehaviour
{
    private const string SensitivityKey = "AimSensitivity";                                 // Save key
    private const float DefaultSensitivity = 2f;                                            // Default sensitivity

    [SerializeField] private Slider slider;                                                 // Sensitivity slider
    [SerializeField] private PlayerController playerController;                             // Player controller

    private bool hasChanges;                                                                // Tracks unsaved changes

    private void OnEnable()
    {
        if (playerController != null)
        {
            playerController.sensitivity =
                PlayerPrefs.GetFloat(SensitivityKey, playerController.sensitivity);         // Apply saved sensitivity
        }

        if (slider != null)
        {
            slider.SetValueWithoutNotify(
                PlayerPrefs.GetFloat(SensitivityKey, DefaultSensitivity));                  // Set slider without firing its event

            slider.onValueChanged.AddListener(SetSensitivity);                              // Listen for slider changes
        }
    }

    private void OnDisable()
    {
        if (slider == null)
            return;                                                                         // Skip on the player prefab

        slider.onValueChanged.RemoveListener(SetSensitivity);                               // Stop listening for changes

        if (hasChanges)
        {
            PlayerPrefs.Save();                                                             // Save for the next launch
            hasChanges = false;                                                             // Clear the unsaved flag
        }
    }

    private void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat(SensitivityKey, value);                                        // Store the slider value
        hasChanges = true;                                                                  // Mark as unsaved
    }
}