using UnityEngine;

public class GridNode {
    public int x, y;
    public Vector2 gridPos;
    public bool walkable;
    public Vector3 worldPosition;
    public float g = float.PositiveInfinity;
    public float rhs = float.PositiveInfinity;
    public GridNode parent;

    public GridNode(int x, int y, bool walkable, Vector2 gridPos, Vector3 worldPos) {
        this.x = x;
        this.y = y;
        this.gridPos = gridPos;
        this.walkable = walkable;
        this.worldPosition = worldPos;
    }
}
