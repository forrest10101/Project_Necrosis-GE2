using UnityEngine;
using TMPro;


public class PointsManager : MonoBehaviour
{
    public int Points = 0;
    public TextMeshProUGUI PointsText;

    void awake()
    {
        PointsText.text = Points.ToString();
    }

    public void AddPoints(int amount)
    {
        Points += amount;

        PointsText.text = Points.ToString();
    }

    public void RemovePoints(int amount)
    {
        Points -= amount;

        if (Points < 0)
        {
            Points = 0;

        }

        PointsText.text = Points.ToString();
    }
    
}
