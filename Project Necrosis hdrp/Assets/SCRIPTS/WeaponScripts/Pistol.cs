using System.Collections;
using System.Diagnostics;
using System.Threading;
using UnityEngine;


//Pistol, Written by Andrew Forrest
public class Pistol : MonoBehaviour
{
    public static Pistol Instance;

    private int CurrentAmmo = 12;
    private int MaxAmmo = 12;
    private int Ammo = 999;
    public float FireRate = 1.5f;
    private float FireTime = 0f;
    private float Damage = 2.5f;
    public GameObject Bullet;
    public Transform BulletPoint;
    private bool Reloading = false;
    public UIManager uiManager;

    AudioSource Fire;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        uiManager.WeaponDisplay("Pistol");
        uiManager.SetAmmoText(CurrentAmmo, MaxAmmo);
    }


    void Start()
    {
        Fire = GetComponent<AudioSource>();
    }


    void Update()
    {
        Shooting();
        Reloader();
    }



    void Shooting()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= FireTime && CurrentAmmo > 0)
        {

            GameObject spawnedBullet = Instantiate(
                Bullet,
                BulletPoint.position,
                BulletPoint.rotation
            );

            BulletScript bulletScript =
            spawnedBullet.GetComponent<BulletScript>();

            if (bulletScript != null)
            {
                bulletScript.Damage = Damage * (PerkSystem.Instance != null ? PerkSystem.Instance.DamageMultiplier : 1f);
            }

            FireTime = Time.time + FireRate;
            //Fire.Play();
            CurrentAmmo -= 1;

            uiManager.SetAmmoText(CurrentAmmo, MaxAmmo);


        }
    }

    void Reloader()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!Reloading && Ammo > 0 && CurrentAmmo < MaxAmmo)
            {
                StartCoroutine(Reload());
            }
        }
    }

    IEnumerator Reload()
    {
        if (Reloading || Ammo <= 0)
            yield break;

        Reloading = true;

        //if (reloadSound != null)
        //   reloadSound.Play();

        yield return new WaitForSeconds(1.5f / (PerkSystem.Instance != null ? PerkSystem.Instance.ReloadSpeedMultiplier : 1f));

        int Used = MaxAmmo - CurrentAmmo;

        Ammo -= Used;
        CurrentAmmo = MaxAmmo;

        Reloading = false;

        uiManager.SetAmmoText(CurrentAmmo, MaxAmmo);
    }


    public void Upgrader()
    {
        Damage = Damage * 1.25f;
        MaxAmmo = MaxAmmo * 2;
        

        UnityEngine.Debug.Log("Upgraded- ammo" + MaxAmmo + " Damage: " + Damage);
    }
}
