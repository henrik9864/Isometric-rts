using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (Unit))]
public class UnitEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        DrawDefaultInspector ();

        Unit targetUnit = (Unit)target;

        GUIStyle style = new GUIStyle ();
        if (targetUnit.team != null)
        {
            GUILayout.Label ("Team: " + targetUnit.team.name, style);
        }
    }
}
