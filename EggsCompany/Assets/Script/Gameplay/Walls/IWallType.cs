using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWallType : ITerrainType
{
    EWallName wallName
    {
        get;
    }
}
