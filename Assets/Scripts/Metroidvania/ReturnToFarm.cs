using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToFarm : MonoBehaviour
{
    [SerializeField] private string farmSceneName = "FarmScene";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (SaveSystem.I != null)
        {
            SaveSystem.I.Save();
        }

        SceneManager.LoadScene(farmSceneName);
    }
}