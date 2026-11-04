using UnityEngine;
using TMPro;
using System.Diagnostics;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance;

    public int Points = 0;
    public UIManager uiManager;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPoints(int amount)
    {
        Points += amount;


        uiManager.UpdatePoints(Points);

    }

    public void RemovePoints(int amount)
    {
        Points -= amount;

        if (Points < 0)
        {
            Points = 0;

        }

        uiManager.UpdatePoints(Points);
    }
    
}
