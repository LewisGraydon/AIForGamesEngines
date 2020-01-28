using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFloorType : ITerrainType
{
    EFloorName floorName
    {
        get;
    }
}
