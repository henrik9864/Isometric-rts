using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

[RequireComponent (typeof (Grid))]
[RequireComponent (typeof (PathRequestManager))]
public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;
    Grid grid;

    void Awake ()
    {
        requestManager = GetComponent<PathRequestManager> ();
        grid = new Grid ();
        grid.Init ();
    }

    public void StartFindPath ( Vector3 startPos, Vector3 targetPos )
    {
        StartCoroutine (FindPath (startPos, targetPos));
    }

    public Vector3[] CalculatePath ( Vector3 startPos, Vector3 targetPos )
    {
        Stopwatch sw = new Stopwatch ();
        sw.Start ();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint (startPos);
        Node targetNode = grid.NodeFromWorldPoint (targetPos);

        if (startNode.walkable && targetNode.walkable && startNode.group == targetNode.group)
        {
            Heap<Node> openSet = new Heap<Node> (grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node> ();

            openSet.Add (startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.removeFIrstItem ();

                closedSet.Add (currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop ();
                    print ("Path found: " + sw.ElapsedMilliseconds + "ms.");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours (currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains (neighbour))
                    {
                        continue;
                    }

                    int newMovmentCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour) + neighbour.movmentPenalty;
                    if (newMovmentCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour))
                    {
                        neighbour.gCost = newMovmentCostToNeighbour;
                        neighbour.hCost = GetDistance (neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains (neighbour))
                        {
                            openSet.Add (neighbour);
                            openSet.UpdateItem (neighbour);
                        }
                    }
                }
            }
        }
        if (pathSuccess)
        {
            waypoints = RetracePath (startNode, targetNode);
            return waypoints;
        }
        return null;
    }

    /// <summary>
    /// Start calculating a new path over multiple frames.
    /// </summary>
    IEnumerator FindPath ( Vector3 startPos, Vector3 targetPos )
    {
        Stopwatch sw = new Stopwatch ();
        sw.Start ();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint (startPos);
        Node targetNode = grid.NodeFromWorldPoint (targetPos);

        print (startNode.worldPosition);

        if (startNode.walkable && targetNode.walkable && startNode.group == targetNode.group)
        {
            Heap<Node> openSet = new Heap<Node> (grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node> ();

            openSet.Add (startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.removeFIrstItem ();

                closedSet.Add (currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop ();
                    print ("Path found: " + sw.ElapsedMilliseconds + "ms.");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours (currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains (neighbour) || Mathf.Abs (currentNode.worldPosition.y - neighbour.worldPosition.y) > 0.2f)
                    {
                        continue;
                    }

                    int newMovmentCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour) + neighbour.movmentPenalty;
                    if (newMovmentCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour))
                    {
                        neighbour.gCost = newMovmentCostToNeighbour;
                        neighbour.hCost = GetDistance (neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains (neighbour))
                        {
                            openSet.Add (neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem (neighbour);
                        }
                    }
                }
            }
            yield return null;
        }
        if (pathSuccess)
        {
            waypoints = RetracePath (startNode, targetNode);
        }
        requestManager.FinishedProcessingPath (waypoints, pathSuccess);
    }

    /// <summary>
    /// Finds path for finished A* calculations.
    /// </summary>
    /// <returns>Simplefid version of path.</returns>
    Vector3[] RetracePath ( Node startNode, Node endNode )
    {
        List<Node> path = new List<Node> ();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add (currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplefyPath (path);
        Array.Reverse (waypoints);
        return waypoints;
    }

    /// <summary>
    /// Reduces amount of waypoints in path to only corners.
    /// </summary>
    /// <param name="path">Path to simplefy.</param>
    /// <returns>Simplefied path.</returns>
    Vector3[] SimplefyPath ( List<Node> path )
    {
        List<Vector3> waypoints = new List<Vector3> ();
        Vector3 directionOld = Vector3.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector3 directionNew = new Vector3 (path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY, path[i - 1].worldPosition.y - path[i].worldPosition.y).normalized;

            if (directionNew != directionOld)
            {
                waypoints.Add (path[i].worldPosition);
                directionOld = directionNew;
            }
        }

        return waypoints.ToArray ();
    }

    /// <summary>
    /// Find distance between to nodes.
    /// </summary>
    /// <returns>Distance.</returns>
    int GetDistance ( Node nodeA, Node nodeB )
    {
        int distX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }

    void OnDrawGizmos ()
    {
        if (grid != null)
        {
            grid.DisplayGrid ();
        }
    }

}
