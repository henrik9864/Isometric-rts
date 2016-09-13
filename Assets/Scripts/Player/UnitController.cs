using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController
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

    public UnitController ( Team team )
    {
        this.team = team;
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
            Coroutines.instance.StopCoroutine (unit.Flock (pos));
            switch (type)
            {
                case MoveUnit.Point:
                    unit.Move (pos);
                    break;
                case MoveUnit.Translate:
                    unit.Move (pos + (-averageUnitPos + unit.transform.position));
                    break;
                case MoveUnit.Flock:
                    Coroutines.instance.StartCoroutine (unit.Flock (pos));
                    break;
                default:
                    break;
            }
        }
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
