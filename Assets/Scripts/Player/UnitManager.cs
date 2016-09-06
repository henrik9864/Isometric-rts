using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{

    [Tooltip ("Scans per second.")]
    public float Sps = 20;

    Team owner;

    float timeToNextScan;
    Team toScan;

    void Update ()
    {
        if (timeToNextScan == 0)
        {
            toScan = Team.GetTeam (0);
        }

        if (timeToNextScan < Time.time)
        {
            timeToNextScan = Time.time + 1 / Sps;

            toScan = Team.GetTeam (toScan.id + 1);
            if (toScan == null)
            {
                toScan = Team.GetTeam (0);
            }

            if (toScan != owner)
            {
                ScanUnits (toScan.units);
            }
        }
    }

    void ScanUnits ( List<Unit> units )
    {
        foreach (Unit unit in owner.units)
        {
            foreach (Unit target in units)
            {
                if (unit != null && target != null)
                {
                    if (unit.team != target.team && Vector3.Distance (unit.transform.position, target.transform.position) <= unit.provokeRange)
                    {
                        StartCoroutine (unit.Attack (target));
                        break;
                    }
                }
            }
        }
    }

    public void SetOwner ( Team owner )
    {
        this.owner = owner;
    }
}
