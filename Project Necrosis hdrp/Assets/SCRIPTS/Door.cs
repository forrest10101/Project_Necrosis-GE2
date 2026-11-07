using UnityEngine;
using TMPro;
using System.Collections;

public class DoorPurchase : MonoBehaviour
{
    [Header("Door")]
    public GameObject doorObject;
    public int cost = 250;
    public float moveHeight = 10f;
    public float moveSpeed = 3f;

    [Header("Spawner Unlocks")]
    public EnemySpawner[] spawnersToUnlock;

    [Header("Managers")]
    public UIManager uiManager;
    public PointsManager pointsManager;


    private bool PlayerNearby;
    private bool opening;
    private bool opened;

    

    private Vector3 targetPosition;

    private void Start()
    {
        if (doorObject != null)
        {
            targetPosition =
                doorObject.transform.position +
                Vector3.up * moveHeight;
        }

        
    }

    private void Update()
    {
        if (!PlayerNearby || opening || opened)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            BuyDoor();
        }
    }

    private void BuyDoor()
    {
        

        if (pointsManager.Points < cost)
        {
            uiManager.SetInteractText("Not enough points");

            return;
        }

        pointsManager.RemovePoints(cost);

        StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor()
    {
        opening = true;

        uiManager.ClearInteractText();

        UnlockSpawners();

        while (doorObject != null &&
               Vector3.Distance(doorObject.transform.position, targetPosition) > 0.05f)
        {
            doorObject.transform.position = Vector3.MoveTowards(
                doorObject.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        if (doorObject != null)
        {
            Destroy(doorObject);
        }

        opened = true;
        opening = false;

        Destroy(gameObject);
    }

    private void UnlockSpawners()
    {
        foreach (EnemySpawner spawner in spawnersToUnlock)
        {
            if (spawner != null)
            {
                spawner.SetAreaUnlocked(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(opened || opening)
            return;

        if (!other.CompareTag("Player"))
            return;

        

        PlayerNearby = true;

        uiManager.SetInteractText("Press F To Open Door [" + cost + "]");


    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerNearby = false;

        uiManager.ClearInteractText();
    }
}
   