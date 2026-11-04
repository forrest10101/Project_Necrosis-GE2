using System.Diagnostics;
using UnityEngine;

public class Power : MonoBehaviour
{
    public static Power Instance;

    public bool PowerOn = false;
    private bool PlayerNearby = false;
    private int Fuses = 0;

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


  

    private void Update()
    {
        if (Fuses == 2)
        {
            if (Input.GetKeyDown(KeyCode.F) && PlayerNearby && !PowerOn)
            {
                TurnPowerOn();
                //uiManager.ClearInteractText();

            }
        }
    }


    public void TurnPowerOn()
    {
        
        PowerOn = true;
        uiManager.SetObjective("Survive");
        
        
    }

    public void FuseCollect()
    { 
        Fuses ++;

        if (Fuses == 2)
        {
            uiManager.SetObjective("Turn on power");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (PowerOn)
            return;

        PlayerNearby = true;

        uiManager.SetInteractText("Press F To Restore Power");

        
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerNearby = false;

        uiManager.ClearInteractText();
    }

}

