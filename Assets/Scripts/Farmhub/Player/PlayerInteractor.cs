using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour {
    [Header("Proximity Detection")]
    [SerializeField] private LayerMask interactMask = ~0;

    [Header("Radius")]
    [SerializeField] private float interactRadius = 1f;

    [Header("Input System")]
    [SerializeField] private InputActionReference interactAction;

    private CropPlot focused;

    private void OnEnable() {

        if (interactAction != null) {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteractPerformed;
        }
    }

    private void OnDisable() {
        
        if (interactAction != null) {
            interactAction.action.performed -= OnInteractPerformed;
            interactAction.action.Disable();
        }
    }

    private void Update() {
        UpdateFocus_ByProximity();
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (focused != null && focused.CanInteract) {
            focused.Interact();
        }
    }

    private void UpdateFocus_ByProximity() {
        Vector3 origin = transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, interactRadius, interactMask);
        CropPlot closestPlot = null;
        float closestSqrDist = float.MaxValue;

        foreach (var hit in hits) {
            var plot = hit.GetComponentInParent<CropPlot>();

            if (plot != null && plot.MatchesCollider(hit)){
                float sqrDist = (plot.transform.position - origin).sqrMagnitude;

                if (sqrDist < closestSqrDist) {
                    closestSqrDist = sqrDist;
                    closestPlot = plot;
                }
            }
        }
        SetFocus(closestPlot);
    }

    private void SetFocus(CropPlot newFocus){
        if (newFocus == focused) {
            return;
        }

        if (focused != null) {
            focused.SetFocused(false);
        }
        focused = newFocus;

        if (focused != null) {
            focused.SetFocused(true);
        }   
    }
}