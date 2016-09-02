using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team
{
    static List<Team> teamList = new List<Team> ();
    static int teamCount = 0;
    public static int teams
    {
        get
        {
            return teamCount;
        }
    }

    List<Unit> unitsInTeam = new List<Unit> ();
    public List<Unit> units
    {
        get
        {
            return unitsInTeam;
        }
    }

    UnitManager unitManager;
    public UnitManager manager
    {
        get
        {
            return unitManager;
        }
    }

    string teamName;
    public string name
    {
        get
        {
            return teamName;
        }
    }

    int teamId;
    public int id
    {
        get
        {
            return teamId;
        }
    }

    protected Team ( int id, string name, UnitManager manager )
    {
        teamId = id;
        unitManager = manager;
        teamName = name;

        unitManager.SetOwner (this);
    }

    public void RegisterUnit ( Unit unit )
    {
        unitsInTeam.Add (unit);
    }

    public void UnregisterUnit ( Unit unit )
    {
        unitsInTeam.Remove (unit);
    }

    public static Team CreateTeam ( string name, UnitManager manager )
    {
        Team team = new Team (teamCount, name, manager);
        teamCount++;
        teamList.Add (team);

        GameObject obj = new GameObject (name);
        obj.AddComponent<UnitManager> ().SetOwner (team);

        return team;
    }

    public static Team[] GetTeams ()
    {
        return teamList.ToArray ();
    }

    public static Team GetTeam ( int id )
    {
        return teamList.Find (a => { return a.id == id; });
    }

    public static Team GetTeam ( Team team )
    {
        return teamList.Find (a => { return a == team; });
    }
}
