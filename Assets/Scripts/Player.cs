using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (CameraController), typeof (UnitController), typeof (UnitManager))]
public class Player : MonoBehaviour
{
    public Controls controls = new Controls ();
    public UI ui = new UI ();
    public float speed;

    CameraController cameraController;
    UnitController unitController;
    UnitManager unitManager;

    Vector3 selectStart;
    Rect selsectBox;

    Team debugTeam;
    public Team team
    {
        get
        {
            return debugTeam;
        }
    }

    bool debug;

    void Start ()
    {
        cameraController = GetComponent<CameraController> ();
        unitController = GetComponent<UnitController> ();
        unitManager = GetComponent<UnitManager> ();

        debugTeam = Team.CreateTeam ("PlayerTeam", unitManager);
        unitController.SetTeam (debugTeam);
    }

    void Update ()
    {
        ControlCamera ();

        UnitSelection ();

        if (debug)
        {
            Debug ();
        }

        if (Input.GetKeyDown (KeyCode.F12))
        {
            debug = !debug;
        }
    }

    void OnGUI ()
    {
        GUIStyle style = new GUIStyle ();
        style.border = new RectOffset (2, 2, 2, 2);
        style.normal.background = ui.boxSelector;

        GUI.Box (selsectBox, "", style);

        if (debug && debugTeam != null)
        {
            GUI.Box (new Rect (20, 20, 300, 70), "");

            if (GUI.Button (new Rect (30, 30, 100, 20), debugTeam.id.ToString ()))
            {
                unitController.DeselectAllUnits ();
                debugTeam = Team.GetTeam (debugTeam.id + 1);
                if (debugTeam == null)
                {
                    debugTeam = Team.GetTeam (0);
                }
            }
            if (GUI.Button (new Rect (30, 60, 100, 20), "Make new team."))
            {
                Team.CreateTeam ("Test: " + (debugTeam.id + 1).ToString (), unitManager);
            }
        }
    }

    void ControlCamera ()
    {
        if (Input.GetKey (controls.forward))
        {
            cameraController.Move (-transform.forward, speed);
        }
        if (Input.GetKey (controls.backward))
        {
            cameraController.Move (transform.forward, speed);
        }
        if (Input.GetKey (controls.right))
        {
            cameraController.Move (transform.right, speed);
        }
        if (Input.GetKey (controls.left))
        {
            cameraController.Move (-transform.right, speed);
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
                        unitController.ToggleUnit (unit);
                        deselectUnits = false;
                    }
                }

                if (deselectUnits)
                {
                    unitController.DeselectAllUnits ();
                }
            }
            else
            {
                unitController.SelectUnitsFromRect (selsectBox);
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
                unitController.MoveUnits (hit.point, MoveUnit.Translate);
            }
        }
    }

    void Debug ()
    {
        if (Input.GetKeyDown (KeyCode.F1))
        {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit, Mathf.Infinity))
            {
                unitController.SpawnUnit (hit.point, Resources.Load<Unit> ("Unit")).SetTeam (debugTeam); // SPawns a unit and forces it to be the custom player team.
            }
        }
    }
}

[System.Serializable]
public class UI
{
    public Texture2D boxSelector;
}

public enum PlayerState
{
    Controlling,
    Building,
}