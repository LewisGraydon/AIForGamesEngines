using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTypeEmpty : WallType
{
    new private int moveCost = 0;
    new public int coverValue = 0;
    new public bool blocksSight = false;
    public EWallName wallName = EWallName.Empty;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
