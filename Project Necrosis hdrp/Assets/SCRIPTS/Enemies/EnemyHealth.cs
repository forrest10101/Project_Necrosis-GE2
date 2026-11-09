using UnityEngine;
using System.Diagnostics;



//EnemyHealth, Written by Andrew Forrest
public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth = 20f;
    private float CurrentHealth;
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

        CurrentHealth -= Damage;
           
        if (CurrentHealth <= 0)
        {
           RoundManager.Instance.ZombieDied();
           PointsManager.Instance.AddPoints(PointWorth);
           Destroy(gameObject);
        }
        




    }

    

    
}
