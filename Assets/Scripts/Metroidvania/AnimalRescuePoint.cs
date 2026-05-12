using UnityEngine;

public class AnimalRescuePoint2D : MonoBehaviour
{
    [Header("Rescue")]
    [SerializeField] private string rescueId = "mv_sheep_01";
    [SerializeField] private string animalId = "sheep";
    [SerializeField] private int amount = 1;
    [SerializeField] private bool saveImmediately = true;

    [Header("Object")]
    [SerializeField] private GameObject objectToHide;

    [Header("Effects")]
    [SerializeField] private ParticleSystem rescueBurstPrefab;
    [SerializeField] private AudioClip poofSound;
    [SerializeField] private float poofVolume = 1f;

    private bool rescued = false;

    private void Start()
    {
        if (objectToHide == null)
        {
            objectToHide = transform.root.gameObject;
        }

        if (GameState.I != null && GameState.I.HasRescuedAnimal(rescueId))
        {
            objectToHide.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (rescued)
            return;

        if (!other.CompareTag("Player"))
            return;

        if (GameState.I == null)
        {
            Debug.LogError("GameState.I is null.", this);
            return;
        }

        GameState.I.AddRescuedAnimal(rescueId, 1);
        GameState.I.AddRescuedAnimal(animalId, amount);

        rescued = true;

        Debug.Log($"Rescued {amount}x {animalId}", this);

        if (saveImmediately && SaveSystem.I != null)
        {
            SaveSystem.I.Save();
        }

        PlayRescueEffects();

        if (objectToHide != null)
        {
            Destroy(objectToHide);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PlayRescueEffects()
    {
        if (rescueBurstPrefab != null)
        {
            ParticleSystem burst = Instantiate(
                rescueBurstPrefab,
                transform.position,
                Quaternion.identity
            );

            burst.Play();

            ParticleSystem.MainModule main = burst.main;
            Destroy(burst.gameObject, main.duration + main.startLifetime.constantMax);
        }

        if (poofSound != null)
        {
            AudioSource.PlayClipAtPoint(poofSound, transform.position, poofVolume);
        }
    }
}