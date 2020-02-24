using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, INodeSearchable
{

    //Currently working on the assumption that we are hand designing the levels
    //GameObject is placeholder for egg or terrainBlocker object
    public ETileOccupier occupier = ETileOccupier.None;
    public FloorType terrainType;
    private List<Tile> _neighbors = new List<Tile>(4);
    public List<Tile> neighbors
    {
        get
        {
            return _neighbors;
        }
    }

    public List<WallType> walls = new List<WallType>(4);
    public WallType nWall;
    public WallType eWall;
    public WallType sWall;
    public WallType wWall;

    public bool searched { get => searched; set => searched = value; }
    public float? DijkstraCost { get => DijkstraCost; set => DijkstraCost = value; }
    public float? HeuristicCost { get => DijkstraCost; set => DijkstraCost = value; }
    public float? TotalCost { get => TotalCost; set => TotalCost = value; }
    public INodeSearchable parent { get => parent; set => parent = value; }
    public List<INodeSearchable> children { get => children; set => children = value; }

    public float distanceToTarget { get => distanceToTarget; set => distanceToTarget = value; }

    // Start is called before the first frame update
    void Awake()
    {
        //Order here is important to align with direction
        walls.Add(nWall);
        walls.Add(eWall);
        walls.Add(sWall);
        walls.Add(wWall);

        //parent = GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignNeighbor(EDirection direction, Tile neighborTile)
    {
        _neighbors[(int)direction] = neighborTile;
        
        //neighbors[(int)direction] = nieghborTile;
    }

    //Can't directly copy to List of Interface. Manual copying over.
    public void CopyNeighborsToChildren()
    {
        foreach (var tile in _neighbors)
        {
            children.Add(tile);
        }
    }

    //Adds empty count for list.
    //This is called from tilegrid to avoid array bounds exception on _neighbors when setting up the grid
    //Awake does not appear to run in time/during TileGrid start. Use constructor?
    public void NeighborListFill()
    {
        for(int i = 0; i < 4; i++)
        {
            _neighbors.Add(null);
        }
    }

    public int CalcMoveCost(EDirection direction)
    {
        return terrainType.moveCost + walls[(int)direction].moveCost;
    }

    public ECoverValue ProvidesCoverInDirection(Vector3 direction)
    {
        //TODO: CHECK IF THERE IS A WALL IN THE DIRECTION GIVEN.
        return ECoverValue.Half;
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

