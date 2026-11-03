using UnityEngine;

public class CreditsScroll : MonoBehaviour
{
    [SerializeField] private RectTransform viewport;                            // Visible credits area
    [SerializeField] private float scrollSpeed = 50f;                           // Scroll speed
    [SerializeField] private float gap = 20f;                                   // Space outside screen

    private RectTransform creditsText;                                          // Credits text

    private void Start()
    {
        creditsText = GetComponent<RectTransform>();                            // Get text position
        ResetPosition();                                                        // Start below screen
    }

    private void Update()
    {
        creditsText.anchoredPosition +=
            Vector2.up * scrollSpeed * Time.unscaledDeltaTime;                  // Move upwards

        float textBottom = creditsText.anchoredPosition.y
            - creditsText.rect.height;

        if (textBottom > viewport.rect.height + gap)
        {
            ResetPosition();                                                    // Loop when text leaves screen
        }
    }

    private void ResetPosition()
    {
        creditsText.anchoredPosition = new Vector2(
            creditsText.anchoredPosition.x, -gap);                              // Move below screen
    }
}