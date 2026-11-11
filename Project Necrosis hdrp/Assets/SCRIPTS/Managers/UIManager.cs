using UnityEngine;
using TMPro;
using UnityEngine.UI;

//UIManager, Written by Andrew Forrest
public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI interactText;
    public TextMeshProUGUI pointText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI weaponText;

    private string objectiveTemplate;

    private void Awake()
    {
        if (objectiveText != null)
            objectiveTemplate = objectiveText.text;
    }

    void Start()
    {
        SetObjective("Find both fuses");
        ClearInteractText();
        
    }

    public void UpdatePoints(int Point)
    {
        if (pointText != null)
        {
            pointText.text = ("$ " + Point);
        }
    }

    public void SetObjective(string text)
    {
        if (objectiveText != null)
            objectiveText.text = objectiveTemplate.Replace("{objective text}", text);
    }

    public void SetInteractText(string text)
    {
        if (interactText != null)
            interactText.text = text;
    }

    public void ClearInteractText()
    {
        if (interactText != null)
            interactText.text = "";
    }

    public void SetRoundText(int Round)
    {
        if (roundText != null)
            roundText.text = ("ROUND " + Round);
    }

    public void WeaponDisplay(string Weapon)
    {
        if (weaponText != null)
            weaponText.text = (Weapon);
    }

    public void SetAmmoText(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null)
            ammoText.text = (currentAmmo + " / " + maxAmmo);
    }


}
