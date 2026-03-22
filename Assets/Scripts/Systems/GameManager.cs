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
    }
    void Start()
    {
        var gameState = GameState.I;
        var saveSystem = SaveSystem.I;
        if (gameState == null || saveSystem == null)
        {
            Debug.LogError("GameState or SaveSystem not found. Cannot initialize game.");
            return;
        }

        if (saveSystem.HasSave())
        {
            saveSystem.Load();
        }
        else
        {
            gameState.InitializeNewGame();
        }
        SceneManager.LoadScene("FarmHub");
    }
}