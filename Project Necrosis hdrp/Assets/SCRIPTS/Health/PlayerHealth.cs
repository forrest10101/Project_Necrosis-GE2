using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float MaxHealth = 20f;
    private float CurrentHealth;
    private float Iframe = 0.2f;
    private float NextDamage = 0f;
    private bool GodMode = false;
    public Slider healthBar;
    private float NextRegen = 0f;
    private float RegenCD = 1f;
    private float RegenRate = 1f;
    private float CombatCD = 5f;
    private float CombatLock = 0f;


    private void Awake()
    {
        CurrentHealth = MaxHealth;
        HealthBar();
    }

    void Update()
    {
        Regeneration();

        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayerDamage(1);
        }


        if (Input.GetKeyDown(KeyCode.G))
        {
            GodMode = !GodMode;
            UnityEngine.Debug.Log("god");
        }


    }



    public void PlayerDamage(float Damage)
    {


        if (Time.time >= NextDamage && GodMode == false)
        {
            CurrentHealth -= Damage;
            NextDamage = Time.time + Iframe;
            CombatLock = Time.time + CombatCD;
            HealthBar();

            if (CurrentHealth <= 0)
            {
                fail();
            }
        }




    }

    void fail()
    {
        SceneManager.LoadScene("GAMEOVER");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void HealthBar()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = MaxHealth;
            healthBar.value = CurrentHealth;
        }
    }

    void Regeneration()
    {


        if (Time.time >= NextRegen && CurrentHealth < MaxHealth && Time.time >= CombatLock)
        {
            NextRegen = Time.time + RegenCD;
            CurrentHealth += RegenRate;

            CurrentHealth = Mathf.Clamp(
                CurrentHealth,
                0,
                MaxHealth);

            HealthBar();
        }
    }
    public void ApplyHealingSpeed(float multiplier)
    {
        RegenCD /= Mathf.Max(1f, multiplier);
        NextRegen = Mathf.Min(NextRegen, Time.time + RegenCD);
    }

    public void AddMaximumHealth(float amount)
    {
        amount = Mathf.Max(0f, amount);
        MaxHealth += amount;
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        HealthBar();
    }
}