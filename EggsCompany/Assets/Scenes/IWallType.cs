using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//currently added for type safety
public interface IWallType : ITerrainType
{
    EWallType wallName
    {
        get;
    }
}
