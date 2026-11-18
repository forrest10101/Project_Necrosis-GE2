using UnityEngine;
using System.Collections;

// WeaponSystem, Written by Andrew Forrest
public class WeaponSystem : MonoBehaviour
{
    public static WeaponSystem Instance;
    public bool[] Unlocked = { true, false, false };
    public GameObject[] Weapons = new GameObject[3];
    public int Equipped = 0;

    public bool IsSwitching { get; private set; }               // Blocks firing during the swap

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EquipWeapon(Equipped);                                // Show the starting gun
    }

    public void WeaponUnlock(int weapon)
    {
        Unlocked[weapon] = true;
        RequestSwap(weapon);                                  // Equip the newly unlocked gun
    }

    private void RequestSwap(int weapon)
    {
        if (weapon == Equipped || IsSwitching)
            return;

        StartCoroutine(SwapWeapon(weapon));
    }

    private IEnumerator SwapWeapon(int weapon)
    {
        IsSwitching = true;

        WeaponSway outgoing = Weapons[Equipped].GetComponent<WeaponSway>();
        if (outgoing != null)
            yield return outgoing.Slide(true);                 // Lower the current gun

        EquipWeapon(weapon);

        WeaponSway incoming = Weapons[Equipped].GetComponent<WeaponSway>();
        if (incoming != null)
            yield return incoming.Slide(false);                // Raise the new gun

        IsSwitching = false;
    }

    private void EquipWeapon(int weapon)
    {
        foreach (GameObject gun in Weapons)
        {
            gun.SetActive(false);
        }

        Equipped = weapon;
        Weapons[Equipped].SetActive(true);
        Weapons[Equipped].GetComponent<WeaponAudio>()?.PlayEquip();
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1) && Unlocked[0])
            RequestSwap(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2) && Unlocked[1])
            RequestSwap(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3) && Unlocked[2])
            RequestSwap(2);
    }
}