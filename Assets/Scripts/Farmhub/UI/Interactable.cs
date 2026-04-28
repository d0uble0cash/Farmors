public interface IInteractable
{
    bool CanInteract { get; }
    void Interact();
    void SetFocused(bool focused);
    bool MatchesCollider(UnityEngine.Collider hit);
}