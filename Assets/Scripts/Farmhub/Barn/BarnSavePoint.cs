using UnityEngine;

public class BarnSavePoint : MonoBehaviour, IInteractable
{
    [Header("Interaction")]
    [SerializeField] private Collider interactionCollider;

    [Header("World Prompt")]
    [SerializeField] private WorldPrompt prompt;

    private bool isFocused = false;

    public bool CanInteract => SaveSystem.I != null && GameState.I != null;

    private void Awake()
    {
        if (interactionCollider == null)
        {
            interactionCollider = GetComponent<Collider>();
        }

        if (prompt != null)
        {
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
        if (SaveSystem.I == null)
        {
            Debug.LogWarning("SaveSystem.I is null. Cannot save.", this);
            return;
        }

        SaveSystem.I.Save();
        Debug.Log("Game saved at barn.", this);
    }

    public void SetFocused(bool focused)
    {
        isFocused = focused;
        RefreshPrompt();
    }

    public bool MatchesCollider(Collider hit)
    {
        if (hit == null)
        {
            return false;
        }

        if (interactionCollider != null)
        {
            return hit == interactionCollider;
        }

        return hit.GetComponentInParent<BarnSavePoint>() == this;
    }

    private void RefreshPrompt()
    {
        if (prompt == null)
        {
            return;
        }

        if (!isFocused)
        {
            prompt.Show(false);
            prompt.SetPulsing(false);
            return;
        }

        prompt.SetText("[E] Save");
        prompt.Show(true);
        prompt.SetPulsing(false);
    }
}