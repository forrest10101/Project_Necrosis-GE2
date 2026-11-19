using DZAssets.AudioManager;
using UnityEngine;

public class WeaponAudio : MonoBehaviour
{
    [SerializeField] private string[] shotSounds = new string[3]; // Shot sound names
    [SerializeField] private string equipSound;                  // Equip sound name
    [SerializeField] private string reloadSound;                 // Reload sound name

    private int nextShot;

    public void PlayShot()
    {
        if (shotSounds == null || shotSounds.Length == 0)
            return;

        PlaySound(shotSounds[nextShot]);
        nextShot = (nextShot + 1) % shotSounds.Length;           // Cycle through the shots
    }

    public void PlayEquip()
    {
        PlaySound(equipSound);
    }

    public void PlayReload()
    {
        PlaySound(reloadSound);
    }

    private void PlaySound(string soundName)
    {
        if (!string.IsNullOrWhiteSpace(soundName))
            AudioManager.Instance.PlaySFX(soundName);
    }
}