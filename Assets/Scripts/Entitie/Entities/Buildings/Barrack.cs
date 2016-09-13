using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrack : Building
{
    [SerializeField]
    List<Unit> canTrain = new List<Unit> ();

    protected override void Start ()
    {
        base.Start ();
    }

    public override void BuildingUI ()
    {
        base.BuildingUI ();

        if (GUI.Button (new Rect (10, 35, 75, 25), "Train unit") && buildingTeam != null)
        {
            team.SpawnUnit (transform.position, canTrain[0]);
        }
    }
}
