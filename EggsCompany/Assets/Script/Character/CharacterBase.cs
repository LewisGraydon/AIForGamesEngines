using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    protected GameObject gsm;
    protected GameState gsmScript;
    void Start()
    {
        gsm = GameObject.Find("GameStateManager");
        gsmScript = gsm.GetComponent<GameState>();
    }

    protected bool onPlayerTeam;
    public bool getOnPlayerTeam
    {
        get { return onPlayerTeam; }
    }

    protected int actionPips = 2;
    public int remainingPips
    {
        get
        {
            return actionPips;
        }
    }

    public void SetRemainingPips(int pipsRemaining)
    {
        if(pipsRemaining > maxActionPips)
        {
            actionPips = maxActionPips;
            return;
        }

        actionPips = pipsRemaining;
    }

    protected int maxActionPips = 2;
    public int maximumActionPips
    {
        get { return maxActionPips; }
    }

    protected int health = 6;
    public int remainingHealth
    {
        get { return health; }
    }

    protected int maxHealthValue = 6;
    public int maximumHealth
    {
        get { return health; }
    }

    protected int _ammunition = 5;

    protected int _maxAmmunition;

    protected List<CharacterBase> _enemiesInSight = new List<CharacterBase>();
    public List<CharacterBase> enemiesInSight
    {
        get
        {
            return _enemiesInSight;
        }
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
    }

    public void AttackCharacter(CharacterBase otherCharacter)
    {
        Debug.Log("Doing An Attack");
    }

    public void Reload()
    {
        Debug.Log("Doing A Reload");
        _ammunition = _maxAmmunition;
        actionPips = actionPips - 1 >= 0 ? actionPips - 1 : 0;
    }

    public void FindSightline(int visionRange)
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

            Vector3 difference = (workingTile.transform.position + pawnYoffset) - (occupiedTile.transform.position + pawnYoffset);

            //calc distance
            float distance = Vector3.Magnitude(difference);
            //calc direction
            Vector3 direction = Vector3.Normalize(difference);
            //find layer mask, assuming wall layer is 8
            int wallLayer = 1 << 8;

            //If the raycast hits nothing, add this tile to seen
            if (!Physics.Raycast(occupiedTile.transform.position + pawnYoffset, direction, distance, wallLayer))
            {
                seenTiles.Add(tile);
            }
            //next: else - 4 cardinal directions raycast.
        }
        
        //occupiedTile;
    }

    //should contian the code to actually move a character along a path in my mind.
    public virtual void MoveCharacterTo(Tile tileToMoveTo)
    {
        Debug.Log("moving Character to: Some Tile I have no way to identify I think: " + tileToMoveTo != null ? tileToMoveTo.name : "tile is null");
    }

    public float sightDistance = 0.0f;

    public Tile occupiedTile;

    public int remainingShots;

    public int maxShots;
}
