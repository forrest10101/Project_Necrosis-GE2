using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using Microsoft.Win32;

//BulletScript, Written by Andrew Forrest
public class BulletScript : MonoBehaviour
{
    private float speed = 20f;
    private float lifeTime = 1f;
    public float Damage = 1;



    void Start()
    {
        Destroy(gameObject, lifeTime);
        
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
            return;

        UnityEngine.Debug.Log("col");
        EnemyHealth health = collision.gameObject.GetComponent<EnemyHealth>();



        if (health != null)
        {
            health.EnemyDamage(Damage);
            
        }
        Destroy(gameObject);
    }
}
