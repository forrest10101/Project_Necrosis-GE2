using System.Collections;
using UnityEngine;

// PERK DISPLAY, by Evan Montgomery
public class PerkDisplay : MonoBehaviour
{
    [SerializeField] private RectTransform[] perkPanels;                        // Perk objects, in display order
    [SerializeField] private int startingPerk = 0;                              // The perk shown when the menu opens
    [SerializeField] private float transitionDuration = 0.3f;                   // How long the animation takes
    [SerializeField] private float slideDistance = 100f;                        // How far each perk slides

    private CanvasGroup[] canvasGroups;                                         // Controls each perk's fade
    private Vector2[] originalPositions;                                        // Stores each perk's starting position
    private int currentPerk;                                                    // The perk currently being shown
    private bool isTransitioning;                                               // Stops the buttons being pressed mid-animation

    private void Awake()
    {
        if (perkPanels == null || perkPanels.Length == 0)                       // Check that the perk list isn't empty
        {
            Debug.LogError("Add your perk panels to PerkDisplay.");             // Show an error if no perks were assigned
            enabled = false;                                                    // Stop this script from running
            return;                                                             // Exit Awake
        }

        canvasGroups = new CanvasGroup[perkPanels.Length];                      // Make space for each perk's Canvas Group
        originalPositions = new Vector2[perkPanels.Length];                     // Make space for each perk's starting position

        currentPerk = Mathf.Clamp(startingPerk, 0, perkPanels.Length - 1);      // Keep the starting index in range

        for (int i = 0; i < perkPanels.Length; i++)                             // Set up each perk panel
        {
            if (perkPanels[i] == null)                                          // Check that this slot has a perk assigned
            {
                Debug.LogError($"Perk panel {i} is missing.");                  // Show which perk is missing
                enabled = false;                                                // Stop this script from running
                return;                                                         // Exit Awake
            }

            canvasGroups[i] = perkPanels[i].GetComponent<CanvasGroup>();        // Get the Canvas Group from the perk

            if (canvasGroups[i] == null)                                        // Check that the perk has a Canvas Group
            {
                Debug.LogError($"Add a Canvas Group to {perkPanels[i].name}."); // Show which perk needs one
                enabled = false;                                                // Stop this script from running
                return;                                                         // Exit Awake
            }

            originalPositions[i] = perkPanels[i].anchoredPosition;              // Remember the perk's starting position

            bool isCurrent = i == currentPerk;                                  // Check if this is the starting perk
            perkPanels[i].gameObject.SetActive(isCurrent);                      // Only show the starting perk
            canvasGroups[i].alpha = 1f;                                         // Reset the perk's opacity
        }
    }

    public void NextPerk()
    {
        ShowPerk(currentPerk + 1, 1);                                           // Move to the next perk, sliding left
    }

    public void PreviousPerk()
    {
        ShowPerk(currentPerk - 1, -1);                                          // Move to the previous perk, sliding right
    }

    private void ShowPerk(int newPerk, int direction)
    {
        if (isTransitioning || perkPanels.Length < 2)                           // Ignore clicks during animation or with one perk
            return;                                                             // Exit without changing the display

        newPerk = (newPerk + perkPanels.Length) % perkPanels.Length;            // Wrap around at either end

        if (newPerk == currentPerk)                                             // Check that the selected perk is different
            return;                                                             // Exit without starting an animation

        StartCoroutine(TransitionToPerk(newPerk, direction));                   // Start the slide and fade animation
    }

    private IEnumerator TransitionToPerk(int newPerk, int direction)
    {
        isTransitioning = true;                                                 // Lock the buttons during the animation

        int oldPerk = currentPerk;                                              // Remember which perk is currently visible

        perkPanels[newPerk].gameObject.SetActive(true);                         // Show the incoming perk
        canvasGroups[newPerk].alpha = 0f;                                       // Start it fully transparent
        canvasGroups[newPerk].interactable = false;                             // Stop it receiving input during the animation
        canvasGroups[newPerk].blocksRaycasts = false;                           // Let clicks pass through during the animation

        perkPanels[newPerk].anchoredPosition =                                  // Start the new perk just off-screen
            originalPositions[newPerk] + Vector2.right * direction * slideDistance;

        float elapsed = 0f;                                                     // Track how much time has passed

        while (elapsed < transitionDuration)                                    // Keep animating until the duration ends
        {
            elapsed += Time.unscaledDeltaTime;                                  // Update even if the game is paused

            float progress = Mathf.Clamp01(elapsed / transitionDuration);       // Get animation progress from 0 to 1
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);           // Smooth the start and end of the movement

            perkPanels[oldPerk].anchoredPosition = Vector2.Lerp(                // Slide the old perk away
                originalPositions[oldPerk],
                originalPositions[oldPerk] - Vector2.right * direction * slideDistance,
                easedProgress
            );

            perkPanels[newPerk].anchoredPosition = Vector2.Lerp(                // Slide the new perk into place
                originalPositions[newPerk] + Vector2.right * direction * slideDistance,
                originalPositions[newPerk],
                easedProgress
            );

            canvasGroups[oldPerk].alpha = 1f - easedProgress;                   // Fade the old perk out
            canvasGroups[newPerk].alpha = easedProgress;                        // Fade the new perk in

            yield return null;                                                  // Wait for the next frame
        }

        perkPanels[oldPerk].anchoredPosition = originalPositions[oldPerk];      // Reset the old perk's position
        canvasGroups[oldPerk].alpha = 1f;                                       // Reset the old perk's opacity
        perkPanels[oldPerk].gameObject.SetActive(false);                        // Hide the old perk

        perkPanels[newPerk].anchoredPosition = originalPositions[newPerk];      // Set the new perk in its exact position
        canvasGroups[newPerk].alpha = 1f;                                       // Make sure the new perk is fully visible
        canvasGroups[newPerk].interactable = true;                              // Allow the new perk to receive input
        canvasGroups[newPerk].blocksRaycasts = true;                            // Let the new perk receive clicks

        currentPerk = newPerk;                                                  // Save the newly displayed perk
        isTransitioning = false;                                                // Allow the buttons to be pressed again
    }
}