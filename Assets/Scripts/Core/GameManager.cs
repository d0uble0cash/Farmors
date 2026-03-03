using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved progress into GameState (if a save exists)
        if (SaveSystem.I != null) SaveSystem.I.Load();

        SceneManager.LoadScene("FarmHub");
    }
}