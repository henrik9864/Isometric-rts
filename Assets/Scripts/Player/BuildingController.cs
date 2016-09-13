using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController
{
    Building selectedBuilding;
    public Building building
    {
        get
        {
            return selectedBuilding;
        }
    }

    public void SelectBuilding ( Building building )
    {
        selectedBuilding = building;
    }

    public void DeselectBuilding ()
    {
        selectedBuilding = null;
    }

    public void ToggleBuilding ( Building building )
    {
        if (selectedBuilding == building)
        {
            selectedBuilding = null;
        }
        else
        {
            selectedBuilding = building;
        }
    }
}
