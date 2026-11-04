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
        CurrentHealth = MaxHealth;
    }

    void Update()
    {
 
    }



    public void EnemyDamage(float Damage)
    {


        if (Time.time >= NextDamage)
        {
            CurrentHealth -= Damage;
            NextDamage = Time.time + Iframe;
            


            if (CurrentHealth <= 0)
            {
                
                PointsManager.Instance.AddPoints(PointWorth);
                Destroy(gameObject);
            }
        }




    }

    

    
}
