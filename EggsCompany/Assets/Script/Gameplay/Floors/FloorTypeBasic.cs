using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTypeBasic : MonoBehaviour, IFloorType
{
    public EFloorName floorName = EFloorName.Empty;
    private int moveCost = 0;
    public int coverValue = 0;
    public bool blocksSight = false;

    EFloorName IFloorType.floorName { get => floorName; }
    int ITerrainType.moveCost { get => moveCost; }
    int ITerrainType.coverValue { get => coverValue; }
    bool ITerrainType.blocksSight { get => blocksSight; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
