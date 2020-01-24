using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITerrainType
{
    ETerrainType terrainName
    {
        get;
    }
    int moveCost
    {
        get;
    }

    int coverValue
    {
        get;
    }
    
    bool blocksSight
    {
        get;
    }



}
