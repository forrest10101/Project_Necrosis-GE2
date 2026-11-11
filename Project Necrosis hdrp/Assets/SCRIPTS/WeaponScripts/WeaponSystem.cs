using UnityEngine;



//WeaponSystem, Written by Andrew Forrest
public class WeaponSystem : MonoBehaviour
{
    public static WeaponSystem Instance;
    public bool[] Unlocked = { true, false, false };
    public GameObject[] Weapons = new GameObject[3];
    public int Equipped = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EquipWeapon(Equipped);
    }

    public void WeaponUnlock(int weapon)
    {
        Unlocked[weapon] = true;
        EquipWeapon(weapon);

    }

    void EquipWeapon(int weapon)
    {
        foreach (GameObject gun in Weapons)
        {
            gun.SetActive(false);
        }

        Weapons[weapon].SetActive(true);
        Equipped = weapon;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && Unlocked[0])
            EquipWeapon(0);

        if (Input.GetKeyDown(KeyCode.Alpha2) && Unlocked[1])
            EquipWeapon(1);

        if (Input.GetKeyDown(KeyCode.Alpha3) && Unlocked[2])
            EquipWeapon(2);
    }



}
