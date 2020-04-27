using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class CharacterBase : MonoBehaviour
{
    protected GameObject gsm;
    protected GameState gsmScript;

    protected Canvas UICanvas;
    protected Text HealthText;
    protected Text ActionPipsText;
    void Start()
    {
        
        //this.setRemainingHealth(4);
        //this.SetRemainingPips(1);
        //Debug.Log("HEALTH = 4, Pips = 1");
        //this.setRemainingHealth(0);
        //this.SetRemainingPips(0);
        //Debug.Log("HEALTH = 0, Pips = 0");
        //this.setRemainingHealth(-1);
        //this.SetRemainingPips(4);
        //Debug.Log("HEALTH = -1, Pips = 4");
        this.setRemainingHealth(7);
        this.SetRemainingPips(-1);
        Debug.Log("HEALTH = 7, Pips = -1");
        gsm = GameObject.Find("GameStateManager");
        gsmScript = gsm.GetComponent<GameState>();
    }

    private void Awake()
    {
        Text[] allAttachedTexts = gameObject.GetComponentsInChildren<Text>();
        foreach (Text text in allAttachedTexts)
        {
            if (text.text.Contains("HEALTH"))
            {
                HealthText = text;
                setRemainingHealth(maximumHealth);
            }
            else if (text.text.Contains("ACTION"))
            {
                ActionPipsText = text;
                SetRemainingPips(maximumActionPips);
            }
            if (HealthText != null && ActionPipsText != null)
            {
                break;
            }
        }
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
        //if(pipsRemaining > maxActionPips)
        //{
        //    actionPips = maxActionPips;
        //    return;
        //}
        actionPips = Mathf.Clamp(pipsRemaining, 0, maxActionPips);
        if(ActionPipsText != null)
        {
            ActionPipsText.text = "ACTION PIPS: " + actionPips;
        }
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
    public void setRemainingHealth(int newHealthValue)
    {
        health = Mathf.Clamp(newHealthValue, 0, maximumHealth);
        if (HealthText != null)
        {
            HealthText.text = "HEALTH: " + health;
        }
    }

    protected int maxHealthValue = 6;
    public int maximumHealth
    {
        get { return maxHealthValue; }
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
    public virtual void MoveCharacterTo(Tile tileToMoveTo)
    {
        Debug.Log("moving Character to: Some Tile I have no way to identify I think: " + tileToMoveTo != null ? tileToMoveTo.name : "tile is null");
    }

    public float sightDistance = 0.0f;

    public Tile occupiedTile;

    public int remainingShots;

    public int maxShots;

    public void faceCanvasToCamera()
    {
        if(HealthText != null)
        {
            HealthText.canvas.transform.LookAt(gameObject.transform.position + Camera.main.transform.rotation * UnityEngine.Vector3.forward, Camera.main.transform.rotation * UnityEngine.Vector3.up);
        }
    }
}
