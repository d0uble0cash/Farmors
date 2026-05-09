using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public static DamagePopup prefab;
    private TextMeshPro text;
    private float moveSpeed = 2f;
    private float fadeSpeed = 2f;
    private Color textColor;

    public static void Create(Vector3 position, int damage)
    {
        DamagePopup popup = Instantiate(prefab, position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        popup.Setup(damage);
    }

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    public void Setup(int damage)
    {
        text.SetText(damage.ToString());
        textColor = text.color;
    }

    private void Update()
    {
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
        textColor.a -= fadeSpeed * Time.deltaTime;
        text.color = textColor;
        if (textColor.a <= 0)
            Destroy(gameObject);
    }
}