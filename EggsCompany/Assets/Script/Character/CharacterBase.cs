using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class CharacterBase : MonoBehaviour
{
    #region external object references
    protected GameObject gsm;
    protected GameState gsmScript;

    protected Text healthText;
    public Text actionPipsText;

    private Tile _occupiedTile;
    public Tile occupiedTile
    {
        get => _occupiedTile;
        set
        {
            _occupiedTile = value;
            value.occupier = this;
        }
    }
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

    public int MovementRange { get => 7; }
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
    public bool isDefending { get => _isDefending; }

    private bool _isOverwatching = false;
    public bool isOverwatching { get => _isOverwatching; set => _isOverwatching = value; }

    protected List<KeyValuePair<CharacterBase, int>> _enemiesInSight = new List<KeyValuePair<CharacterBase, int>>();
    public List<KeyValuePair<CharacterBase, int>> enemiesInSight { get => _enemiesInSight; }

    public float sightDistance = 0.0f;

    public int Accuracy { get => 65; }

    private const float angleForFlanking = 0.44f;

    public float closeDistanceCap { get => 2; }
    public float middleDistanceCap { get => 4; }
    public float longDistanceCap { get => 6; }
    #endregion

    //Awake instead of Start() as it is not called when instantiating an object.
    protected void Awake()
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

    public virtual void EnterOverwatchStance()
    {
        //probably have a ref to gamestate and add to an overwatch list. then have it looped over during other movements etc.
        Debug.Log(name + ": is Doing An Overwatch Stance");
        _isOverwatching = true;
        actionPips = 0;
    }

    public void EnterDefenseStance()
    {
        _isDefending = true;
        actionPips = 0;
    }

    public virtual void AttackCharacter(CharacterBase otherCharacter)
    {
        Debug.Log(name + ": is Doing An attack for 2 damage ");
        actionPips = 0;
        int chanceToHit = enemiesInSight.Find((KeyValuePair<CharacterBase, int> characterHitChance) => characterHitChance.Key == otherCharacter).Value;
        if(UnityEngine.Random.Range(0, 100) <= chanceToHit)
        {
            otherCharacter.health -= 2; //TODO: replace 1 with damage;
        }
        gsmScript.ProcessGameState();
    }

    public virtual void OverwatchAttackCharacter(CharacterBase otherCharacter)
    {
        Debug.Log(name + ": is Doing An ");
        actionPips = 0;
        int chanceToHit = enemiesInSight.Find((KeyValuePair<CharacterBase, int> characterHitChance) => characterHitChance.Key == otherCharacter).Value;
        if (UnityEngine.Random.Range(0, 120) <= chanceToHit)
        {
            otherCharacter.health -= 2; //TODO: replace 1 with damage;
        }
        gsmScript.ProcessGameState();
    }

    public void Reload()
    {
        Debug.Log(name + ": is Doing A Reload");
        ammunition = MaximumAmmunition;
        actionPips = 0;
    }

    public List<INodeSearchable> FindSightline(int visionRange = 2)
    {
        Tile[] g = GameObject.FindObjectsOfType<Tile>();

        gsmScript.pathfindingAgent.NodeReset(g.ToList<INodeSearchable>());

        if (tilePathToDestination != null)
        {
            gsmScript.pathfindingAgent.NodeReset(tilePathToDestination.ToList());
        }

        enemiesInSight.Clear();

        List<INodeSearchable> possibleSeenTiles = gsmScript.pathfindingAgent.FindNodeSightRange(occupiedTile, visionRange);
        List<INodeSearchable> seenTiles = new List<INodeSearchable>();

        if (this is PlayerCharacter)
        {
            gsmScript.clearBadEggsSpottedContainer();        
        }

        if (!gsmScript.isAnyBadEggSpotted())
        {
            gsmScript.badEggsSpottedUI.SetActive(false);
        }

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
            if (!Physics.Raycast(startingcentre, direction, distance, wallLayer))
            {
                Debug.DrawRay(startingcentre, direction * distance, Color.magenta, 5, true);
                seenTiles.Add(tile);
                if (workingTile.occupier != null && (this is PlayerCharacter && workingTile.occupier is EnemyCharacter) || (this is EnemyCharacter && workingTile.occupier is PlayerCharacter))
                {
                    enemiesInSight.Add(new KeyValuePair<CharacterBase, int>(workingTile.occupier, CalculateHitChance(workingTile.occupier, false, false)));
                }
            }
            else
            {
                bool hitN;
                bool hitE;
                bool hitS;
                bool hitW;

                Vector3 differenceN = new Vector3((targetcentre.x + tiledirectionNoffset.x) - (startingcentre.x + tiledirectionNoffset.x), (targetcentre.y + tiledirectionNoffset.y) - (startingcentre.y + tiledirectionNoffset.y), (targetcentre.z + tiledirectionNoffset.z) - (startingcentre.z + tiledirectionNoffset.z));
                Vector3 differenceE = new Vector3((targetcentre.x + tiledirectionEoffset.x) - (startingcentre.x + tiledirectionEoffset.x), (targetcentre.y + tiledirectionEoffset.y) - (startingcentre.y + tiledirectionEoffset.y), (targetcentre.z + tiledirectionEoffset.z) - (startingcentre.z + tiledirectionEoffset.z));
                Vector3 differenceS = new Vector3((targetcentre.x + tiledirectionSoffset.x) - (startingcentre.x + tiledirectionSoffset.x), (targetcentre.y + tiledirectionSoffset.y) - (startingcentre.y + tiledirectionSoffset.y), (targetcentre.z + tiledirectionSoffset.z) - (startingcentre.z + tiledirectionSoffset.z));
                Vector3 differenceW = new Vector3((targetcentre.x + tiledirectionWoffset.x) - (startingcentre.x + tiledirectionWoffset.x), (targetcentre.y + tiledirectionWoffset.y) - (startingcentre.y + tiledirectionWoffset.y), (targetcentre.z + tiledirectionWoffset.z) - (startingcentre.z + tiledirectionWoffset.z));

                Vector3 modifiedStartN = new Vector3((startingcentre.x + tiledirectionNoffset.x), (startingcentre.y + tiledirectionNoffset.y), (startingcentre.z + tiledirectionNoffset.z));
                Vector3 modifiedStartE = new Vector3((startingcentre.x + tiledirectionEoffset.x), (startingcentre.y + tiledirectionEoffset.y), (startingcentre.z + tiledirectionEoffset.z));
                Vector3 modifiedStartS = new Vector3((startingcentre.x + tiledirectionSoffset.x), (startingcentre.y + tiledirectionSoffset.y), (startingcentre.z + tiledirectionSoffset.z));
                Vector3 modifiedStartW = new Vector3((startingcentre.x + tiledirectionWoffset.x), (startingcentre.y + tiledirectionWoffset.y), (startingcentre.z + tiledirectionWoffset.z));


                float distanceN = Vector3.Magnitude(differenceN);
                float distanceE = Vector3.Magnitude(differenceE);
                float distanceS = Vector3.Magnitude(differenceS);
                float distanceW = Vector3.Magnitude(differenceW);

                Vector3 directionN = Vector3.Normalize(differenceN);
                Vector3 directionE = Vector3.Normalize(differenceE);
                Vector3 directionS = Vector3.Normalize(differenceS);
                Vector3 directionW = Vector3.Normalize(differenceW);

                hitN = Physics.Raycast(modifiedStartN, directionN, distanceN, wallLayer);
                hitE = Physics.Raycast(modifiedStartE, directionE, distanceE, wallLayer);
                hitS = Physics.Raycast(modifiedStartS, directionS, distanceS, wallLayer);
                hitW = Physics.Raycast(modifiedStartW, directionW, distanceW, wallLayer);

                if (!hitN || !hitE || !hitS || !hitW)
                {
                    int coverValue = 0;
                    if (direction.x >= angleForFlanking)
                    {
                        coverValue = workingTile.walls[(int)EWallDirection.East].coverValue;
                    }
                    else if (direction.x <= -angleForFlanking)
                    {
                        coverValue = workingTile.walls[(int)EWallDirection.West].coverValue;
                    }
                    if (direction.z >= angleForFlanking)
                    {
                        coverValue = coverValue == 0 ?
                        workingTile.walls[(int)EWallDirection.South].coverValue : workingTile.walls[(int)EWallDirection.South].coverValue < coverValue ?
                                                                                        workingTile.walls[(int)EWallDirection.South].coverValue : coverValue;
                    }
                    else if (direction.z <= -angleForFlanking)
                    {
                        coverValue = coverValue == 0 ?
                        workingTile.walls[(int)EWallDirection.South].coverValue : workingTile.walls[(int)EWallDirection.North].coverValue < coverValue ?
                                                                                    workingTile.walls[(int)EWallDirection.North].coverValue : coverValue;
                    }
                    if (workingTile.occupier != null && (this is PlayerCharacter && workingTile.occupier is EnemyCharacter) || (this is EnemyCharacter && workingTile.occupier is PlayerCharacter))
                    {
                        enemiesInSight.Add(new KeyValuePair<CharacterBase, int>(workingTile.occupier, CalculateHitChance(workingTile.occupier, coverValue == 1, coverValue == 2)));                       
                    }
                    
                    seenTiles.Add(tile);
                }           
            }
        }
        gsmScript.pathfindingAgent.NodeReset(possibleSeenTiles);

        if (this is PlayerCharacter)
        {
            gsmScript.badEggsSpottedUI.SetActive(true);
            gsmScript.addToBadEggsSpottedUI(enemiesInSight);
        }

        return seenTiles;
    }

    public KeyValuePair<bool, int> FudgedSightHitchance(Tile shooterTile, Tile targetTile)
    {
        KeyValuePair<bool, int> canBeSeenHitChancePair = new KeyValuePair<bool, int>(false, -8888);
        Vector3 pawnYoffset = new Vector3(0, .5f, 0);
        Vector3 tiledirectionEoffset = new Vector3(.4f, 0, 0);
        Vector3 tiledirectionWoffset = new Vector3(-.4f, 0, 0);
        Vector3 tiledirectionNoffset = new Vector3(0, 0, .4f);
        Vector3 tiledirectionSoffset = new Vector3(0, 0, -.4f);
        Vector3 targetcentre = targetTile.transform.position + pawnYoffset;
        Vector3 startingcentre = shooterTile.transform.position + pawnYoffset;
        Vector3 difference = targetcentre - startingcentre;

        //calc distance
        float distance = Vector3.Magnitude(difference);
        //calc direction
        Vector3 direction = Vector3.Normalize(difference);
        //find layer mask, assuming wall layer is 8
        int wallLayer = 1 << 8;
        //If the raycast hits nothing, add this tile to seen
        if (!Physics.Raycast(startingcentre, direction, distance, wallLayer))
        {
            Debug.DrawRay(startingcentre, direction * distance, Color.magenta, 5, true);
            if (targetTile.occupier != null && (this is PlayerCharacter && targetTile.occupier is EnemyCharacter) || (this is EnemyCharacter && targetTile.occupier is PlayerCharacter))
            {
                canBeSeenHitChancePair = new KeyValuePair<bool, int>(true, CalculateHitChance(targetTile.occupier, false, false));
            }
        }
        else
        {
            bool hitN;
            bool hitE;
            bool hitS;
            bool hitW;

            Vector3 differenceN = new Vector3((targetcentre.x + tiledirectionNoffset.x) - (startingcentre.x + tiledirectionNoffset.x), (targetcentre.y + tiledirectionNoffset.y) - (startingcentre.y + tiledirectionNoffset.y), (targetcentre.z + tiledirectionNoffset.z) - (startingcentre.z + tiledirectionNoffset.z));
            Vector3 differenceE = new Vector3((targetcentre.x + tiledirectionEoffset.x) - (startingcentre.x + tiledirectionEoffset.x), (targetcentre.y + tiledirectionEoffset.y) - (startingcentre.y + tiledirectionEoffset.y), (targetcentre.z + tiledirectionEoffset.z) - (startingcentre.z + tiledirectionEoffset.z));
            Vector3 differenceS = new Vector3((targetcentre.x + tiledirectionSoffset.x) - (startingcentre.x + tiledirectionSoffset.x), (targetcentre.y + tiledirectionSoffset.y) - (startingcentre.y + tiledirectionSoffset.y), (targetcentre.z + tiledirectionSoffset.z) - (startingcentre.z + tiledirectionSoffset.z));
            Vector3 differenceW = new Vector3((targetcentre.x + tiledirectionWoffset.x) - (startingcentre.x + tiledirectionWoffset.x), (targetcentre.y + tiledirectionWoffset.y) - (startingcentre.y + tiledirectionWoffset.y), (targetcentre.z + tiledirectionWoffset.z) - (startingcentre.z + tiledirectionWoffset.z));

            Vector3 modifiedStartN = new Vector3((startingcentre.x + tiledirectionNoffset.x), (startingcentre.y + tiledirectionNoffset.y), (startingcentre.z + tiledirectionNoffset.z));
            Vector3 modifiedStartE = new Vector3((startingcentre.x + tiledirectionEoffset.x), (startingcentre.y + tiledirectionEoffset.y), (startingcentre.z + tiledirectionEoffset.z));
            Vector3 modifiedStartS = new Vector3((startingcentre.x + tiledirectionSoffset.x), (startingcentre.y + tiledirectionSoffset.y), (startingcentre.z + tiledirectionSoffset.z));
            Vector3 modifiedStartW = new Vector3((startingcentre.x + tiledirectionWoffset.x), (startingcentre.y + tiledirectionWoffset.y), (startingcentre.z + tiledirectionWoffset.z));


            float distanceN = Vector3.Magnitude(differenceN);
            float distanceE = Vector3.Magnitude(differenceE);
            float distanceS = Vector3.Magnitude(differenceS);
            float distanceW = Vector3.Magnitude(differenceW);

            Vector3 directionN = Vector3.Normalize(differenceN);
            Vector3 directionE = Vector3.Normalize(differenceE);
            Vector3 directionS = Vector3.Normalize(differenceS);
            Vector3 directionW = Vector3.Normalize(differenceW);

            hitN = Physics.Raycast(modifiedStartN, directionN, distanceN, wallLayer);
            hitE = Physics.Raycast(modifiedStartE, directionE, distanceE, wallLayer);
            hitS = Physics.Raycast(modifiedStartS, directionS, distanceS, wallLayer);
            hitW = Physics.Raycast(modifiedStartW, directionW, distanceW, wallLayer);

            if (!hitN || !hitE || !hitS || !hitW)
            {
                int coverValue = 0;
                if (direction.x >= angleForFlanking)
                {
                    coverValue = targetTile.walls[(int)EWallDirection.East].coverValue;
                }
                else if (direction.x <= -angleForFlanking)
                {
                    coverValue = targetTile.walls[(int)EWallDirection.West].coverValue;
                }
                if (direction.z >= angleForFlanking)
                {
                    coverValue = coverValue == 0 ?
                    targetTile.walls[(int)EWallDirection.South].coverValue : targetTile.walls[(int)EWallDirection.South].coverValue < coverValue ?
                                                                                    targetTile.walls[(int)EWallDirection.South].coverValue : coverValue;
                }
                else if (direction.z <= -angleForFlanking)
                {
                    coverValue = coverValue == 0 ?
                    targetTile.walls[(int)EWallDirection.South].coverValue : targetTile.walls[(int)EWallDirection.North].coverValue < coverValue ?
                                                                                targetTile.walls[(int)EWallDirection.North].coverValue : coverValue;
                }
                if (targetTile.occupier != null && (this is PlayerCharacter && targetTile.occupier is EnemyCharacter) || (this is EnemyCharacter && targetTile.occupier is PlayerCharacter))
                {
                    canBeSeenHitChancePair = new KeyValuePair<bool, int>(true, CalculateHitChance(targetTile.occupier, coverValue == 1, coverValue == 2));
                }
            }
        }
        return canBeSeenHitChancePair;
    
    }

    //should contian the code to actually move a character along a path in my mind.
    public virtual void MoveCharacterAlongTilePath()
    {
        if (Mathf.Abs(this.transform.position.x - currentDestinationTile.transform.position.x) > 0.05f || Mathf.Abs(this.transform.position.z - currentDestinationTile.transform.position.z) > 0.05f)
        {
            this.transform.position += directionToDestination * Time.deltaTime;
        }
        else if (tilePathToDestination.Count == 0)
        {
            gsmScript.gameState = (this is PlayerCharacter) ? EGameState.playerTurn : EGameState.enemyTurn;
            gsmScript.pathfindingAgent.NodeReset(allPossibleMovementNodes);
            FindSightline();
            gsmScript.updateOtherTeamSightLines(this);
            actionPips--;
            gsmScript.ProcessGameState();
        }
        else
        {
            currentDestinationTile.GetComponent<Tile>().occupier = null;
            transform.position = new Vector3(currentDestinationTile.transform.position.x, this.transform.position.y, currentDestinationTile.transform.position.z);
            currentDestinationTile = tilePathToDestination.Pop() as Tile;

            //check if this destination is in overwatching sightlines


            FindSightline();
            gsmScript.updateOtherTeamSightLines(this);
        }
    }

    public void faceCanvasToCamera()
    {
        if (healthText != null)
        {
            healthText.canvas.transform.LookAt(gameObject.transform.position + Camera.main.transform.rotation * UnityEngine.Vector3.forward, Camera.main.transform.rotation * UnityEngine.Vector3.up);
        }
    }

    public void SetMovementStack(Stack<INodeSearchable> movementStack, List<INodeSearchable> allEffectedNodes)
    {
        tilePathToDestination = movementStack;
        allPossibleMovementNodes = allEffectedNodes;
        currentDestinationTile = movementStack.Pop() as Tile;
        gsmScript.pathfindingAgent.NodeReset(allPossibleMovementNodes);
    }

    int CalculateHitChance(CharacterBase other, bool isHalfCovered, bool isFullCovered)
    {
        int outInt = Accuracy; //acc = 65
        #region handle cover (or lack thereof) effects;
        if (isFullCovered)
        {
            outInt -= 20; // w/ full = 45
        }
        else if (isHalfCovered)
        {
            outInt -= 10; // w/ half = 55;
        }
        else // therefore flanked;
        {
            outInt += 20; // flanked = 85;
        }
        #endregion
        outInt -= other.isDefending ? 10 : 0;
        #region handle distance effects;
        // assuming that a tile is 1*1;
        // within 2 = + to hit chance, 2 to 4 is nothing, >= 4 is -10, >= 6 is -20;

        float distance = Vector3.Distance(this.gameObject.transform.position, other.gameObject.transform.position);
        if (distance < closeDistanceCap)
        {
            outInt += 10; // max w/ flanking = 65 +20 + 10 = 95; good enough;
        }
        else if (distance < middleDistanceCap)
        {
            outInt += 0;
        }
        else if (distance < longDistanceCap)
        {
            outInt -= 10;
        }
        else
        {
            outInt -= 20; // worst case = 65 - 20 - 10 - 20 = 15;
        }

        #endregion
        return outInt;
    }
}
