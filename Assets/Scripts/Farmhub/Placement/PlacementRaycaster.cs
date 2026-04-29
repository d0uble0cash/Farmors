using UnityEngine;
using UnityEngine.InputSystem;

public class PlacementRaycaster : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private FarmGrid farmGrid;

    [SerializeField] private LayerMask groundMask;

    public bool TryGetPlacementPosition(out Vector3 worldPosition)
    {
        worldPosition = Vector3.zero;

        if (cam == null || farmGrid == null)
            return false;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask))
        {   var gridPosition = farmGrid.ConvertToGridPosition(hit.point);
            worldPosition = farmGrid.ConvertToWorldPosition(gridPosition.x, gridPosition.y);
            return true;
        }

        return false;
    }
}