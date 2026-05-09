using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenUI : MonoBehaviour
{
    public static DeathScreenUI Instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI faintText;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float displayDuration = 2f;
    [SerializeField] private string farmSceneName = "FarmHub";

    private void Awake()
    {
        Instance = this;
        canvasGroup.alpha = 0;
        gameObject.SetActive(true);
    }

    public void Show()
    {
        StartCoroutine(FaintSequence());
    }

    private IEnumerator FaintSequence()
    {
        float t= 0;
        while(t < fadeDuration)
        {
            t+= Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        faintText.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(displayDuration);

        Time.timeScale = 1f;
        SceneManager.LoadScene(farmSceneName);
    }
}
