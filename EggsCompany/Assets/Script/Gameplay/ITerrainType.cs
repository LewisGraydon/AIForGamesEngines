using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface that contains fields common to all traverasble terrain.
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
