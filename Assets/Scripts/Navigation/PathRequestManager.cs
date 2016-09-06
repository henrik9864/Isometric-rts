using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathRequestManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest> ();
    PathRequest currentPathRequest;

    static PathRequestManager instance;

    protected Pathfinding pathfinding;
    bool isProcessingPath;

    void Awake ()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding> ();
    }

    /// <summary>
    /// Calculate a path over mutiple frames.
    /// </summary>
    /// <param name="pathStart">Start.</param>
    /// <param name="pathEnd">End.</param>
    /// <param name="callback">Method to call when calculations is compleete.</param>
    public static void RequestPath ( Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback )
    {
        PathRequest newReuest = new PathRequest (pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue (newReuest);
        instance.TryProcessNext ();
    }

    /// <summary>
    /// Calculate a path this frame.
    /// </summary>
    /// <param name="pathStart">Start.</param>
    /// <param name="pathEnd">End.</param>
    /// <returns>Path.</returns>
    public static Vector3[] CalcualtePath ( Vector3 pathStart, Vector3 pathEnd )
    {
        return instance.pathfinding.CalculatePath (pathStart, pathEnd);
    }

    /// <summary>
    /// Calculate path distance from start to end.
    /// </summary>
    /// <returns>Distance.</returns>
    public static float GetPathDistance ( Vector3[] path )
    {
        float pathLength = 0f;
        for (int i = 0; i < path.Length - 1; i++)
        {
            pathLength += Vector3.Distance (path[i], path[i + 1]);
        }

        return pathLength;
    }

    /// <summary>
    /// Distributes calculation of new path's. 1 per frame.
    /// </summary>
    void TryProcessNext ()
    {
        if (!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue ();
            isProcessingPath = true;
            pathfinding.StartFindPath (currentPathRequest.pathStart, currentPathRequest.pathEnd);
            print ("mjesi");
        }
    }

    /// <summary>
    /// Path calculations is compleete.
    /// </summary>
    /// <param name="path">Calculated path.</param>
    /// <param name="success">Was it successful.</param>
    public void FinishedProcessingPath ( Vector3[] path, bool success )
    {
        currentPathRequest.callback (path, success);
        isProcessingPath = false;
        TryProcessNext ();
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest ( Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback )
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
