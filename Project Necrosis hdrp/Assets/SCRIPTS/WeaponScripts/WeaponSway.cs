using System.Collections;
using UnityEngine;

// WEAPON SWAY EFFECT, BY EVAN MONTGOMERY
public class WeaponSway : MonoBehaviour
{
    [SerializeField] private Transform gunVisual;              // Mesh or visual pivot
    [SerializeField, Min(0f)] private float sideSway = 0.015f;
    [SerializeField, Min(0f)] private float pullBack = 0.02f;
    [SerializeField, Min(0.1f)] private float smoothSpeed = 12f;

    [SerializeField, Min(0f)] private float shotKick = 0.04f;
    [SerializeField, Min(0f)] private float reloadDrop = 0.12f;
    [SerializeField, Min(0f)] private float swapDrop = 0.35f;
    [SerializeField, Min(0.01f)] private float slideTime = 0.16f;

    private Vector3 restPosition;
    private Vector3 swayOffset;
    private float recoil;
    private float reloadDuration;
    private float reloadElapsed;
    private float slideOffset;

    private void Awake()
    {
        restPosition = gunVisual.localPosition;
    }

    public void PlayShot()
    {
        recoil = shotKick;                                      // Kick back once per shot
    }

    public void PlayReload(float duration)
    {
        reloadDuration = duration;
        reloadElapsed = 0f;                                    // Start the reload dip
    }

    public IEnumerator Slide(bool lower)
    {
        float from = lower ? slideOffset : -swapDrop;
        float to = lower ? -swapDrop : 0f;
        slideOffset = from;

        float elapsed = 0f;
        while (elapsed < slideTime)
        {
            elapsed += Time.deltaTime;
            slideOffset = Mathf.SmoothStep(from, to, elapsed / slideTime);
            yield return null;
        }

        slideOffset = to;
    }

    private void LateUpdate()
    {
        float deltaTime = Time.deltaTime;
        if (deltaTime == 0f)
            return;

        Vector2 mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        mouse = Vector2.ClampMagnitude(mouse * 0.25f, 1f);

        Vector3 targetSway = new Vector3(
            -mouse.x * sideSway,
            -mouse.y * sideSway,
            -mouse.magnitude * pullBack);

        float blend = 1f - Mathf.Exp(-smoothSpeed * deltaTime);
        swayOffset = Vector3.Lerp(swayOffset, targetSway, blend);

        float reloadOffset = 0f;
        if (reloadElapsed < reloadDuration)
        {
            float progress = reloadElapsed / reloadDuration;
            reloadOffset = Mathf.Sin(progress * Mathf.PI) * reloadDrop;
            reloadElapsed += deltaTime;
        }

        gunVisual.localPosition = restPosition + swayOffset
            + new Vector3(0f, slideOffset - reloadOffset, -recoil);

        recoil = Mathf.MoveTowards(recoil, 0f, shotKick * deltaTime / 0.12f);
    }

    private void OnDisable()
    {
        gunVisual.localPosition = restPosition;                 // Reset when holstered
        swayOffset = Vector3.zero;
        recoil = 0f;
        reloadDuration = 0f;
        reloadElapsed = 0f;
        slideOffset = 0f;
    }
}