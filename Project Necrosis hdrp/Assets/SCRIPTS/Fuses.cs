using Microsoft.Win32;
using UnityEngine;

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
