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
    SaveSystem.I?.Load();
    SceneManager.LoadScene("FarmHub");
}
}