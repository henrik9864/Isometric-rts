using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector
{
    public static List<ISelectable> GetSelectables ( Rect area )
    {
        List<ISelectable> selectables = new List<ISelectable> ();
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject> ();

        foreach (GameObject obj in objects)
        {
            ISelectable selectable = obj.GetComponent<ISelectable> ();

            if (selectable != null && area.Contains (selectable.hitbox.center))
            {
                selectables.Add (selectable);
            }
        }

        return selectables;
    }

    public static List<ISelectable> GetSelectables ( Vector2 point )
    {
        List<ISelectable> selectables = new List<ISelectable> ();
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject> ();

        foreach (GameObject obj in objects)
        {
            ISelectable selectable = obj.GetComponent<ISelectable> ();

            if (selectable != null && selectable.hitbox.Contains (point))
            {
                selectables.Add (selectable);
            }
        }

        return selectables;
    }
}