using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "LEVEL ONE")
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
