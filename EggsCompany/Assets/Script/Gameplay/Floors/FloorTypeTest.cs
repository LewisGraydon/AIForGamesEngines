using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTypeTest : FloorType, ITerrainType
{
    new private int moveCost = 0;
    new public int coverValue = 0;
    new public bool blocksSight = false;
    new private EFloorName floorName = EFloorName.Empty;
    

    //int ITerrainType.moveCost => moveCost;
    //int ITerrainType.coverValue => coverValue;
    //bool ITerrainType.blocksSight => blocksSight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
