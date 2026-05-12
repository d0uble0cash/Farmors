using TMPro;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Killbox : MonoBehaviour
{
    [Header("Settings")]
    public string farmSceneName = "FarmHub";
    public float faintDuration = 2f;

    [Header("UI")]
    public GameObject faintPanel;
    public TextMeshProUGUI faintText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;

        Player player = other.GetComponent<Player>();
        if(player != null)
        {
            player.ChangeState(player.deathState);
        }

        StartCoroutine(FaintRoutine());
    }

    private System.Collections.IEnumerator FaintRoutine()
    {
        if(faintPanel != null) faintPanel.SetActive(true);
        if(faintText != null) faintText.text = "You fainted...";
        yield return new WaitForSeconds(faintDuration);
        
        if(faintPanel != null) faintPanel.SetActive(false);

        if(SaveSystem.I != null) SaveSystem.I.Save();
        SceneManager.LoadScene(farmSceneName);
    }

    public void TriggerFaint()
    {
        StartCoroutine(FaintRoutine());
    }
}
