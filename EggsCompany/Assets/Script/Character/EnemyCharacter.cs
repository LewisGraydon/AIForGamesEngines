using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyCharacter : CharacterBase
{
    public string _characterName;
    public string characterName { get => _characterName; }

    private PathfindingAgent pathfindingAgent;
    public int actionVariance = 1;
    public EnemyCharacter()
    {
        pathfindingAgent = null;
    }
    public EnemyCharacter(PathfindingAgent inPathfindingAgent)
    {
        pathfindingAgent = inPathfindingAgent;
    }
    void MakeDecision()
    {
        ConsiderationLists.MakeDecision(this);
    }

    public void moveDecision()
    {
        Debug.Log("Doing A Move Decision");
        if(pathfindingAgent != null)
        {
            List<INodeSearchable> allPossibleTiles = pathfindingAgent.FindMovementRange(occupiedTile, 100, ConsiderationLists.ConsiderSingleTileForMovement, this);
            Tile tileToMoveToHolder = ConsiderationLists.GetTileToMoveTo();
            if (!allPossibleTiles.Contains(tileToMoveToHolder as INodeSearchable))
            {
                Debug.LogError("Tile chosen to move to: " + tileToMoveToHolder + " not contained within the allPossibleTiles from FindMovementRangeFunction");
                return;
            }
            else
            {
                SetMovementStack(pathfindingAgent.CreatePath(tileToMoveToHolder), allPossibleTiles);
                //TODO: set turnorder to movement;
            }
        }
        else
        {
            Debug.LogWarning("pathfinder variable on enemy character: " + name + " is not set");
        }
    }
}
