using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public int movmentPenalty;
    public Node parent;

    public bool grouped;
    public Group group;

    int HeapIndex;

    public Node ( bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty )
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        movmentPenalty = _penalty;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int heapIndex
    {
        get
        {
            return HeapIndex;
        }
        set
        {
            this.HeapIndex = value;
        }
    }

    public int CompareTo ( Node nodeToCompare )
    {
        int compare = fCost.CompareTo (nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo (nodeToCompare.hCost);
        }

        return -compare;
    }
}
