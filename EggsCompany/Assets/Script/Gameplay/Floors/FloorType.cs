using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorType : MonoBehaviour, ITerrainType
{
    public int moveCost;
    public int coverValue;
    public bool blocksSight;
    public EFloorName floorName;

    int ITerrainType.moveCost { get => moveCost; }
    int ITerrainType.coverValue { get => coverValue; }
    bool ITerrainType.blocksSight { get => blocksSight; }

    private void Start()
    {
        
    }
    private void Update()
    {
        
    }
}
