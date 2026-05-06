using UnityEngine;

public class ChestInteract : MonoBehaviour, IInteractable
{
    [Header("Animation")]
    [SerializeField] private Animator lidAnimator;

    [Header("Chest Screen")]
    [SerializeField] private GameObject chestScreen;

    [Header("Inventory")]
    [SerializeField] private ChestInventory chestInventory;
    [SerializeField] private InventoryUI chestInventoryUI;

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
            interactionCollider = GetComponent<Collider>();

        if (chestInventory == null)
            chestInventory = GetComponent<ChestInventory>();

        if (prompt != null)
        {
            prompt.Show(false);
            prompt.SetPulsing(false);
        }

        if (chestScreen != null)
            chestScreen.SetActive(false);
    }

    private void Reset()
    {
        interactionCollider = GetComponent<Collider>();
        chestInventory = GetComponent<ChestInventory>();
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
            CloseChest();
        }
        else
        {
            OpenChest();
        }

        RefreshPrompt();
    }

    private void OpenChest()
    {
        lidAnimator.Play("chest_Open");
        isOpen = true;

        if (chestScreen != null)
            chestScreen.SetActive(true);

        if (chestInventoryUI != null && chestInventory != null)
            chestInventoryUI.Show(chestInventory.Inventory);
    }

    private void CloseChest()
    {
        lidAnimator.Play("Chest_Close");
        isOpen = false;

        if (chestScreen != null)
            chestScreen.SetActive(false);
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

        return hit.GetComponentInParent<ChestInteract>() == this;
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

        prompt.SetText(isOpen ? "[E] Close" : "[E] Open");
        prompt.Show(true);
        prompt.SetPulsing(false);
    }
}