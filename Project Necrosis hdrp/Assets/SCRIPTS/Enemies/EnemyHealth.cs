using UnityEngine;
using System.Diagnostics;



//EnemyHealth, Written by Andrew Forrest
public class EnemyHealth : MonoBehaviour
{
    public float MaxHealth = 20f;
    private float CurrentHealth;
    private int PointWorth = 5;

    private bool isDead;

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
        if (isDead)
            return;

        CurrentHealth -= Damage;

        if (CurrentHealth > 0f)
            return;

        isDead = true;

        RoundManager.Instance.ZombieDied();
        PointsManager.Instance.AddPoints(PointWorth);

        Animator animator = GetComponentInChildren<Animator>();

        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Destroy(gameObject);
            return;
        }

        if (TryGetComponent(out ZombieAI zombieAI))
            zombieAI.enabled = false;

        if (TryGetComponent(out UnityEngine.AI.NavMeshAgent agent))
            agent.enabled = false;

        if (TryGetComponent(out Collider hitCollider))
            hitCollider.enabled = false;

        animator.SetTrigger("Die");
        StartCoroutine(DespawnBody(animator.transform));
    }

    private System.Collections.IEnumerator DespawnBody(Transform model)
    {
        // Let the fall finish, then leave the body on the ground.
        yield return new WaitForSeconds(1.3f + 3f);

        const float sinkDuration = 0.6f;
        const float sinkDistance = 0.8f;

        Vector3 start = model.position;
        Vector3 end = start + Vector3.down * sinkDistance;
        float elapsed = 0f;

        while (elapsed < sinkDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, elapsed / sinkDuration);
            model.position = Vector3.Lerp(start, end, progress);
            yield return null;
        }

        Destroy(gameObject);
    }
}
