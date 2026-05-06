using System.Collections;
using UnityEngine;

public class AnimalInteract : MonoBehaviour, IInteractable
{
    [Header("Animal")]
    [SerializeField] private string animalName = "Chicken";

    [Header("Interaction")]
    [SerializeField] private Collider interactionCollider;

    [Header("World Prompt")]
    [SerializeField] private WorldPrompt prompt;

    [Header("Reaction")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float reactionDuration = 0.6f;
    [SerializeField] private float bounceHeight = 0.12f;
    [SerializeField] private float bounceSpeed = 12f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip petSound;
    [SerializeField] private float minPitch = 0.7f;
    [SerializeField] private float maxPitch = 1.4f;

    [Header("Idle Audio")]
    [SerializeField] private bool enableIdleSounds = true;
    [SerializeField] private float minIdleTime = 6f;
    [SerializeField] private float maxIdleTime = 15f;
    [SerializeField] private float idleVolumeMultiplier = 0.6f;

    [Header("Shared Noise Cooldown")]
    [SerializeField] private float sharedSoundCooldown = 2f;

    private static float nextAllowedAnimalSoundTime;

    private bool isFocused;
    private bool isReacting;
    private float idleTimer;
    private Vector3 visualStartLocalPosition;

    public bool CanInteract => !isReacting;

    private void Awake()
    {
        if (interactionCollider == null)
            interactionCollider = GetComponentInChildren<Collider>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (visualRoot != null)
            visualStartLocalPosition = visualRoot.localPosition;

        if (prompt != null)
        {
            prompt.Show(false);
            prompt.SetPulsing(false);
        }
    }

    private void Start()
    {
        ResetIdleTimer();
    }

    private void Update()
    {
        if (!enableIdleSounds || isReacting)
            return;

        idleTimer -= Time.deltaTime;

        if (idleTimer <= 0f)
        {
            TryPlayIdleSound();
            ResetIdleTimer();
        }
    }

    private void Reset()
    {
        interactionCollider = GetComponentInChildren<Collider>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (isReacting)
            return;

        Debug.Log($"Petted {animalName}");

        PlaySound(1f, true);
        StartCoroutine(PetReaction());
    }

    private IEnumerator PetReaction()
    {
        isReacting = true;

        float timer = 0f;

        while (timer < reactionDuration)
        {
            timer += Time.deltaTime;

            if (visualRoot != null)
            {
                float bounce = Mathf.Abs(Mathf.Sin(timer * bounceSpeed)) * bounceHeight;
                visualRoot.localPosition = visualStartLocalPosition + new Vector3(0f, bounce, 0f);
            }

            yield return null;
        }

        if (visualRoot != null)
            visualRoot.localPosition = visualStartLocalPosition;

        isReacting = false;
    }

    private void TryPlayIdleSound()
    {
        if (Time.time < nextAllowedAnimalSoundTime)
            return;

        PlaySound(idleVolumeMultiplier, false);
        nextAllowedAnimalSoundTime = Time.time + sharedSoundCooldown;
    }

    private void PlaySound(float volumeMultiplier, bool ignoreSharedCooldown)
    {
        if (audioSource == null || petSound == null)
            return;

        if (!ignoreSharedCooldown && Time.time < nextAllowedAnimalSoundTime)
            return;

        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.PlayOneShot(petSound, volumeMultiplier);

        nextAllowedAnimalSoundTime = Time.time + sharedSoundCooldown;
    }

    private void ResetIdleTimer()
    {
        idleTimer = Random.Range(minIdleTime, maxIdleTime);
    }

    public void SetFocused(bool focused)
    {
        isFocused = focused;
        RefreshPrompt();
    }

    public bool MatchesCollider(Collider hit)
    {
        if (hit == null)
            return false;

        if (interactionCollider != null)
            return hit == interactionCollider;

        return hit.GetComponentInParent<AnimalInteract>() == this;
    }

    private void RefreshPrompt()
    {
        if (prompt == null)
            return;

        if (!isFocused)
        {
            prompt.Show(false);
            prompt.SetPulsing(false);
            return;
        }

        prompt.SetText($"[E] Pet {animalName}");
        prompt.Show(true);
        prompt.SetPulsing(false);
    }
}