using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Entitie, ISelectable
{
    public Rect hitbox
    {
        get
        {
            return EntitieRenderer.getEntitieHitbox (this);
        }
    }

    protected Vector2 windwoSize = new Vector2 (150, 100);
    public Vector2 uiSize
    {
        get
        {
            return windwoSize;
        }
    }

    protected Team buildingTeam;
    public Team team
    {
        get
        {
            return buildingTeam;
        }
    }

    protected override void Start ()
    {
        base.Start ();
    }

    public void SetTeam ( Team team )
    {
        buildingTeam = team;
    }

    //Return the size of the window.
    public virtual void BuildingUI ()
    {
        GUI.Button (new Rect (0, 0, windwoSize.x, 25), this.name);
    }
}
