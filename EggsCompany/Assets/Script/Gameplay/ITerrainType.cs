using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITerrainType
{
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
