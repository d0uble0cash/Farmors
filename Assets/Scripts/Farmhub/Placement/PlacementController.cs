using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlacementController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlacementRaycaster raycaster;
    [SerializeField] private FarmGrid farmGrid;
    [SerializeField] private Transform previewObject;
    [SerializeField] private GameObject placeablePrefab;

    [Header("Settings")]
    [SerializeField] private bool placementModeActive = false;
    [SerializeField] private bool hidePreviewWhenInvalid = false;

    [Header("Blocking")]
    [SerializeField] private LayerMask blockingMask;
    [SerializeField] private Vector3 placementCheckHalfExtents = new Vector3(0.45f, 0.5f, 0.45f);
    [SerializeField] private Vector3 placementCheckOffset = Vector3.zero;

    private readonly HashSet<Vector2Int> occupiedCells = new();
    private Renderer previewRenderer;

    private void Awake()
    {
        if (previewObject != null)
        {
            previewRenderer = previewObject.GetComponentInChildren<Renderer>();

            if (previewRenderer != null)
            {
                previewRenderer.material = new Material(previewRenderer.material);
            }

            previewObject.gameObject.SetActive(placementModeActive);
        }
    }

    public void OnBuildToggle(InputValue value)
    {
        if (!value.isPressed)
            return;

        placementModeActive = !placementModeActive;
        SetPreviewVisible(placementModeActive);
        Debug.Log("Placement mode: " + placementModeActive);
    }

    private void Update()
    {
        if (!placementModeActive)
        {
            SetPreviewVisible(false);
            return;
        }

        if (raycaster == null || farmGrid == null || previewObject == null || placeablePrefab == null)
        {
            SetPreviewVisible(false);
            return;
        }

        if (!raycaster.TryGetPlacementPosition(out Vector3 snappedWorldPosition))
        {
            SetPreviewVisible(false);
            return;
        }

        Vector2Int gridPosition = farmGrid.ConvertToGridPosition(snappedWorldPosition);

        bool isLogicallyOccupied = occupiedCells.Contains(gridPosition);
        bool isPhysicallyBlocked = IsPhysicallyBlocked(snappedWorldPosition);
        bool isBlocked = isLogicallyOccupied || isPhysicallyBlocked;

        if (hidePreviewWhenInvalid && isBlocked)
        {
            SetPreviewVisible(false);
        }
        else
        {
            SetPreviewVisible(true);
            previewObject.position = snappedWorldPosition;
            UpdatePreviewColor(isBlocked ? Color.red : Color.green);
        }

        if (isBlocked)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceObject(gridPosition, snappedWorldPosition);
        }
    }

    private bool IsPhysicallyBlocked(Vector3 worldPosition)
    {
        Vector3 checkCenter = worldPosition + placementCheckOffset;

        return Physics.CheckBox(
            checkCenter,
            placementCheckHalfExtents,
            Quaternion.identity,
            blockingMask
        );
    }

    private void PlaceObject(Vector2Int gridPosition, Vector3 worldPosition)
    {
        Instantiate(placeablePrefab, worldPosition, Quaternion.identity);
        occupiedCells.Add(gridPosition);
    }

    private void SetPreviewVisible(bool isVisible)
    {
        if (previewObject != null && previewObject.gameObject.activeSelf != isVisible)
            previewObject.gameObject.SetActive(isVisible);
    }

    private void UpdatePreviewColor(Color color)
    {
        if (previewRenderer == null)
            return;

        previewRenderer.material.color = color;
    }

    public void SetPlacementMode(bool active)
    {
        placementModeActive = active;
        SetPreviewVisible(active);
    }

    public bool IsOccupied(Vector2Int gridPosition)
    {
        return occupiedCells.Contains(gridPosition);
    }
}