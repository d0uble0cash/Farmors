using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask interactMask = ~0;

    [Header("Distance")]
    [SerializeField] private float interactDistance = 100f;
    [SerializeField] private bool useCameraFarClip = true;

    [Header("Input System")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private bool allowMouseClick = true;

    private CropPlot focused;

    private void Awake()
    {
        if (cam == null)
            cam = Camera.main;

        if (useCameraFarClip && cam != null)
            interactDistance = cam.farClipPlane;
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteractPerformed;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= OnInteractPerformed;
            interactAction.action.Disable();
        }
    }

    private void Update()
    {
        UpdateFocus_MouseHover();

        if (allowMouseClick && focused != null && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            focused.Interact();
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (focused != null && focused.CanInteract)
            focused.Interact();
    }

    private void UpdateFocus_MouseHover()
    {
        if (cam == null)
        {
            SetFocus(null);
            return;
        }

        if (Mouse.current == null)
        {
            SetFocus(null);
            return;
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);

        CropPlot newFocus = null;

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask))
        {
            var plot = hit.collider.GetComponentInParent<CropPlot>();
            if (plot != null && plot.MatchesCollider(hit.collider))
                newFocus = plot;
        }

        SetFocus(newFocus);
    }

    private void SetFocus(CropPlot newFocus)
    {
        if (newFocus == focused)
            return;

        if (focused != null)
            focused.SetFocused(false);

        focused = newFocus;

        if (focused != null)
            focused.SetFocused(true);
    }
}