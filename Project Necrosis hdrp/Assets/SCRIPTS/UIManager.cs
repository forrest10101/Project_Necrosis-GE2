using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI interactText;

    void Start()
    {
        SetObjective("Find both fuses");
        ClearInteractText();
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
