﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum coverValue
{
    None,
    Half,
    Full
}

public class Tile : MonoBehaviour
{
    //Currently working on the assumption that we are hand designing the levels
    //GameObject is placeholder for egg or terrainBlocker object
    public GameObject occupier = null;
    public FloorType terrainType;
    private List<Tile> neighbors = new List<Tile>(4);

    private List<WallType> walls = new List<WallType>(4);
    public WallType nWall;
    public WallType eWall;
    public WallType sWall;
    public WallType wWall;

    // Start is called before the first frame update
    void Awake()
    {
        //Order here is important to align with direction
        walls.Add(nWall);
        walls.Add(eWall);
        walls.Add(sWall);
        walls.Add(wWall);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignNeighbor(EDirection direction, Tile neighborTile)
    {
        neighbors.Insert((int)direction, neighborTile);
        //neighbors[(int)direction] = nieghborTile;
    }

    //Adds empty count for list
    public void NeighborListFill()
    {
        for(int i = 0; i < 4; i++)
        {
            neighbors.Add(null);
        }
    }

    public int CalcMoveCost(EDirection direction)
    {
        return terrainType.moveCost + walls[(int)direction].moveCost;
    }

    public coverValue ProvidesCoverInDirection(Vector3 direction)
    {
        //TODO: CHECK IF THERE IS A WALL IN THE DIRECTION GIVEN.
        return coverValue.Half;
    }

    public int chanceToHit(Tile other)
    {
        //TODO: CALCULATE THE CHANCE TO HIT FROM ONE TILE TO ANOTHER ON THE GRID
        return 0;
    }

    public bool isVisibleFromTile(Tile other)
    {
        return true;
    }


}
