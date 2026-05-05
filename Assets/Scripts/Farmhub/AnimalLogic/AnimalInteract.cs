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

    private bool isFocused;
    private bool isReacting;
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

        // 🔊 Play sound
        if (audioSource != null && petSound != null)
        {
            audioSource.PlayOneShot(petSound);
        }

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