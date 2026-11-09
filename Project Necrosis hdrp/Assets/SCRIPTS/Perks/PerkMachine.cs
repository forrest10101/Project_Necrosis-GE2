using UnityEngine;

public class PerkMachine : MonoBehaviour
{
    // PERK SYSTEM, written by Evan Montgomery

    public PerkEffect perk;                                                                 // Locate perk effect script
    public Transform floatingImage;                                                         // Image above box
    public float spinSpeed = 25f;                                                           // Spin speed
    [Min(0f)] public float floatHeight = 0.15f;                                             // Float height
    [Min(0f)] public float floatSpeed = 1.5f;                                               // Float speed

    private Vector3 imageStartPosition;                                                     // Image start position
    private float animationTime;                                                            // Animation time

    private void Start()
    {
        if (floatingImage != null) imageStartPosition = floatingImage.localPosition;        // Begin in start position
    }

    private void Update()
    {
        if (floatingImage == null) return;                                                  // Cancel out if image isn't there

        animationTime += Time.deltaTime;                                                    // Use delta time
        floatingImage.localPosition = imageStartPosition +
            Vector3.up * Mathf.Sin(animationTime * floatSpeed) * floatHeight;               // Calculate floating pace
        floatingImage.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);          // Calculate rotation
    }
}
