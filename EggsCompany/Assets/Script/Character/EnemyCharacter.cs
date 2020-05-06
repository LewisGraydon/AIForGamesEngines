using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemyCharacter : CharacterBase
{
    #region Variables
    public string _characterName;
    public string characterName { get => _characterName; }

    private PathfindingAgent pathfindingAgent;
    public int actionVariance = 2;
    #endregion

    protected new void Awake()
    {
        base.Awake();
        pathfindingAgent = gsmScript.pathfindingAgent;
    }

    public void MakeDecision()
    {
        ConsiderationLists.MakeDecision(this);
    }

    public void moveDecision()
    {
        Debug.Log("Doing A Move Decision");
        if(pathfindingAgent != null)
        {

            List<INodeSearchable> allPossibleTiles = pathfindingAgent.FindMovementRange(occupiedTile, this.MovementRange, ConsiderationLists.ConsiderSingleTileForMovement, this);
            Tile tileToMoveToHolder = ConsiderationLists.GetTileToMoveTo();
            if (!allPossibleTiles.Contains(tileToMoveToHolder as INodeSearchable))
            {
                Debug.LogError("Tile chosen to move to: " + tileToMoveToHolder + " not contained within the allPossibleTiles from FindMovementRangeFunction");
                return;
            }
            else
            {
                gsmScript.gameState = EGameState.movement;
                Stack<INodeSearchable> pathToTake = pathfindingAgent.CreatePath(tileToMoveToHolder);
                pathfindingAgent.NodeReset(allPossibleTiles);
                SetMovementStack(pathToTake, allPossibleTiles);
            }
        }
        else
        {
            Debug.LogWarning("pathfinder variable on enemy character: " + name + " is not set");
        }
    }

    public override void EnterOverwatchStance()
    {
        Debug.Log(this + "  has eyes out... (overwatch)");

        gsmScript.enemyContainer.GetComponent<EnemyManager>().overwatchingEnemies.Add(this);

        base.EnterOverwatchStance();

    }

}
