using DZAssets.AudioManager;
using UnityEngine;

public class WeaponAudio : MonoBehaviour
{
    [SerializeField] private string[] shotSounds = new string[3];   // Shot sound names
    [SerializeField] private string equipSound;                     // Equip sound name
    [SerializeField] private string reloadSound;                    // Reload sound name

    private int nextShot;                                           // Tracks which shot sound plays next

    public void PlayShot()
    {
        if (shotSounds == null || shotSounds.Length == 0)           // Check that shot sounds are assigned
            return;                                                 // Exit if there are no shot sounds

        PlaySound(shotSounds[nextShot]);                            // Play the next shot sound
        nextShot = (nextShot + 1) % shotSounds.Length;              // Move to the next sound and loop back
    }

    public void PlayEquip()
    {
        PlaySound(equipSound);                                      // Play the equip sound
    }

    public void PlayReload()
    {
        PlaySound(reloadSound);                                     // Play the reload sound
    }

    private void PlaySound(string soundName)
    {
        if (!string.IsNullOrWhiteSpace(soundName))                  // Check that a sound name was entered
            AudioManager.Instance.PlaySFX(soundName);               // Play the sound through the Audio Manager
    }
}