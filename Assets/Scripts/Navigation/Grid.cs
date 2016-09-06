using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Grid
{
    public bool displayGridGizmos = true;

    public LayerMask unwalkableMask = LayerMask.NameToLayer ("Obstacle");
    public Vector2 gridWorldSize = new Vector2 (100, 100);
    public float nodeRadius = .5f;
    public TerrayinType[] terrainMask = new TerrayinType[0] { };
    public Vector3 position = Vector3.zero;

    LayerMask walkableMask;
    Dictionary<int, int> terrainMaskDictionary = new Dictionary<int, int> ();

    Node[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeY;
    int gridGroupAmount = 1;

    public void Init ()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt (gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt (gridWorldSize.y / nodeDiameter);

        foreach (TerrayinType terrain in terrainMask)
        {
            walkableMask |= terrain.terrainMask.value;
            terrainMaskDictionary.Add ((int)Mathf.Log (terrain.terrainMask.value, 2), terrain.terrainPenalty);
        }

        CreateGrid ();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    /// <summary>
    /// Creates a grid for the A* algorithm.
    /// </summary>
    void CreateGrid ()
    {
        Stopwatch sw = new Stopwatch ();
        sw.Start ();
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere (worldPoint, nodeRadius, unwalkableMask);
                int movmentPenalty = 0;

                if (walkable)
                {
                    Ray ray = new Ray (worldPoint + Vector3.up * 50, Vector3.down);
                    RaycastHit hit;
                    if (Physics.Raycast (ray, out hit, 100, walkableMask))
                    {
                        terrainMaskDictionary.TryGetValue (hit.collider.gameObject.layer, out movmentPenalty);
                        worldPoint.y = hit.point.y + 1;
                    }
                }

                grid[x, y] = new Node (walkable, worldPoint, x, y, movmentPenalty);
            }
        }

        FindGridGroups ();

        sw.Stop ();
        UnityEngine.Debug.Log ("Grid baking took " + sw.ElapsedMilliseconds + "ms.");
    }

    /// <summary>
    /// Uses the floodfill to find all isolated groups on the grid.
    /// </summary>
    void FindGridGroups ()
    {
        List<Group> groups = new List<Group> (); // If i need it later.

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (!grid[x, y].grouped && grid[x, y].walkable)
                {
                    groups.Add (FloodFill (grid[x, y]));
                }
            }
        }
    }

    /// <summary>
    /// Floodfill to find all members of one group.
    /// </summary>
    /// <param name="startNode">Node to start floodfill from.</param>
    /// <returns>Group.</returns>
    Group FloodFill ( Node startNode )
    {
        Queue<Node> nodeQueue = new Queue<Node> ();
        Group group = new Group (gridGroupAmount);
        gridGroupAmount++;

        startNode.grouped = true;
        startNode.group = group;
        nodeQueue.Enqueue (startNode);

        while (nodeQueue.Count > 0)
        {
            Node n = nodeQueue.Dequeue ();

            if (n.walkable)
            {
                List<Node> neighbours = GetNeighbours (n);

                for (int i = 0; i < neighbours.Count; i++)
                {
                    if (!neighbours[i].grouped)
                    {
                        neighbours[i].grouped = true;
                        neighbours[i].group = group;
                        nodeQueue.Enqueue (neighbours[i]);
                    }
                }
            }
        }

        group.members = nodeQueue;
        return group;
    }

    /// <summary>
    /// Finds neighbours of a given node.
    /// </summary>
    public List<Node> GetNeighbours ( Node node )
    {
        List<Node> neighbours = new List<Node> ();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && grid[checkX, checkY] != null)
                {
                    neighbours.Add (grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    /// <summary>
    /// Finds corresponding node from position.
    /// </summary>
    public Node NodeFromWorldPoint ( Vector3 worldPosition )
    {
        float precentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float precentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        precentX = Mathf.Clamp01 (precentX);
        precentY = Mathf.Clamp01 (precentY);

        int x = Mathf.RoundToInt ((gridSizeX - 1) * precentX);
        int y = Mathf.RoundToInt ((gridSizeY - 1) * precentY);
        return grid[x, y];
    }

    public void DisplayGrid ()
    {
        Gizmos.DrawWireCube (position, new Vector3 (gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null && displayGridGizmos)
        {
            UnityEngine.Debug.Log ("mjes");
            foreach (Node n in grid)
            {
                if (n.group != null)
                {
                    Random.InitState (n.group.id);
                }
                Gizmos.color = (n.walkable) ? (new Color (Random.Range (0f, 1f), Random.Range (1f, 2f) - 1, Random.Range (2f, 3f) - 2)) : Color.red;
                Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - (nodeRadius / 5)));
            }
        }
    }

    public void DisplayGridHandles ()
    {
        Vector3 pos = Vector3.zero;
        Vector2 displaySize = gridWorldSize * nodeRadius;

        Vector3[] verts = new Vector3[] { new Vector3(pos.x - displaySize.x,pos.y,pos.z-displaySize.y),
                                            new Vector3(pos.x-displaySize.x,pos.y,pos.z + displaySize.y),
                                            new Vector3(pos.x+displaySize.x,pos.y,pos.z + displaySize.y),
                                            new Vector3(pos.x+displaySize.x,pos.y,pos.z-displaySize.y) };
        UnityEditor.Handles.DrawSolidRectangleWithOutline (verts, new Color (0, 0, 0, 0f), new Color (0, 0, 0, 1));

        if (grid != null && displayGridGizmos)
        {
            UnityEngine.Debug.Log ("mjes");
            foreach (Node n in grid)
            {
                if (n.group != null)
                {
                    Random.InitState (n.group.id);
                }

                Color color = (n.walkable) ? (new Color (Random.Range (0f, 1f), Random.Range (1f, 2f) - 1, Random.Range (2f, 3f) - 2)) : Color.red;
                color.a = .2f;
                Vector3[] nodeVerts = new Vector3[] { new Vector3(n.worldPosition.x - nodeRadius,n.worldPosition.y,n.worldPosition.z-nodeRadius),
                                            new Vector3(n.worldPosition.x-nodeRadius,n.worldPosition.y,n.worldPosition.z + nodeRadius),
                                            new Vector3(n.worldPosition.x+nodeRadius,n.worldPosition.y,n.worldPosition.z + nodeRadius),
                                            new Vector3(n.worldPosition.x+nodeRadius,n.worldPosition.y,n.worldPosition.z-nodeRadius) };
                UnityEditor.Handles.DrawSolidRectangleWithOutline (nodeVerts, color, new Color (0, 0, 0, 1));

            }
        }
    }

    [System.Serializable]
    public class TerrayinType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}
