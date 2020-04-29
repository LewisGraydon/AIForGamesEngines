using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Tile : MonoBehaviour, INodeSearchable
{

    //Currently working on the assumption that we are hand designing the levels
    //GameObject is placeholder for egg or terrainBlocker object
    public CharacterBase occupier = null;
    public FloorType terrainType;
    private List<Tile> _neighbors = new List<Tile>((int)EDirection.Error)
    {
    };
    public List<Tile> neighbors
    {
        get
        {
            return _neighbors;
        }
    }

    public List<WallType> walls = new List<WallType>(4);

    public bool searched { get; set; }
    public float? DijkstraCost { get; set; }
    public float? HeuristicCost { get; set; }
    public float? TotalCost { get; set; }
    public INodeSearchable parent { get; set; }
    public List<INodeSearchable> children { get; set; }
    public float distanceToTarget { get; set; }
    static private float activeCoverDirectionThreshold = 0.44f;

    private Renderer gameObjectRenderer;

    void Awake()
    {
        //Order here is important to align with direction
        children = new List<INodeSearchable>();
        
        //parent = GetComponentInParent<Transform>();
    }

    private Color startcolor;
    private GameState gsmScript = null;
    private PlayerManager pmScript = null;

    void OnMouseEnter()
    {
        if(gsmScript.gameState != EGameState.playerTurn)
        {
            return;
        }

        startcolor = gameObjectRenderer.material.color;       
        pmScript.destinationTile = this;

        pmScript.nodeSearchables = gsmScript.pathfindingAgent.FindMovementRange(pmScript.selectedPlayer.GetComponent<CharacterBase>().occupiedTile, pmScript.selectedPlayer.GetComponent<CharacterBase>().MovementRange);

        if (!pmScript.nodeSearchables.Contains(pmScript.destinationTile) || pmScript.selectedPlayer.GetComponent<CharacterBase>().actionPips == 0)
        {
            gameObjectRenderer.material.color = Color.magenta;
        }
        else
        {
            gameObjectRenderer.material.color = Color.cyan;
            pmScript.selectedPlayer.GetComponent<CharacterBase>().actionPipsText.text += " (-1)";
        }

        //Probably text showing pip for movement to said tile (or something)
    }

    void OnMouseExit()
    {
        if(gsmScript.gameState != EGameState.playerTurn)
        {
            return;
        }

        gameObjectRenderer.material.color = startcolor;
        GameObject.Find("Players").GetComponent<PlayerManager>().destinationTile = null;

        pmScript.selectedPlayer.GetComponent<CharacterBase>().actionPips = pmScript.selectedPlayer.GetComponent<CharacterBase>().actionPips;

        gsmScript.pathfindingAgent.NodeReset(pmScript.nodeSearchables);
    }


    void Start()
    {
        gameObjectRenderer = GetComponent<Renderer>();        
        gsmScript = GameObject.Find("GameStateManager").GetComponent<GameState>();
        pmScript = GameObject.Find("Players").GetComponent<PlayerManager>();

        NeighborListFill();
        GenerateWalls();

        switch(terrainType.floorName)
        {
            case EFloorName.Road:
                gameObjectRenderer.material.color = Color.grey;
                break;

            case EFloorName.SpawnPoint:
                gameObjectRenderer.material.color = Color.green;
                break;

            case EFloorName.SpawnPointEnemy:
                gameObjectRenderer.material.color = Color.red;
                break;

            default:
                break;
        }
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
                wallCube.layer = 8;

                // Make tile parent of wall nodes.
                wallCube.transform.parent = transform;   
                
                // For debug purposes, half walls will be a different colour to regular walls.
                if(walls[i].wallName == EWallName.HalfWall)
                {
                    wallCube.GetComponent<Renderer>().material.color = Color.magenta;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignNeighbor(EDirection direction, Tile neighborTile)
    {
        if(direction == EDirection.Error)
        {
            Debug.Log("HELP");
        }
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

            if (Mathf.RoundToInt(other.transform.position.x) < Mathf.RoundToInt(transform.position.x) && Mathf.RoundToInt(other.transform.position.z) < Mathf.RoundToInt(transform.position.z))
            {
                dir = EDirection.SouthWest;
            }

            else if (Mathf.RoundToInt(otherTile.transform.position.x) < Mathf.RoundToInt(transform.position.x) && Mathf.RoundToInt(otherTile.transform.position.z) > Mathf.RoundToInt(transform.position.z))
            {
                dir = EDirection.NorthWest;
            }

            else if (Mathf.RoundToInt(other.transform.position.x) > Mathf.RoundToInt(transform.position.x) && Mathf.RoundToInt(other.transform.position.z) < Mathf.RoundToInt(transform.position.z))
            {
                dir = EDirection.SouthEast;
            }

            else if (Mathf.RoundToInt(other.transform.position.x) > Mathf.RoundToInt(transform.position.x) && Mathf.RoundToInt(other.transform.position.z) > Mathf.RoundToInt(transform.position.z))
            {
                dir = EDirection.NorthEast;
            }

            else if(Mathf.RoundToInt(other.transform.position.x) > Mathf.RoundToInt(transform.position.x))
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

            else if (Mathf.RoundToInt(other.transform.position.x) < Mathf.RoundToInt(transform.position.x))
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

            else if (Mathf.RoundToInt(other.transform.position.z) > Mathf.RoundToInt(transform.position.z))
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

            else if (Mathf.RoundToInt(other.transform.position.z) < Mathf.RoundToInt(transform.position.z))
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

