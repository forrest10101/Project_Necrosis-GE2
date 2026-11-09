using Microsoft.Win32;
using UnityEngine;

//Fuses, Written by Andrew Forrest
public class Fuses : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;


        Power.Instance.FuseCollect();

        gameObject.SetActive(false);

    }

}
