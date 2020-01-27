using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //Currently working on the assumption that we are hand designing the levels
    //GameObject is placeholder for egg or terrain object
    private GameObject occupier;
    public ITerrainType terrainType;
    private Tile[] neighbors = new Tile[4];

    private IWallType[] walls = new IWallType[4];
    //public IWallType nWall = EWallType.Empty;
    //public IWallType eWall = EWallType.Empty;
    //public IWallType sWall = EWallType.Empty;
    //public IWallType wWall = EWallType.Empty;

    // Start is called before the first frame update
    void Start()
    {





        //walls[(int)EDirection.North] = nWall;
        //walls[(int)EDirection.East] = eWall;
        //walls[(int)EDirection.South] = sWall;
        //walls[(int)EDirection.West] = wWall;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignNeighbor(EDirection direction, Tile nieghborTile)
    {
        neighbors[(int)direction] = nieghborTile;
    }

    public int CalcMoveCost(EDirection direction)
    {
        return terrainType.moveCost + walls[(int)direction].moveCost;
    }


}
