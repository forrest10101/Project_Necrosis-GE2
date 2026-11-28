using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CrosshairChange : MonoBehaviour
{
    // CROSSHAIR CHANGE, written by Evan Montgomery
    [SerializeField] private Camera aimCamera;                                              // Camera used to check what the player is aiming at
    [SerializeField, Min(0f)] private float aimDistance = 50f;                              // Maximum distance for the aim check
    [SerializeField] private LayerMask aimLayers = Physics.DefaultRaycastLayers;            // Layers the aim check can hit
    [SerializeField] private Color enemyColor = new Color(1f, 0.35f, 0.35f);                // Crosshair colour when aiming at an enemy
    [SerializeField, Range(0.5f, 1f)] private float enemyScale = 0.9f;                      // Crosshair size when aiming at an enemy

    private Image crosshair;                                                                // The crosshair image
    private Color normalColor;                                                              // The crosshair's original colour
    private Vector3 normalScale;                                                            // The crosshair's original size
    private float currentScale;                                                             // The crosshair's current size multiplier

    private void Awake()
    {
        crosshair = GetComponent<Image>();                                                  // Get the Image on this object
    }

    private void OnEnable()
    {
        normalColor = crosshair.color;                                                      // Save the original colour
        normalScale = crosshair.rectTransform.localScale;                                   // Save the original size
        currentScale = 1f;                                                                  // Reset the size multiplier
    }

    private void LateUpdate()
    {
        bool aimingAtEnemy = false;                                                         // Assume the player isn't aiming at an enemy

        if (aimCamera != null)                                                              // Check that a camera has been assigned
        {
            Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));            // Shoot a ray from the centre of the screen

            if (Physics.Raycast(ray, out RaycastHit hit, aimDistance, aimLayers,
                QueryTriggerInteraction.Ignore))                                            // Check what the ray hits
            {
                aimingAtEnemy = hit.collider.GetComponentInParent<EnemyHealth>() != null;   // Check if the hit object is an enemy
            }
        }

        Color tint = enemyColor;                                                            // Start with the enemy crosshair colour
        tint.a = normalColor.a;                                                             // Keep the original transparency
        crosshair.color = aimingAtEnemy ? tint : normalColor;                               // Change colour when aiming at an enemy

        float targetScale = aimingAtEnemy ? enemyScale : 1f;                                            // Choose the target size
        currentScale = Mathf.MoveTowards(currentScale, targetScale, 2f * Time.unscaledDeltaTime);       // Smoothly change the size
        crosshair.rectTransform.localScale = normalScale * currentScale;                                // Apply the size change
    }

    private void OnDisable()
    {
        crosshair.color = normalColor;                                                      // Restore the original colour
        crosshair.rectTransform.localScale = normalScale;                                   // Restore the original size
    }
}