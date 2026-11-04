using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI interactText;
    public TextMeshProUGUI pointText;


    void Start()
    {
        SetObjective("Find both fuses");
        ClearInteractText();
        UpdatePoints(0);
    }

    public void UpdatePoints(int Point)
    {
        if (pointText != null)
        {
            pointText.text = ("Points: " + Point);
        }
    }

    public void SetObjective(string text)
    {
        if (objectiveText != null)
            objectiveText.text = text;
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
}
