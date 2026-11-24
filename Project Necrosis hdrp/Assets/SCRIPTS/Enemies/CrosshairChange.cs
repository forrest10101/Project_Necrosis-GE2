using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CrosshairChange : MonoBehaviour
{
    [SerializeField] private Camera aimCamera;
    [SerializeField, Min(0f)] private float aimDistance = 50f;
    [SerializeField] private LayerMask aimLayers = Physics.DefaultRaycastLayers;
    [SerializeField] private Color enemyColor = new Color(1f, 0.35f, 0.35f);
    [SerializeField, Range(0.5f, 1f)] private float enemyScale = 0.9f;

    private Image crosshair;
    private Color normalColor;
    private Vector3 normalScale;
    private float currentScale;

    private void Awake()
    {
        crosshair = GetComponent<Image>();
    }

    private void OnEnable()
    {
        normalColor = crosshair.color;
        normalScale = crosshair.rectTransform.localScale;
        currentScale = 1f;
    }

    private void LateUpdate()
    {
        bool aimingAtEnemy = false;

        if (aimCamera != null)
        {
            Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, aimDistance, aimLayers,
                QueryTriggerInteraction.Ignore))
            {
                aimingAtEnemy = hit.collider.GetComponentInParent<EnemyHealth>() != null;
            }
        }

        Color tint = enemyColor;
        tint.a = normalColor.a;
        crosshair.color = aimingAtEnemy ? tint : normalColor;

        float targetScale = aimingAtEnemy ? enemyScale : 1f;
        currentScale = Mathf.MoveTowards(currentScale, targetScale, 2f * Time.unscaledDeltaTime);
        crosshair.rectTransform.localScale = normalScale * currentScale;
    }

    private void OnDisable()
    {
        crosshair.color = normalColor;
        crosshair.rectTransform.localScale = normalScale;
    }
}
