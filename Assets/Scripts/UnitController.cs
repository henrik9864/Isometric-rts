using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : MonoBehaviour
{

    List<Unit> selectedUnits = new List<Unit> ();
    public List<Unit> units
    {
        get
        {
            return selectedUnits;
        }
    }

    Team team;

    public void SelectUnitsFromRect ( Rect selectionBox )
    {
        Unit[] units = FindObjectsOfType<Unit> ();

        foreach (Unit unit in units)
        {
            if (unit.team.id == team.id && selectionBox.Contains (EntitieRenderer.getEntitieHitbox (unit).center))
            {
                SelectUnit (unit);
            }
        }
    }

    public void SelectUnit ( Unit unit )
    {
        if (unit.team == team && selectedUnits.Find (( a ) => { return unit == a; }) == null)
        {
            selectedUnits.Add (unit);
            unit._onKilled += SelectedUnitDead;
            unit.ToggleHighlight (true);
        }
    }

    public void DeselectUnit ( Unit unit )
    {
        unit.ToggleHighlight (false);
        selectedUnits.Remove (unit);
    }

    public void DeselectAllUnits ()
    {
        foreach (Unit unit in selectedUnits)
        {
            unit.ToggleHighlight (false);
        }
        selectedUnits.RemoveRange (0, selectedUnits.Count);
    }

    public void ToggleUnit ( Unit unit )
    {
        if (unit.team == team)
        {
            if (selectedUnits.Find (( a ) => { return unit == a; }) == null)
            {
                selectedUnits.Add (unit);
                unit._onKilled += SelectedUnitDead;
                unit.ToggleHighlight (true);
            }
            else
            {
                selectedUnits.Remove (unit);
                unit.ToggleHighlight (false);
            }
        }
    }

    public void MoveUnits ( Vector3 pos, MoveUnit type )
    {
        Vector3 averageUnitPos = Vector3.zero;
        foreach (Unit unit in selectedUnits)
        {
            averageUnitPos += unit.transform.position;
        }
        averageUnitPos /= selectedUnits.Count;

        foreach (Unit unit in selectedUnits)
        {
            StopCoroutine (unit.Flock (pos));
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

    public void SetTeam ( Team team )
    {
        this.team = team;
    }

    public Unit SpawnUnit ( Vector3 pos, Unit unit )
    {
        GameObject obj = (GameObject)Instantiate (unit.gameObject, pos, Quaternion.identity);
        Unit unitObj = obj.GetComponent<Unit> ();
        unitObj.SetTeam (team);
        return unitObj;
    }

    void SelectedUnitDead ( Unit unit )
    {
        selectedUnits.Remove (unit);
    }
}

public enum MoveUnit
{
    Point,
    Translate,
    Flock,
}
