using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private EventHandlerUI eventHandlerUI;
    [SerializeField] private WorldPrompt prompt;
    [SerializeField] private Collider interactCollider;

    [Header("Prompt")]
    [SerializeField] private string promptText = "[E] Enter Forest";

    private bool isFocused;

    public bool CanInteract => true;

    private void Awake()
    {
        if (interactCollider == null)
            interactCollider = GetComponentInChildren<Collider>();

        if (prompt != null)
        {
            prompt.Show(false);
            prompt.SetPulsing(false);
        }
    }

    public void Interact()
    {
        if (!CanInteract)
            return;

        if (eventHandlerUI == null)
        {
            Debug.LogWarning("SceneTransitionTrigger is missing EventHandlerUI reference.", this);
            return;
        }

        SceneManager.LoadScene("MetroidTester");
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

        if (interactCollider != null)
            return hit == interactCollider || hit.transform.IsChildOf(interactCollider.transform);

        return hit.GetComponentInParent<SceneTransitionTrigger>() == this;
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

        prompt.SetText(promptText);
        prompt.Show(true);
        prompt.SetPulsing(false);
    }
}
