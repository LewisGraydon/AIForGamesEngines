using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    //Currently working on the assumption that we are hand designing the levels
    //GameObject is placeholder for egg or terrainBlocker object
    private GameObject occupier = null;
    public ITerrainType terrainType;
    private List<Tile> neighbors = new List<Tile>(4);

    private IWallType[] walls;
    public IWallType nWall;
    public IWallType eWall;
    public IWallType sWall;
    public IWallType wWall;

    // Start is called before the first frame update
    void Start()
    {
        //neighbors = new List<Tile>(4);
        //neighbors.Add(this.gameObject.AddComponent(typeof(Tile)) as Tile);
        //neighbors.Add(this.gameObject.AddComponent(typeof(Tile)) as Tile);
        //neighbors.Add(this.gameObject.AddComponent(typeof(Tile)) as Tile);
        //neighbors.Add(this.gameObject.AddComponent(typeof(Tile)) as Tile);

        //walls[(int)EDirection.North] = nWall;
        //walls[(int)EDirection.East] = eWall;
        //walls[(int)EDirection.South] = sWall;
        //walls[(int)EDirection.West] = wWall;
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

    public int CalcMoveCost(EDirection direction)
    {
        return terrainType.moveCost + walls[(int)direction].moveCost;
    }


}
