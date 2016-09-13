using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [HideInInspector]
    public Matrix4x4 matrix;

    List<Rect> hitboxes = new List<Rect> ();

    public bool IsValid ( Vector2 pos )
    {
        foreach (Rect hitbox in hitboxes)
        {
            if (hitbox.Contains (pos))
            {
                return false;
            }
        }
        return true;
    }

    public void Box ( Rect box, string text, GUIStyle style )
    {
        Vector3 size = matrix.MultiplyPoint (new Vector3 (box.size.x, box.size.y, 0));
        Vector3 pos = matrix.MultiplyPoint (new Vector3 (box.position.x, box.position.y, 0));

        Rect hitbox = new Rect (new Vector2 (pos.x, pos.y), new Vector2 (size.x, size.y));

        hitboxes.Add (hitbox);
        GUI.Box (box, text, style);
    }

    public void Box ( Rect box, string text )
    {
        Vector3 size = matrix.MultiplyPoint (new Vector3 (box.size.x, box.size.y, 0));
        Vector3 pos = matrix.MultiplyPoint (new Vector3 (box.position.x, box.position.y, 0));

        Rect hitbox = new Rect (new Vector2 (pos.x, pos.y), new Vector2 (size.x, size.y));

        hitboxes.Add (hitbox);
        GUI.Box (box, text);
    }

    public bool Button ( Rect box, string text, GUIStyle style )
    {
        Vector3 size = matrix.MultiplyPoint (new Vector3 (box.size.x, box.size.y, 0));
        Vector3 pos = matrix.MultiplyPoint (new Vector3 (box.position.x, box.position.y, 0));

        Rect hitbox = new Rect (new Vector2 (pos.x, pos.y), new Vector2 (size.x, size.y));

        hitboxes.Add (hitbox);
        if (GUI.Button (box, text, style))
        {
            return true;
        }
        return false;
    }

    public bool Button ( Rect box, string text )
    {
        Vector3 size = matrix.MultiplyPoint (new Vector3 (box.size.x, box.size.y, 0));
        Vector3 pos = matrix.MultiplyPoint (new Vector3 (box.position.x, box.position.y, 0));

        Rect hitbox = new Rect (new Vector2 (pos.x, pos.y), new Vector2 (size.x, size.y));

        hitboxes.Add (hitbox);
        if (GUI.Button (box, text))
        {
            return true;
        }
        return false;
    }

    public void ClearHitboxes ()
    {
        hitboxes.Clear ();
        matrix = new Matrix4x4 ();
    }
}
