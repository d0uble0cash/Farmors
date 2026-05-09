using UnityEngine;

public class GenericWorldPromptInteract : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private Collider interactionCollider;

    [Header("World Prompt")]
    [SerializeField] private WorldPrompt prompt;

    [Header("Prompt Message")]
    [TextArea(1, 4)]
    [SerializeField] private string message = "Prompt message here";

    [Header("Prompt Settings")]
    [SerializeField] private bool pulsePrompt = false;

    private bool isFocused = false;

    public bool CanInteract => true;

    private void Awake()
    {
        if (interactionCollider == null)
            interactionCollider = GetComponent<Collider>();

        if (prompt != null)
        {
            prompt.SetText(message);
            prompt.Show(false);
            prompt.SetPulsing(false);
        }
    }

    private void Reset()
    {
        interactionCollider = GetComponent<Collider>();
    }

    public void Interact()
    {
        // No action needed.
        // This object only shows a world prompt.
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

        return hit.GetComponentInParent<GenericWorldPromptInteract>() == this;
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

        prompt.SetText(message);
        prompt.Show(true);
        prompt.SetPulsing(pulsePrompt);
    }
}