
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;



//EnemySpawner, Written by Andrew Forrest and Lyall McClue
public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject zombiePrefab;
    public GameObject UpgradePrefab;
    public GameObject TankPrefab;

    [Header("Area Unlock")]
    public bool areaUnlocked = false;

    [Header("Spawn Settings")]
    public Transform player;
    public float minDistanceFromPlayer = 5f;
    public float maxDistanceFromPlayer = 50f;

    private RoundManager roundManager;
    private Coroutine spawnRoutine;

    private int zombiesToSpawn;
    private int currentRound;

    private float healthMultiplier;



    private void Start()
    {
        roundManager = FindObjectOfType<RoundManager>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    public bool CanSpawn()
    {
        if (!areaUnlocked)
            return false;

        if (player == null)
            return true;

        float distance = Vector3.Distance(transform.position, player.position);

        return distance >= minDistanceFromPlayer &&
               distance <= maxDistanceFromPlayer;
    }

    public void SetAreaUnlocked(bool unlocked)
    {
        areaUnlocked = unlocked;
    }

    public void SetRoundStats(
        int round, 
        float hpMultiplier,
        int amount)
    {
        currentRound = round;
        zombiesToSpawn = amount;
        healthMultiplier = hpMultiplier;


        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
        }

        spawnRoutine = StartCoroutine(SpawnZombies());
    }

    private IEnumerator SpawnZombies()
    {
        for (int i = 0; i < zombiesToSpawn; i++)
        {
            while (roundManager == null || !roundManager.CanSpawnZombie())
            {
                yield return null;
            }

            GameObject enemyPrefab = ChooseEnemyPrefab();

            if (enemyPrefab == null)
            {
                UnityEngine.Debug.Log("No enemy prefab assigned on " + gameObject.name);
                yield break;
            }

            GameObject enemy = Instantiate(
                enemyPrefab,
                transform.position,
                Quaternion.identity
            );

            ApplyRoundScaling(enemy);

            yield return new WaitForSeconds(0.5f);
        }
    }

    private GameObject ChooseEnemyPrefab()
    {
        if (currentRound >= 3 && UpgradePrefab != null)
        {
            float UpgradeChance = Mathf.Clamp(currentRound * 8f, 0f, 75f);

            if (UnityEngine.Random.Range(0f, 100f) <= UpgradeChance)
            {
                return UpgradePrefab;
            }
        }

        return zombiePrefab;
    }

    private void ApplyRoundScaling(GameObject enemy)
    {
        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
       

        float healthScale = Mathf.Pow(healthMultiplier, currentRound - 1);
       

        if (health != null)
        {
            health.MaxHealth *= healthScale;
        }

        
    }


}