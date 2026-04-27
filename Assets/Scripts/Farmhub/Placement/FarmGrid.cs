using UnityEngine;

public class FarmGrid : MonoBehaviour
{
    [SerializeField] float cellSize = 1f;
    [SerializeField] int width = 10;
    [SerializeField] int height = 10;

    public Vector3 ConvertToWorldPosition(int x, int y)
    {
        return new Vector3(
            x * cellSize + transform.position.x + cellSize * 0.5f,
            transform.position.y,
            y * cellSize + transform.position.z + cellSize * 0.5f
        );
    }

    public Vector2Int ConvertToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - transform.position.x) / cellSize);
        int y = Mathf.FloorToInt((worldPosition.z - transform.position.z) / cellSize);

        return new Vector2Int(x, y);
    }
    internal Vector3 ConvertToWorldPosition(Vector2Int vector2Int)
    {    
        return ConvertToWorldPosition(vector2Int.x, vector2Int.y);
    }
}