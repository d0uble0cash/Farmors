using UnityEngine;
public class ChestInteract : MonoBehaviour, IInteractable
{
    [Header("Animation")]
    [SerializeField] private Animator lidAnimator;

    [Header("ChestScreen")]
    [SerializeField] private GameObject chestScreen;
    
    [Header("Interaction")]
    [SerializeField] private Collider interactionCollider;

    [Header("World Prompt")]
    [SerializeField] private WorldPrompt prompt;

    private bool isOpen = false;
    private bool isFocused = false;

    public bool CanInteract => true;
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
        if (lidAnimator == null)
        {
            Debug.LogError("Lid Animator not assigned.", this);
            return;
        }

        if (isOpen)
        {
            lidAnimator.Play("Chest_Close");
            isOpen = false;
            chestScreen.SetActive(false);
        }
        else
        {
            lidAnimator.Play("chest_Open");
            isOpen = true;
            chestScreen.SetActive(true);
        }

        RefreshPrompt();
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

        return hit.GetComponentInParent<ChestInteract>() == this;
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

        prompt.SetText(isOpen ? "[E] Close" : "[E] Open");
        prompt.Show(true);
        prompt.SetPulsing(false);
    }
}