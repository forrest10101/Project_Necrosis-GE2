using UnityEngine;
using TMPro;
using System.Diagnostics;


//PointsManager, Written by Andrew Forrest

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

        uiManager.UpdatePoints(Points);
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
