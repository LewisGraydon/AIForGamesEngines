using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour, INodeSearchable
{

    //Currently working on the assumption that we are hand designing the levels
    //GameObject is placeholder for egg or terrainBlocker object
    public ETileOccupier occupier = ETileOccupier.None;
    public FloorType terrainType;
    private List<Tile> _neighbors = new List<Tile>((int)EDirection.Error);
    public List<Tile> neighbors
    {
        get
        {
            return _neighbors;
        }
    }

    public List<WallType> walls = new List<WallType>(4);

    public bool searched { get => searched; set => searched = value; }
    public float? DijkstraCost { get => DijkstraCost; set => DijkstraCost = value; }
    public float? HeuristicCost { get => DijkstraCost; set => DijkstraCost = value; }
    public float? TotalCost { get => TotalCost; set => TotalCost = value; }
    public INodeSearchable parent { get => parent; set => parent = value; }
    public List<INodeSearchable> children { get; set; }
    public float distanceToTarget { get => distanceToTarget; set => distanceToTarget = value; }
    static private float activeCoverDirectionThreshold = 0.44f;

    void Awake()
    {
        //Order here is important to align with direction
        children = new List<INodeSearchable>();
        
        //parent = GetComponentInParent<Transform>();
    }

    void Start()
    {
        GenerateWalls();
    }

    void GenerateWalls()
    {
        for(int i = 0; i < walls.Count; i++)
        {
            float wallHeight;
            if(walls[i].wallName != EWallName.Empty)
            {
                GameObject wallCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wallHeight = walls[i].wallName == EWallName.FullWall ? 1.0f : 0.5f;                                               

                if(i == (int)EWallDirection.North || i == (int)EWallDirection.South)
                {
                    wallCube.transform.localScale = new Vector3(1.0f, wallHeight, 0.05f);
                }

                else if (i == (int)EWallDirection.East || i == (int)EWallDirection.West)
                {
                    wallCube.transform.localScale = new Vector3(0.05f, wallHeight, 1.0f);
                }

                float wallCubeX = transform.position.x;
                float wallCubeY = transform.position.y + (wallCube.transform.localScale.y / 2.0f + transform.localScale.y / 2.0f);
                float wallCubeZ = transform.position.z;

                switch(i)
                {
                    case (int)EWallDirection.North:
                        wallCubeZ = transform.position.z + (transform.localScale.z / 2.0f - wallCube.transform.localScale.z / 2.0f);
                        break;

                    case (int)EWallDirection.East:
                        wallCubeX = transform.position.x + (transform.localScale.z / 2.0f - wallCube.transform.localScale.x / 2.0f);
                        break;

                    case (int)EWallDirection.South:
                        wallCubeZ = transform.position.z - (transform.localScale.z / 2.0f - wallCube.transform.localScale.z / 2.0f);
                        break;

                    case (int)EWallDirection.West:
                        wallCubeX = transform.position.x - (transform.localScale.z / 2.0f - wallCube.transform.localScale.x / 2.0f);
                        break;

                    default:
                        break;
                }

                wallCube.transform.position = new Vector3(wallCubeX, wallCubeY, wallCubeZ);
                // Make tile parent of wall nodes.
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignNeighbor(EDirection direction, Tile neighborTile)
    {
        _neighbors[(int)direction] = neighborTile;
        AssignNeighborToChildren(direction, neighborTile);
    }

    private void AssignNeighborToChildren(EDirection direction, INodeSearchable tileNode)
    {
        children[(int)direction] = tileNode;
    }

    //Adds empty count for list.
    //This is called from tilegrid to avoid array bounds exception on _neighbors when setting up the grid
    //Awake does not appear to run in time/during TileGrid start. Use constructor?
    public void NeighborListFill()
    {
        for(int i = 0; i < (int)EDirection.Error; i++)
        {
            _neighbors.Add(null);   
        }
        ChildrenListFill();
    }

    //Also adds empty count for INodeSearchable list.
    private void ChildrenListFill()
    {
        for (int i = 0; i < (int)EDirection.Error; i++)
        {
            children.Add(null);
        }
    }

    public int CalcMoveCost(EDirection direction)
    {
        return terrainType.moveCost + walls[(int)direction].moveCost;
    }

    //MADE ON THE UNDERSTANDING THAT +X = North and +Z = West
    public ECoverValue ProvidesCoverInDirection(Vector3 direction)
    {
        //TODO: CHECK IF THERE IS A WALL IN THE DIRECTION GIVEN.
        if(direction.x >= activeCoverDirectionThreshold)
        {
            return (ECoverValue)walls[(int)EDirection.West].coverValue;
        }
        else if(direction.x <= -activeCoverDirectionThreshold)
        {
            return (ECoverValue)walls[(int)EDirection.East].coverValue;
        }
        else
        {
            //bobble;
        }
        if (direction.x >= activeCoverDirectionThreshold)
        {
            return (ECoverValue)walls[(int)EDirection.North].coverValue;
        }
        else if (direction.x <= -activeCoverDirectionThreshold)
        {
            return (ECoverValue)walls[(int)EDirection.South].coverValue;
        }
        else
        {
            //bobble;
        }

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

    private void OnTriggerEnter(Collider other)
    {
        Tile otherTile = other.gameObject.GetComponent<Tile>();
        if (otherTile != null)
        {
            EDirection dir = EDirection.Error;

            if (other.transform.position.x < transform.position.x && other.transform.position.z < transform.position.z)
            {
                dir = EDirection.SouthWest;
            }

            else if (other.transform.position.x < transform.position.x && other.transform.position.z > transform.position.z)
            {
                dir = EDirection.NorthWest;
            }

            else if (other.transform.position.x > transform.position.x && other.transform.position.z < transform.position.z)
            {
                dir = EDirection.SouthEast;   
            }

            else if (other.transform.position.x > transform.position.x && other.transform.position.z > transform.position.z)
            {
                dir = EDirection.NorthEast;
            }

            else if(other.transform.position.x > transform.position.x)
            {
                dir = EDirection.East;
                if (otherTile.walls[(int)EWallDirection.West] != walls[(int)EWallDirection.East])
                {
                    if (otherTile.walls[(int)EWallDirection.West].coverValue > walls[(int)EWallDirection.East].coverValue)
                    {
                        walls[(int)EWallDirection.East] = otherTile.walls[(int)EWallDirection.West];
                    }
                    else
                    {
                        otherTile.walls[(int)EWallDirection.West] = walls[(int)EWallDirection.East];
                    }
                }
            }

            else if (other.transform.position.x < transform.position.x)
            {
                dir = EDirection.West;
                if (otherTile.walls[(int)EWallDirection.East] != walls[(int)EWallDirection.West])
                {
                    if (otherTile.walls[(int)EWallDirection.East].coverValue > walls[(int)EWallDirection.West].coverValue)
                    {
                        walls[(int)EWallDirection.West] = otherTile.walls[(int)EWallDirection.East];
                    }
                    else
                    {
                        otherTile.walls[(int)EWallDirection.East] = walls[(int)EWallDirection.West];
                    }
                }
            }

            else if (other.transform.position.z > transform.position.z)
            {
                dir = EDirection.North;
                if (otherTile.walls[(int)EWallDirection.South] != walls[(int)EWallDirection.North])
                {
                    if (otherTile.walls[(int)EWallDirection.South].coverValue > walls[(int)EWallDirection.North].coverValue)
                    {
                        walls[(int)EWallDirection.North] = otherTile.walls[(int)EWallDirection.South];
                    }
                    else
                    {
                        otherTile.walls[(int)EWallDirection.South] = walls[(int)EWallDirection.North];
                    }
                }
            }

            else if (other.transform.position.z < transform.position.z)
            {
                dir = EDirection.South;
                if(otherTile.walls[(int)EWallDirection.North] != walls[(int)EWallDirection.South])
                {
                    if (otherTile.walls[(int)EWallDirection.North].coverValue > walls[(int)EWallDirection.South].coverValue)
                    {                   
                        walls[(int)EWallDirection.South] = otherTile.walls[(int)EWallDirection.North];
                    }
                    else
                    {
                        otherTile.walls[(int)EWallDirection.North] = walls[(int)EWallDirection.South];
                    }
                }               
            }

            AssignNeighbor(dir, otherTile);
        }
    }
}

