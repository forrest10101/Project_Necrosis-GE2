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

    [SerializeField] private GameObject uiImage;

    AudioSource Fire;

    private WeaponSway weaponSway;
    private WeaponAudio weaponAudio;

    private void Awake()
    {
        Instance = this;
        weaponSway = GetComponent<WeaponSway>();
        weaponAudio = GetComponent<WeaponAudio>();
    }

    private void OnEnable()
    {
        uiManager.WeaponDisplay("Pistol");
        uiImage.SetActive(true);
        uiManager.SetAmmoText(CurrentAmmo, MaxAmmo);
    }
    private void OnDisable()
    {
        if (uiImage != null)
            uiImage.SetActive(false);

        Reloading = false;                                              // Cancel reload when holstered
    }

    void Start()
    {
        Fire = GetComponent<AudioSource>();
    }


    void Update()
    {
        if (Time.timeScale == 0f || Reloading ||
            (WeaponSystem.Instance != null && WeaponSystem.Instance.IsSwitching))
            return;

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

            weaponSway.PlayShot();
            weaponAudio?.PlayShot();       // Play once per shot

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

        float reloadTime = 1.5f / (PerkSystem.Instance != null
            ? PerkSystem.Instance.ReloadSpeedMultiplier : 1f);

        weaponSway.PlayReload(reloadTime);
        weaponAudio?.PlayReload();     // Play when reload starts

        yield return new WaitForSeconds(reloadTime);

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
