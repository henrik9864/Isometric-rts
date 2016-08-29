using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (CameraController))]
public class Player : MonoBehaviour
{
    public Controls controls = new Controls ();
    public UI ui = new UI ();
    public float speed;

    [SerializeField]
    List<Unit> selectedUnits = new List<Unit> ();

    CameraController controller;
    Vector3 selectStart;
    Rect selsectBox;

    void Start ()
    {
        controller = GetComponent<CameraController> ();
    }

    void Update ()
    {
        ControlCamera ();

        UnitSelection ();
    }

    void OnGUI ()
    {
        GUIStyle style = new GUIStyle ();
        style.border = new RectOffset (2, 2, 2, 2);
        style.normal.background = ui.boxSelector;

        GUI.Box (selsectBox, "", style);
    }

    void ControlCamera ()
    {
        if (Input.GetKey (controls.forward))
        {
            controller.Move (-transform.forward, speed);
        }
        if (Input.GetKey (controls.backward))
        {
            controller.Move (transform.forward, speed);
        }
        if (Input.GetKey (controls.right))
        {
            controller.Move (transform.right, speed);
        }
        if (Input.GetKey (controls.left))
        {
            controller.Move (-transform.right, speed);
        }
    }

    void UnitSelection ()
    {
        if (Input.GetMouseButtonDown (controls.rightClick))
        {
            selectStart = Input.mousePosition;
        }
        if (Input.GetMouseButton (controls.rightClick))
        {
            Vector3 mousePos = Input.mousePosition;

            if ((mousePos - selectStart).magnitude > 1)
            {
                if (mousePos.x > selectStart.x)
                {
                    selsectBox.width = -(selectStart.x - mousePos.x);
                    selsectBox.x = selectStart.x;
                }
                else
                {
                    selsectBox.width = (selectStart.x - mousePos.x);
                    selsectBox.x = mousePos.x;
                }

                if (mousePos.y < selectStart.y)
                {
                    selsectBox.height = (selectStart.y - mousePos.y);
                    selsectBox.y = Screen.height - selectStart.y;
                }
                else
                {
                    selsectBox.height = -(selectStart.y - mousePos.y);
                    selsectBox.y = Screen.height - mousePos.y;
                }
            }
        }
        if (Input.GetMouseButtonUp (controls.rightClick))
        {
            Vector3 mousePos = Input.mousePosition;

            if ((mousePos - selectStart).magnitude < 1)
            {
                Unit[] units = FindObjectsOfType<Unit> ();
                bool deselectUnits = true;

                foreach (Unit unit in units)
                {
                    Rect hitbox = EntitieRenderer.getEntitieHitbox (unit);
                    if (hitbox.Contains (new Vector2 (mousePos.x, Screen.height - mousePos.y)))
                    {
                        ToggleUnit (unit);
                        deselectUnits = false;
                    }
                }

                if (deselectUnits)
                {
                    selectedUnits.RemoveRange (0, selectedUnits.Count);
                }
            }
            else
            {
                SelectUnitsFromRect (selsectBox);
                selectStart = Vector3.zero;
                selsectBox = new Rect ();
            }
        }
        if (Input.GetMouseButtonUp (controls.leftClick))
        {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit, Mathf.Infinity))
            {
                MoveUnits (hit.point, MoveUnit.Flock);
            }
        }
    }

    void SelectUnitsFromRect ( Rect selectionBox )
    {
        Unit[] units = FindObjectsOfType<Unit> ();

        foreach (Unit unit in units)
        {
            if (selectionBox.Contains (EntitieRenderer.getEntitieHitbox (unit).center))
            {
                SelectUnit (unit);
            }
        }
    }

    void SelectUnit ( Unit unit )
    {
        if (selectedUnits.Find (( a ) => { return unit == a; }) == null)
        {
            selectedUnits.Add (unit);
            unit.ToggleHighlight (true);
        }
    }

    void ToggleUnit ( Unit unit )
    {
        if (selectedUnits.Find (( a ) => { return unit == a; }) == null)
        {
            selectedUnits.Add (unit);
            unit.ToggleHighlight (true);
        }
        else
        {
            selectedUnits.Remove (unit);
            unit.ToggleHighlight (false);
        }
    }

    void MoveUnits ( Vector3 pos, MoveUnit type )
    {
        Vector3 averageUnitPos = Vector3.zero;
        foreach (Unit unit in selectedUnits)
        {
            averageUnitPos += unit.transform.position;
        }
        averageUnitPos /= selectedUnits.Count;

        foreach (Unit unit in selectedUnits)
        {
            switch (type)
            {
                case MoveUnit.Point:
                    unit.Move (pos);
                    break;
                case MoveUnit.Translate:
                    unit.Move (pos + (-averageUnitPos + unit.transform.position));
                    break;
                case MoveUnit.Flock:
                    StartCoroutine (unit.Flock (pos));
                    break;
                default:
                    break;
            }
        }
    }
}

[System.Serializable]
public class UI
{
    public Texture2D boxSelector;
}

public enum MoveUnit
{
    Point,
    Translate,
    Flock,
}