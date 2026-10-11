using UnityEngine;
using UnityEngine.SceneManagement;


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
        UnityEngine.Debug.Log("col");
        EnemyHealth health = collision.gameObject.GetComponent<EnemyHealth>();



        if (health != null)
        {
           // health.TakeDamage(Damage);
            
        }
        Destroy(gameObject);
    }
}
