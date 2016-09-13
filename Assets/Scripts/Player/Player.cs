using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (CameraController), typeof (UnitController), typeof (UnitManager))]
[RequireComponent (typeof (PlayerUI))]
public class Player : MonoBehaviour
{
    public Controls controls = new Controls ();
    public UI ui = new UI ();
    public float speed;
    public bool enableDebug;

    float native_width = Screen.width;
    float native_height = Screen.height;

    // Enable this resolution when building.
    //float native_width = 960;
    //float native_height = 540;

    CameraController cameraController;
    UnitController unitController;
    UnitManager unitManager;
    PlayerUI playerUi;

    Vector3 selectStart;
    Rect selsectBox;
    bool selecting;

    UiOptions uiOptions = UiOptions.Units;
    int uiOptionsIndex = 0;

    Team debugTeam;
    public Team team
    {
        get
        {
            return debugTeam;
        }
    }

    bool debug;
    string debugUnitName = "Unit";

    void Start ()
    {
        cameraController = GetComponent<CameraController> ();
        unitController = GetComponent<UnitController> ();
        unitManager = GetComponent<UnitManager> ();
        playerUi = GetComponent<PlayerUI> ();

        debugTeam = Team.CreateTeam ("PlayerTeam", unitManager);
        unitController.SetTeam (debugTeam);

        debug = enableDebug;
    }

    void Update ()
    {
        ControlCamera ();

        if (debug)
        {
            Debug ();
        }
        else
        {
            UnitSelection ();
        }
    }

    void OnGUI ()
    {
        playerUi.ClearHitboxes ();

        GUIStyle style = new GUIStyle ();
        style.border = new RectOffset (2, 2, 2, 2);
        style.normal.background = ui.boxSelector;

        GUI.Box (selsectBox, "", style);

        /*if (debug && debugTeam != null)
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
        }*/

        Ui ();
    }

    void Ui ()
    {
        //Scales ui to fit screen, kinda knows how it works
        float rx = Screen.width / native_width;
        float ry = Screen.height / native_height;

        Matrix4x4 matrix = Matrix4x4.TRS (new Vector3 (0, 0, 0), Quaternion.identity, new Vector3 (rx, ry, 1));

        GUI.matrix = matrix;
        playerUi.matrix = matrix;

        GUIStyle style = new GUIStyle ();
        style.border = new RectOffset (4, 4, 4, 4);
        style.normal.background = ui.sideBar;

        playerUi.Box (new Rect (0, 150, 150, native_height - 150), "", style);
        style.normal.background = ui.sideBarTriangle;
        playerUi.Box (new Rect (148, 150, 50, 50), "", style);
        style.normal.background = ui.box;
        playerUi.Box (new Rect (0, 0, 200, 154), "", style);

        GUI.Button (new Rect (30, 160, 90, 20), System.Enum.GetName (typeof (UiOptions), uiOptionsIndex));
        if (GUI.Button (new Rect (10, 160, 20, 20), "<"))
        {
            if (uiOptionsIndex > 0)
            {
                uiOptionsIndex--;
                uiOptions = (UiOptions)uiOptionsIndex;
            }
        }
        else if (GUI.Button (new Rect (120, 160, 20, 20), ">"))
        {
            if (uiOptionsIndex < System.Enum.GetNames (typeof (UiOptions)).Length - 1)
            {
                uiOptionsIndex++;
                uiOptions = (UiOptions)uiOptionsIndex;
            }
        }

        switch (uiOptions)
        {
            case UiOptions.Units:
                Units ();
                break;
            case UiOptions.Building:
                break;
            case UiOptions.Debug:
                UiDebug ();
                break;
            default:
                break;
        }
    }

    void Units ()
    {
        for (int i = 0; i < unitController.units.Count; i++)
        {
            if (GUI.Button (new Rect (10, 190 + 30 * i, 130, 20), unitController.units[i].name))
            {
                unitController.DeselectUnit (unitController.units[i]);
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
            Vector3 mousePos = Input.mousePosition;
            if (playerUi.IsValid (new Vector2 (mousePos.x, Screen.height - mousePos.y)))
            {
                selectStart = mousePos;
                selecting = true;
            }
        }
        if (Input.GetMouseButton (controls.rightClick) && selecting)
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

            if ((mousePos - selectStart).magnitude < 1 && playerUi.IsValid (new Vector2 (mousePos.x, Screen.height - mousePos.y)))
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
            else if (selecting)
            {
                unitController.SelectUnitsFromRect (selsectBox);
                selectStart = Vector3.zero;
                selsectBox = new Rect ();
            }
            selecting = false;
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
        if (Input.GetMouseButtonDown (0))
        {
            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast (ray, out hit, Mathf.Infinity))
            {
                unitController.SpawnUnit (hit.point, Resources.Load<Unit> (debugUnitName)).SetTeam (debugTeam); // SPawns a unit and forces it to be the custom player team.
            }
        }
    }

    void UiDebug ()
    {
        debug = GUI.Toggle (new Rect (10, 190, 130, 20), debug, "Debug");

        debugUnitName = GUI.TextField (new Rect (10, 210, 130, 20), debugUnitName);

        if (GUI.Button (new Rect (10, 240, 130, 20), "Debug team id: " + debugTeam.id.ToString ()))
        {
            unitController.DeselectAllUnits ();
            debugTeam = Team.GetTeam (debugTeam.id + 1);
            if (debugTeam == null)
            {
                debugTeam = Team.GetTeam (0);

            }
        }
        if (GUI.Button (new Rect (10, 270, 130, 20), "Make new team."))
        {
            Team.CreateTeam ("Test: " + (debugTeam.id + 1).ToString (), unitManager);
        }
    }
}

[System.Serializable]
public class UI
{
    public Texture2D boxSelector;
    public Texture2D sideBar;
    public Texture2D sideBarTriangle;
    public Texture2D box;
}

public enum PlayerState
{
    Controlling,
    Building,
}

public enum UiOptions
{
    Units,
    Building,
    Debug,
}