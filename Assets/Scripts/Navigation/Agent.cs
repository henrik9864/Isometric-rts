using UnityEngine;
using System.Collections;

public delegate void AgentEventHandler ();
public class Agent : MonoBehaviour
{
    //Testing
    public Transform target;

    public float speed;
    public bool hasPath
    {
        get
        {
            return (path != null);
        }
    }

    public float remainingDistance
    {
        get
        {
            if (path != null)
            {
                return GetDistance (path, targetIndex);
            }
            return -1;
        }
    }

    public event AgentEventHandler _pathComplete; // When agent has reach the destination.

    Vector3[] path;
    int targetIndex;

    void Start ()
    {
        SetDestination (target.position);
        //SetPath (PathRequestManager.CalcualtePath (transform.position, target.position));
    }

    void OnPathFound ( Vector3[] newPath, bool pathSuccessful )
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine ("FollowPath");
            StartCoroutine ("FollowPath");
        }
    }

    IEnumerator FollowPath ()
    {
        Vector3 currentWaypoint = path[0];
        targetIndex = 0;

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    if (_pathComplete != null)
                    {
                        _pathComplete.Invoke ();
                    }
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);
            transform.LookAt (currentWaypoint);
            yield return null;
        }
    }

    void OnDrawGizmosSelected ()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube (path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine (transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine (path[i - 1], path[i]);
                }
            }
        }
    }

    /// <summary>
    /// Set agent destination.
    /// </summary>
    /// <param name="destination">Destination position.</param>
    public void SetDestination ( Vector3 destination )
    {
        PathRequestManager.RequestPath (transform.position, destination, OnPathFound);
    }

    /// <summary>
    /// Assign this agent a precalculated path.
    /// </summary>
    /// <param name="_path"></param>
    public void SetPath ( Vector3[] _path )
    {
        path = _path;
        StopCoroutine ("FollowPath");
        StartCoroutine ("FollowPath");
    }

    /// <summary>
    /// Calculate distance from a given index.
    /// </summary>
    /// <param name="path">Path to calculate distance from.</param>
    /// <param name="targetIndex">Where to calculate distance from.</param>
    /// <returns>Path length.</returns>
    float GetDistance ( Vector3[] path, int targetIndex )
    {
        float pathLength = 0f;
        for (int i = targetIndex; i < path.Length - 1; i++)
        {
            pathLength += Vector3.Distance (path[i], path[i + 1]);
        }

        if (targetIndex < path.Length)
        {
            pathLength += Vector3.Distance (transform.position, path[targetIndex]);
        }

        return pathLength;
    }
}

/*
Todo

Find isolated clusters in the grid.
Add weights.
*/
