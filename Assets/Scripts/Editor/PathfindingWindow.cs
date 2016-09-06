using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PathfindingWindow : EditorWindow
{
    int unwalkableMask = 0;

    Grid grid;

    [MenuItem ("Window/Pathfinding")]
    public static void ShowWindow ()
    {
        GetWindow (typeof (PathfindingWindow));
    }

    void OnEnable ()
    {
        grid = new Grid ();

        //SceneView.onSceneGUIDelegate -= OnSceneGUI;
        //SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnGUI ()
    {
        unwalkableMask = EditorGUILayout.LayerField (unwalkableMask);

        if (GUI.Button (new Rect (10, position.height - 40, position.width - 20, 20), "Bake"))
        {
            grid.unwalkableMask = unwalkableMask;
            grid.Init ();
        }
    }

    void OnSceneGUI ( SceneView view )
    {
        grid.DisplayGridHandles ();
    }
}
