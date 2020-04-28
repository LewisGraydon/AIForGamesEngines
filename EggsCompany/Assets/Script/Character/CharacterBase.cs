using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class CharacterBase : MonoBehaviour
{
    #region external object references
    protected GameObject gsm;
    protected GameState gsmScript;

    protected Text healthText;
    protected Text actionPipsText;

    public Tile occupiedTile;
    #endregion

    #region pathfinding variables
    private Stack<INodeSearchable> tilePathToDestination = null;
    private List<INodeSearchable> allPossibleMovementNodes = null;
    private Vector3 directionToDestination;
    private Tile _currentDestinationTile;
    private Tile currentDestinationTile
    {
        get
        {
            return _currentDestinationTile;
        }
        set
        {
            _currentDestinationTile = value;
            _currentDestinationTile.GetComponent<Tile>().occupier = this;
            occupiedTile = _currentDestinationTile;
            directionToDestination = _currentDestinationTile.gameObject.transform.position - this.gameObject.transform.position;
            directionToDestination.y = 0;
        }
    }

    public int MovementRange { get => 5; }
    #endregion

    #region gameplay variables
    public bool OnPlayerTeam { get => (this is PlayerCharacter); }

    public int MaximumActionPips { get => 2; }
    protected int _actionPips = 2;
    public int actionPips
    {
        get => _actionPips;
        set
        {
            _actionPips = Mathf.Clamp(value, 0, MaximumActionPips);
            if (actionPipsText)
                actionPipsText.text = "ACTION PIPS: " + _actionPips;
        }
    }


    public int MaximumHealthValue { get => 6; }
    protected int _health = 6;
    public int health
    {
        get => _health;
        set
        {
            _health = Mathf.Clamp(value, 0, MaximumHealthValue);
            if (healthText)
                healthText.text = "HEALTH: " + _health;
        }
    }

    public int MaximumAmmunition { get => 6; }
    protected int _ammunition = 5;
    public int ammunition
    {
        get => _ammunition;
        set
        {
            _ammunition = Mathf.Clamp(value, 0, MaximumAmmunition);
            //TODO: update UI;
        }
    }

    private bool _isDefending = false;
    public bool isDefending { get => isDefending; }


    protected List<KeyValuePair<CharacterBase, int>> _enemiesInSight = new List<KeyValuePair<CharacterBase, int>>();
    public List<KeyValuePair<CharacterBase, int>> enemiesInSight
    {
        get
        {
            return _enemiesInSight;
        }
    }
    public float sightDistance = 0.0f;
    #endregion

    //Awake instead of Start() as it is not called when instantiating an object.
    private void Awake()
    {
        gsm = GameObject.Find("GameStateManager");
        gsmScript = gsm.GetComponent<GameState>();
        Text[] allAttachedTexts = gameObject.GetComponentsInChildren<Text>();
        foreach (Text text in allAttachedTexts)
        {
            if (text.text.Contains("HEALTH"))
            {
                healthText = text;
            }
            else if (text.text.Contains("ACTION"))
            {
                actionPipsText = text;
            }
            if (healthText != null && actionPipsText != null)
            {
                break;
            }
        }
        health = MaximumHealthValue;
        actionPips = MaximumActionPips;
        ammunition = MaximumAmmunition;
    }

    public bool isInCover(CharacterBase fromEnemy)
    {
        return false;
    }

    public void EnterOverwatchStance()
    {
        //probably have a ref to gamestate and add to an overwatch list. then have it looped over during other movements etc.
        Debug.Log("Doing An Overwatch Stance");
    }

    public void EnterDefenseStance()
    {
        Debug.Log("Doing A Defensive Stance");
        _isDefending = true;
    }

    public void AttackCharacter(CharacterBase otherCharacter)
    {
        Debug.Log("Doing An Attack");
    }

    public void Reload()
    {
        Debug.Log("Doing A Reload");
        ammunition = MaximumAmmunition;
        actionPips = 0;        
    }

    public List<INodeSearchable> FindSightline(int visionRange)
    {
        List<INodeSearchable> possibleSeenTiles  = gsmScript.pathfindingAgent.FindNodeSightRange(occupiedTile, visionRange);
        List<INodeSearchable> seenTiles = new List<INodeSearchable>();

        //Math from pawn to target tile
        //center to center
        //center to directions
        //Layer mask to ignore anything that isn't a wall
        //Tile distance to figure out max raycast distance
        //direction to send the raycast "2D" Vector

        //max distance of raycast = magnitude of (center of pawn transform - tile transform + pawn.y)
        //target transform vector - starting transform vector = direction

        foreach (var tile in possibleSeenTiles)
        {
            Tile workingTile = tile as Tile;
            Vector3 pawnYoffset = new Vector3(0, .5f, 0);
            Vector3 tiledirectionEoffset = new Vector3(.4f, 0, 0);
            Vector3 tiledirectionWoffset = new Vector3(-.4f, 0, 0);
            Vector3 tiledirectionNoffset = new Vector3(0, 0, .4f);
            Vector3 tiledirectionSoffset = new Vector3(0, 0, -.4f);
            Vector3 targetcentre = workingTile.transform.position + pawnYoffset;
            Vector3 startingcentre = occupiedTile.transform.position + pawnYoffset;
            Vector3 difference = targetcentre - startingcentre;

            //calc distance
            float distance = Vector3.Magnitude(difference);
            //calc direction
            Vector3 direction = Vector3.Normalize(difference);
            //find layer mask, assuming wall layer is 8
            int wallLayer = 1 << 8;

            //If the raycast hits nothing, add this tile to seen
            if (!Physics.Raycast(targetcentre, direction, distance, wallLayer))
            {
                seenTiles.Add(tile);
                if((tile as Tile).occupier is EnemyCharacter)
                {
                    
                }
            }
            else
            {
                bool hitN;
                bool hitE;
                bool hitS;
                bool hitW;

                Vector3 differenceN = (targetcentre + tiledirectionNoffset) - (startingcentre + tiledirectionNoffset);
                Vector3 differenceE = (targetcentre + tiledirectionEoffset) - (startingcentre + tiledirectionEoffset);
                Vector3 differenceS = (targetcentre + tiledirectionSoffset) - (startingcentre + tiledirectionSoffset);
                Vector3 differenceW = (targetcentre + tiledirectionWoffset) - (startingcentre + tiledirectionWoffset);

                float distanceN = Vector3.Magnitude(differenceN);
                float distanceE = Vector3.Magnitude(differenceE);
                float distanceS = Vector3.Magnitude(differenceS);
                float distanceW = Vector3.Magnitude(differenceW);

                Vector3 directionN = Vector3.Normalize(differenceN); 
                Vector3 directionE = Vector3.Normalize(differenceE);
                Vector3 directionS = Vector3.Normalize(differenceS);
                Vector3 directionW = Vector3.Normalize(differenceW);

                hitN = Physics.Raycast(targetcentre, directionN, distanceN, wallLayer);
                hitE = Physics.Raycast(targetcentre, directionE, distanceE, wallLayer);
                hitS = Physics.Raycast(targetcentre, directionS, distanceS, wallLayer);
                hitW = Physics.Raycast(targetcentre, directionW, distanceW, wallLayer);

                if(!hitN ||!hitE ||!hitS ||!hitW)
                {
                    seenTiles.Add(tile);
                }
            }
        }
        return seenTiles;
        //occupiedTile;
    }

    //should contian the code to actually move a character along a path in my mind.
    public virtual void MoveCharacterAlongTilePath()
    {
        //Debug.Log("moving Character to: Some Tile I have no way to identify I think: " + tileToMoveTo != null ? tileToMoveTo.name : "tile is null");
        if (Mathf.Abs(this.transform.position.x - currentDestinationTile.transform.position.x) > 0.01f || Mathf.Abs(this.transform.position.z - currentDestinationTile.transform.position.z) > 0.01f)
        {
            this.transform.position += directionToDestination * Time.deltaTime;
            Debug.Log("Moving " + name + ", by: " + directionToDestination);
        }
        else if(tilePathToDestination.Count == 0)
        {
            gsmScript.gameState = (this is PlayerCharacter) ? EGameState.playerTurn : EGameState.enemyTurn;
            gsmScript.pathfindingAgent.NodeReset(allPossibleMovementNodes);
            actionPips--;
            gsmScript.ProcessGameState();
        }
        else
        {
            currentDestinationTile.GetComponent<Tile>().occupier = null;
            transform.position = new Vector3(currentDestinationTile.transform.position.x, this.transform.position.y, currentDestinationTile.transform.position.z);
            currentDestinationTile = tilePathToDestination.Pop() as Tile;
        }
    }

    public void faceCanvasToCamera()
    {
        if(healthText != null)
        {
            healthText.canvas.transform.LookAt(gameObject.transform.position + Camera.main.transform.rotation * UnityEngine.Vector3.forward, Camera.main.transform.rotation * UnityEngine.Vector3.up);
        }
    }

    public void SetMovementStack(Stack<INodeSearchable> movementStack, List<INodeSearchable> allEffectedNodes)
    {
        tilePathToDestination = movementStack;
        allPossibleMovementNodes = allEffectedNodes;
        currentDestinationTile = movementStack.Pop() as Tile;
    }
}
