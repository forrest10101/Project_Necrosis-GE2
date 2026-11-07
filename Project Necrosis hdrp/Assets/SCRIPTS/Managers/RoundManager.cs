
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

    [Header("Spawners")]
    public EnemySpawner[] spawners;

    [Header("Round Settings")]
    public int currentRound = 1;
    public int baseZombies = 5;
    public float zombiesPerRoundMultiplier = 1.4f;
    public UIManager uiManager;

    [Header("Zombie Caps")]
    public int maxZombiesPerRound = 100;
    public int maxZombiesInScene = 50;

    [Header("Spawner Selection")]
    public int maxActiveSpawners = 3;

    [Header("Round Break")]
    public float timeBetweenRounds = 5f;

    private int zombiesAlive;
    private int zombiesToSpawnThisRound;
    private int zombiesSpawnedThisRound;
    


    private bool roundEnding = false;


    private void Start()
    {
        Instance = this;
        StartRound();
    }

    private void StartRound()
    {
        roundEnding = false;

        uiManager.SetRoundText(currentRound);

        zombiesToSpawnThisRound = Mathf.RoundToInt(
            baseZombies * Mathf.Pow(zombiesPerRoundMultiplier, currentRound - 1)
        );

        zombiesToSpawnThisRound = Mathf.Min(
            zombiesToSpawnThisRound,
            maxZombiesPerRound
        );

        zombiesAlive = 0;
        zombiesSpawnedThisRound = 0;

        EnemySpawner[] activeSpawners = GetClosestUnlockedSpawners(maxActiveSpawners);

        if (activeSpawners.Length == 0)
        {
            UnityEngine.Debug.Log("No unlocked spawners found. Unlock at least one starting spawner.");
            return;
        }

        foreach (EnemySpawner spawner in activeSpawners)
        {
            int amount = Mathf.CeilToInt(
                (float)zombiesToSpawnThisRound / activeSpawners.Length
            );

            spawner.SetRoundStats(
                currentRound,
                amount
            );
        }

        UnityEngine.Debug.Log("Round " + currentRound + " started with " + activeSpawners.Length + " spawners.");
    }

    public bool CanSpawnZombie()
    {
        if (roundEnding)
            return false;

        if (zombiesAlive < maxZombiesInScene &&
            zombiesSpawnedThisRound < zombiesToSpawnThisRound)
        {
            zombiesAlive++;
            zombiesSpawnedThisRound++;
            return true;
        }

        return false;
    }

    public void ZombieDied()
    {
        
        if (roundEnding)
            return;

        zombiesAlive--;

        if (zombiesAlive < 0)
        {
            zombiesAlive = 0;
        }

        
        CheckRoundComplete();
    }

    private void CheckRoundComplete()
    {
        
        bool allZombiesSpawned =
            zombiesSpawnedThisRound >= zombiesToSpawnThisRound;

        bool allZombiesDead =
            zombiesAlive <= 0;

        
        if (allZombiesSpawned && allZombiesDead)
        {
            StartCoroutine(EndRoundDelay());
        }

    }

    private IEnumerator EndRoundDelay()
    {
        UnityEngine.Debug.Log("delay ran");
        if (roundEnding)
            yield break;

        roundEnding = true;

        UnityEngine.Debug.Log("Round complete. Next round in " + timeBetweenRounds + " seconds.");

        yield return new WaitForSeconds(timeBetweenRounds);

        currentRound++;

        StartRound();
    }

    private EnemySpawner[] GetClosestUnlockedSpawners(int amount)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        List<EnemySpawner> unlockedSpawners = new List<EnemySpawner>();

        foreach (EnemySpawner spawner in spawners)
        {
            if (spawner != null && spawner.CanSpawn())
            {
                unlockedSpawners.Add(spawner);
            }
        }

        if (player != null)
        {
            unlockedSpawners.Sort((a, b) =>
            {
                float distanceA = Vector3.Distance(
                    a.transform.position,
                    player.transform.position
                );

                float distanceB = Vector3.Distance(
                    b.transform.position,
                    player.transform.position
                );

                return distanceA.CompareTo(distanceB);
            });
        }

        int count = Mathf.Min(amount, unlockedSpawners.Count);

        EnemySpawner[] result = new EnemySpawner[count];

        for (int i = 0; i < count; i++)
        {
            result[i] = unlockedSpawners[i];
        }

        return result;
    }
}