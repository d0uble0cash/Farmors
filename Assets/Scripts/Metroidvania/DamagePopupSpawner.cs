using UnityEngine;

public class DamagePopupSpawner : MonoBehaviour
{
    [SerializeField] private DamagePopup damagePopupPrefab;

    private void Awake()
    {
        DamagePopup.prefab = damagePopupPrefab;
    }
}