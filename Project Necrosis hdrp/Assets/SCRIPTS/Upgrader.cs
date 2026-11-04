using UnityEngine;
using System.Diagnostics;
using Microsoft.Win32;


public class Upgrader : MonoBehaviour
{
    public WeaponSystem weaponSystem;
    public Power power;
    public PointsManager pointsManager;
    public Pistol pistol;
    public Rifle rifle;
    public Shotgun shotgun;
    public UIManager uiManager;


    private bool PlayerNearby = false;
    private int Cost = 500;
    private bool PistolUpgraded = false;
    private bool RifleUpgraded = false;
    private bool ShotgunUpgraded = false;

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F) && PlayerNearby)
        {
            TryUpgrade();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (!power.PowerOn)
            return;

        PlayerNearby = true;

        uiManager.SetInteractText("Press F upgrade weapon cost: 500");


    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerNearby = false;

        uiManager.ClearInteractText();
    }

    void TryUpgrade()
    {
        if (power.PowerOn && pointsManager.Points >= Cost)
        {
            

            if (weaponSystem.Equipped == 0 && !PistolUpgraded)
            {
                pistol.Upgrader();
                PistolUpgraded = true;
                pointsManager.RemovePoints(Cost);
            }

            else if (weaponSystem.Equipped == 1 && !RifleUpgraded)
            {
                rifle.Upgrader();
                RifleUpgraded = true;
                pointsManager.RemovePoints(Cost);
            }

            else if (weaponSystem.Equipped == 2 && !ShotgunUpgraded) 
            {
                shotgun.Upgrader();
                ShotgunUpgraded = true;
                pointsManager.RemovePoints(Cost);
            }

            
        }
    }
}
