using System.Collections;
using UnityEngine;

// WEAPON SWAY EFFECT, BY EVAN MONTGOMERY
public class WeaponSway : MonoBehaviour
{
    [SerializeField] private Transform gunVisual;                                       // Gun model
    [SerializeField, Min(0f)] private float sideSway = 0.015f;                          // Side-to-side movement from mouse input
    [SerializeField, Min(0f)] private float pullBack = 0.02f;                           // Backward movement from mouse input
    [SerializeField, Min(0.1f)] private float smoothSpeed = 12f;                        // How quickly the sway follows mouse movement

    [SerializeField, Min(0f)] private float shotKick = 0.04f;                           // How far the gun kicks back when fired
    [SerializeField, Min(0f)] private float reloadDrop = 0.12f;                         // How far the gun dips while reloading
    [SerializeField, Min(0f)] private float swapDrop = 0.35f;                           // How far the gun lowers when swapped
    [SerializeField, Min(0.01f)] private float slideTime = 0.16f;                       // How long the weapon swap movement takes

    private Vector3 restPosition;                                                       // The gun's original local position
    private Vector3 swayOffset;                                                         // Current movement from mouse sway
    private float recoil;                                                               // Current backwards movement from firing
    private float reloadDuration;                                                       // Total length of the reload animation
    private float reloadElapsed;                                                        // Time elapsed during the reload animation
    private float slideOffset;                                                          // Current vertical movement from swapping

    private void Awake()
    {
        restPosition = gunVisual.localPosition;                                         // Save the gun's starting position
    }

    public void PlayShot()
    {
        recoil = shotKick;                                                              // Kick back once per shot
    }

    public void PlayReload(float duration)
    {
        reloadDuration = duration;                                                      // Save how long the reload takes
        reloadElapsed = 0f;                                                             // Start the reload dip
    }

    public IEnumerator Slide(bool lower)
    {
        float from = lower ? slideOffset : -swapDrop;                                   // Set the starting slide position
        float to = lower ? -swapDrop : 0f;                                              // Set the target slide position
        slideOffset = from;                                                             // Apply the starting slide position

        float elapsed = 0f;                                                             // Track time elapsed during the slide
        while (elapsed < slideTime)                                                     // Continue until the slide finishes
        {
            elapsed += Time.deltaTime;                                                  // Update the elapsed time
            slideOffset = Mathf.SmoothStep(from, to, elapsed / slideTime);              // Smoothly move to the target position
            yield return null;                                                          // Wait for the next frame
        }

        slideOffset = to;                                                               // Set the exact final slide position
    }

    private void LateUpdate()
    {
        float deltaTime = Time.deltaTime;                                               // Get the time since the last frame
        if (deltaTime == 0f)                                                            // Check if the game is paused
            return;                                                                     // Skip movement while paused

        Vector2 mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));        // Read mouse movement
        mouse = Vector2.ClampMagnitude(mouse * 0.25f, 1f);                                      // Limit the sway amount

        Vector3 targetSway = new Vector3(                                               // Calculate the target sway position
            -mouse.x * sideSway,                                                        // Move opposite to horizontal mouse movement
            -mouse.y * sideSway,                                                        // Move opposite to vertical mouse movement
            -mouse.magnitude * pullBack);                                               // Pull the gun back during faster movement

        float blend = 1f - Mathf.Exp(-smoothSpeed * deltaTime);                         // Smooth the movement over time
        swayOffset = Vector3.Lerp(swayOffset, targetSway, blend);                       // Move the sway towards its target

        float reloadOffset = 0f;                                                        // Start with no reload movement
        if (reloadElapsed < reloadDuration)                                             // Check if the reload animation is active
        {
            float progress = reloadElapsed / reloadDuration;                            // Get progress through the reload
            reloadOffset = Mathf.Sin(progress * Mathf.PI) * reloadDrop;                 // Calculate the reload dip
            reloadElapsed += deltaTime;                                                 // Update the reload timer
        }

        gunVisual.localPosition = restPosition + swayOffset                             // Apply the original position and sway
            + new Vector3(0f, slideOffset - reloadOffset, -recoil);                     // Add the swap, reload, and recoil movement

        recoil = Mathf.MoveTowards(recoil, 0f, shotKick * deltaTime / 0.12f);           // Smoothly return from recoil
    }

    private void OnDisable()
    {
        gunVisual.localPosition = restPosition;                                         // Reset when holstered
        swayOffset = Vector3.zero;                                                      // Clear the sway movement
        recoil = 0f;                                                                    // Clear the recoil
        reloadDuration = 0f;                                                            // Clear the reload duration
        reloadElapsed = 0f;                                                             // Reset the reload timer
        slideOffset = 0f;                                                               // Clear the weapon swap movement
    }
}