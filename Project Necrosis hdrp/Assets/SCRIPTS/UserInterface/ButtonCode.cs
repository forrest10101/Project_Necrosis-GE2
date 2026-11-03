using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonCode : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    // BUTTON CODE, WRITTEN BY EVAN MONTGOMERY

    [Header("Text Animation")]
    [SerializeField] private RectTransform buttonText;                                                  // Button text
    [SerializeField] private float moveRight = 8f;                                                      // Move right
    [SerializeField] private float hoverScale = 1.04f;                                                  // Hover scale
    [SerializeField] private float animationSpeed = 14f;                                                // Animation speed

    [Header("Room Loading")]
    [SerializeField] private string nextScene;                                                          // Next scene

    [SerializeField] private bool useTransition = true;                                                 // Use transition (true/false)

    private Button button;                                                                              // Find button
    private Vector2 originalPosition;                                                                   // Original Position
    private Vector3 originalScale;                                                                      // Original Scale
    private float hoverAmount;                                                                          // Hover amount
    private bool hovered;                                                                               // If hovering

    private void Awake()
    {
        button = GetComponent<Button>();                                                                // Get button component

        if (buttonText != null)
        {
            originalPosition = buttonText.anchoredPosition;                                             // If button is there, start at original position
            originalScale = buttonText.localScale;                                                      // and original scale
        }
    }

    private void Update()
    {
        if (buttonText == null) return;                                                                 // Cancel if button doesn't have text

        float target = hovered && button.IsInteractable() ? 1f : 0f;                                    // Calculate target
        float blend = 1f - Mathf.Exp(-animationSpeed * Time.unscaledDeltaTime);                         // Calculate speed

        hoverAmount = Mathf.Lerp(hoverAmount, target, blend);                                           // Detect hover amount

        buttonText.anchoredPosition =
            originalPosition + Vector2.right * moveRight * hoverAmount;                                 // Calculate new position

        buttonText.localScale =
            originalScale * Mathf.Lerp(1f, hoverScale, hoverAmount);                                    // Calculate new scale
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;                                                                                 // Activate hover features if mouse is on button
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;                                                                                // Deactivate if mouse isn't on button
    }

    private void OnDisable()
    {
        hovered = false;                                                                                // Don't do anything if button is disabled
        hoverAmount = 0f;                                                                               // Cancel out hover amount

        if (buttonText != null)
        {
            buttonText.anchoredPosition = originalPosition;                                             // Stay at original position
            buttonText.localScale = originalScale;                                                      // and original scale
        }
    }

    public void LoadRoom()
    {
        if (!Application.CanStreamedLevelBeLoaded(nextScene))
        {
            Debug.LogError(
                $"Scene '{nextScene}' is missing from the build scene list.", this);                    // Debug message for terminal
            return;
        }

        if (SceneFade.Instance != null)
        {
            SceneFade.Instance.LoadRoom(nextScene, useTransition);                                      // Load room if scene fade is active
        }
        else
        {
            SceneManager.LoadSceneAsync(nextScene);                                                     // Else, load like normal
        }
    }

    public void QuitGame()
    {
        Application.Quit();                                                                             // Close game
    }
}