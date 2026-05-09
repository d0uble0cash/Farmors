using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class BenchInteract : MonoBehaviour
{
    [Header("UI")]
    public GameObject returnPrompt;
    public Button yesButton;
    public Button noButton;
    public GameObject interactHint;

    [Header("Settings")]
    public string farmSceneName = "Farmhub";

    private bool playerNearby = false;
    private bool promptOpen = false;

    private void Start()
    {
        yesButton.onClick.AddListener(ReturnToFarm);
        noButton.onClick.AddListener(ClosePrompt);
        returnPrompt.SetActive(false);
        if(interactHint != null) interactHint.SetActive(false);
    }

    private void Update()
    {
        if(playerNearby && !promptOpen && Keyboard.current.eKey.wasPressedThisFrame) {
            OpenPrompt();
            returnPrompt.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void OpenPrompt()
    {
        promptOpen = true;
        returnPrompt.SetActive(false);
        Time.timeScale = 0f;
    }

    private void ClosePrompt()
    {
        promptOpen = false;
        returnPrompt.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ReturnToFarm()
    {
        Time.timeScale = 1f;
        if(GameState.I != null)
        {
            GameState.I.lastCheckpointScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            GameState.I.lastCheckpointX = transform.position.x;
            GameState.I.lastCheckpointY = transform.position.y;
        }
        Debug.Log("Before saving, seed_corn count = " + GameState.I.PlayerInventory.GetCount("seed_corn"));
        SaveSystem.I?.Save();
        SceneManager.LoadScene(farmSceneName);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerNearby = true;
            if(interactHint != null) interactHint.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerNearby = false;
            ClosePrompt();
            if(interactHint != null) interactHint.SetActive(false);
        }
    }
}
