using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class BackgroundScroll : MonoBehaviour
{
    // BACKGROUND SCROLL EFFECT, MADE BY EVAN MONTGOMERY
    [SerializeField] private Vector2 direction = Vector2.left;                      // Direction of scroll
    [SerializeField, Min(0f)] private float scrollSpeed = 0.03f;                    // Scroll speed
    [SerializeField] private bool useUnscaledTime = true;                           // Use unscaled time

    private RawImage backgroundImage;                                               // Use raw image

    private void Awake()
    {
        backgroundImage = GetComponent<RawImage>();                                 // Get raw image component
        backgroundImage.raycastTarget = false;                                      // Not a raycast target
    }

    private void Update()
    {
        float deltaTime = useUnscaledTime                                           // Calculate time
            ? Time.unscaledDeltaTime
            : Time.deltaTime;

        Vector2 movement = direction.normalized * scrollSpeed * deltaTime;          // Calculate movement

        Rect textureArea = backgroundImage.uvRect;                                  // Texture area

        textureArea.x = Mathf.Repeat(textureArea.x - movement.x, 1f);               // Move at X speed chosen
        textureArea.y = Mathf.Repeat(textureArea.y - movement.y, 1f);               // Move at Y speed chosen

        backgroundImage.uvRect = textureArea;                                       // Set background image to update in live time
    }
}