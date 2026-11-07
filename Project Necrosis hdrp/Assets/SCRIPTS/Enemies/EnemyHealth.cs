using UnityEngine;
using System.Diagnostics;

public class EnemyHealth : MonoBehaviour
{
    private float MaxHealth = 20f;
    private float CurrentHealth;
    private float Iframe = 1f;
    private float NextDamage = 0f;
    private int PointWorth = 5;

    

    private void Awake()
    {
        if (CompareTag("NormZombie"))
        {
            MaxHealth = 8;
            PointWorth = 25;
        }

        else if (CompareTag("UpgradeZombie"))
        {
            MaxHealth = 15;
            PointWorth = 50;
        }

        else if (CompareTag("TankZombie"))
        {
            MaxHealth = 35;
            PointWorth = 150;
        }

        CurrentHealth = MaxHealth;

        
    }

    


    public void EnemyDamage(float Damage)
    {


        if (Time.time >= NextDamage)
        {
            CurrentHealth -= Damage;
            NextDamage = Time.time + Iframe;
            


            if (CurrentHealth <= 0)
            {
                RoundManager.Instance.ZombieDied();
                PointsManager.Instance.AddPoints(PointWorth);
                Destroy(gameObject);
            }
        }




    }

    

    
}
