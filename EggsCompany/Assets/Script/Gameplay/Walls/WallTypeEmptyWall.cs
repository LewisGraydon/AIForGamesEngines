using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTypeEmpty : MonoBehaviour, IWallType
{

    public EWallName wallName = EWallName.Empty;
    private int moveCost = 0;
    public int coverValue = 0;
    public bool blocksSight = false;

    EWallName IWallType.wallName { get => wallName; }
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
