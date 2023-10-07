using UnityEngine;

public class Tile
{
    public enum TileTerrain
    {
        NOT_WALKABLE,
        WALKABLE,
        HAS_A_UNIT_ON
    }
    public int x;
    public float y;
    public int z;
    public Vector3 realPos;
    public GameObject currentTile;
    public TileTerrain state;
    public int G;
    public int H;
    public int F { get { return G + H; } }
    public Tile previous;

    public bool IsThisTileWalkable()
    {
        return this.state == TileTerrain.WALKABLE;
    }
    public Tile(TileTerrain state, int xPos, float yPos, int zPos, GameObject go, Vector3 position)
    {
        this.state = state;
        x = xPos;
        y = yPos;
        z = zPos;
        realPos = position;
        if (go != null) currentTile = go;
    }
}