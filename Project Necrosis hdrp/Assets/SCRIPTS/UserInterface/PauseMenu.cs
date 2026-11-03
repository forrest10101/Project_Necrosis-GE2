using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseScreen;             // Pause screen

    private bool paused;                                        // If paused
    private float previousTimeScale = 1f;                       // Speed before pausing

    private void Awake()
    {
        pauseScreen.SetActive(false);                           // Start with screen hidden
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
                ResumeGame();                                   // Resume if already paused
            else
                PauseGame();                                    // Otherwise pause
        }
    }

    public void PauseGame()
    {
        if (paused) return;

        previousTimeScale = Time.timeScale;                         // Save game speed
        Time.timeScale = 0f;                                        // Pause gameplay
        paused = true;
        pauseScreen.SetActive(true);                                // Show pause screen

        Cursor.lockState = CursorLockMode.None;                     // Unlock mouse
        Cursor.visible = true;                                     // Show mouse
    }

    public void ResumeGame()
    {
        if (!paused) return;

        Time.timeScale = previousTimeScale;                         // Restore game speed
        paused = false;
        pauseScreen.SetActive(false);                               // Hide pause screen

        Cursor.lockState = CursorLockMode.Locked;                   // Lock mouse for aiming
        Cursor.visible = false;                                    // Hide mouse
    }

    private void OnDestroy()
    {
        if (paused)
            Time.timeScale = previousTimeScale;                 // Restore speed when leaving scene
    }
}