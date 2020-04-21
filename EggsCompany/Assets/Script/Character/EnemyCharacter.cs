using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : CharacterBase
{
    public string _characterName;
    public PathfindingAgent pathfinder;
    public string characterName
    {
        get { return _characterName; }
    }

    // Start is called before the first frame update
    void Start()
    {
        onPlayerTeam = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int actionVariance = 1;

    void MakeDecision()
    {
        //ConsiderationLists.ConsiderActions(this);
        ConsiderationLists.MakeDecision(this);
    }
        
        

    public void moveDecision()
    {
        Debug.Log("Doing A Move Decision");
        if(pathfinder != null)
        {
            pathfinder.FindMovementRange(occupiedTile, 100, ConsiderationLists.ConsiderSingleTileForMovement, this);
            this.MoveCharacterTo(ConsiderationLists.GetTileToMoveTo());
        }
        else
        {
            Debug.LogWarning("pathfinder variable on enemy character is not set");
        }
    }
}
