using UnityEngine;
using System.Collections;

public class Shotgun : MonoBehaviour
{
    public static Shotgun Instance;

    private int CurrentAmmo = 12;
    private int MaxAmmo = 12;
    private int Ammo = 999;
    public float FireRate = 1.5f;
    private float FireTime = 0f;
    private float Damage = 5f;
    private bool Upgraded = false;
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
        uiManager.WeaponDisplay("Shotgun");
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
            Vector3 rotation = BulletPoint.eulerAngles;

            for (int i = 0; i < 5; i++)
            {
                float RotationX = Random.Range(-7.5f, 7.5f);
                float RotationY = Random.Range(-7.5f, 7.5f);
                float RotationZ = Random.Range(-7.5f, 7.5f);


                GameObject spawnedBullet = Instantiate(
                    Bullet,
                    BulletPoint.position,
                    Quaternion.Euler(
                        rotation.x + RotationX,
                        rotation.y + RotationY,
                        rotation.z + RotationZ)
                );

                BulletScript bulletScript =
                spawnedBullet.GetComponent<BulletScript>();

                if (bulletScript != null)
                {
                    bulletScript.Damage = Damage * (PerkSystem.Instance != null ? PerkSystem.Instance.DamageMultiplier : 1f);
                }

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
        Damage += 1;
        MaxAmmo += 24;
        Upgraded = true;

        UnityEngine.Debug.Log("Upgraded- ammo" + MaxAmmo + " Damage: " + Damage);
    }
}