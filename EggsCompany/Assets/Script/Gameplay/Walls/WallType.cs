using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallType : MonoBehaviour, ITerrainType
{
    public int moveCost;
    public int coverValue;
    public bool blocksSight;
    public EWallName wallName;

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
