using UnityEngine;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image[] hearts; // drag your heart images in order
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private int hpPerHeart = 10;

    private void OnEnable()
    {
        playerHealth.OnDamaged += UpdateHearts;
        playerHealth.OnDeath += UpdateHearts;
    }

    private void OnDisable()
    {
        playerHealth.OnDamaged -= UpdateHearts;
        playerHealth.OnDeath -= UpdateHearts;
    }

    private void Start() => UpdateHearts(Vector2.zero);

    private void UpdateHearts(Vector2 _)
    {
        int filledHearts = Mathf.CeilToInt((float)playerHealth.health / hpPerHeart);

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = i < filledHearts ? fullHeart : emptyHeart;
        }
    }
}
