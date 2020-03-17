using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ConsiderationLists
{
    
    public static List<SingleEnemyActionConsideration> singleEnemyBasedActionConsiderationList = new List<SingleEnemyActionConsideration>()
    {
        new MoveConsideration(),
        new ShootConsideration(),
        new OverwatchConsideration(),
        new DefendConsideration()
    };
    public static List<NoEnemyActionConsideration> nonEnemyActionConsiderationList = new List<NoEnemyActionConsideration>()
    {
        new ReloadConsideration()
    };

    public static List<SingleEnemyMovementConsideration> singleEnemyMovementConsiderationList = new List<SingleEnemyMovementConsideration>()
    {
        new HitChanceDifferenceConsideration(),
        new FlankingConsideration(),
        new SelfCoverConsideration(),
        new SelfVisibilityConsideration(),
    };
    public static List<NoEnemyMovementConsideration> noEnemyMovementConsiderationList = new List<NoEnemyMovementConsideration>()
    {
        new ProximityToAllyConsideration()
    };
    private const int numberTilesToRandomlySelectFrom = 3;
    public static List<KeyValuePair<Tile, int>> topTileScoresList = new List<KeyValuePair<Tile, int>>(numberTilesToRandomlySelectFrom);

    #region Action Consideration List Functions
    public static Dictionary<ActionID, int> ConsiderActions(ref CharacterBase self)
    {
        Dictionary<ActionID, int> actionIdValuePair = new Dictionary<ActionID, int>();
        foreach (CharacterBase enemy in self.enemiesInSight)
        {
            foreach(SingleEnemyActionConsideration consideration in singleEnemyBasedActionConsiderationList)
            {
                if(actionIdValuePair.ContainsKey(consideration.actionID))
                {
                    actionIdValuePair.Add(consideration.actionID, 0);
                }
                ECoverValue agentCoverFromEnemy = self.occupiedTile.ProvidesCoverInDirection(enemy.transform.position - self.transform.position);
                ECoverValue enemtCoverFromAgent = self.occupiedTile.ProvidesCoverInDirection(self.transform.position - enemy.transform.position);
                actionIdValuePair[consideration.actionID] = consideration.CompareValues(actionIdValuePair[consideration.actionID], consideration.ConsiderAction(self, enemy, agentCoverFromEnemy, enemtCoverFromAgent));
            }
        }
        foreach(NoEnemyActionConsideration consideration in nonEnemyActionConsiderationList)
        {
            if(actionIdValuePair.ContainsKey(consideration.actionID))
            {
                actionIdValuePair.Add(consideration.actionID, 0);
            }
            actionIdValuePair[consideration.actionID] =  consideration.ConsiderAction(self);
        }
        return actionIdValuePair;
    }


    public static void SortActionList(ref Dictionary<ActionID, int> actionDictionary, ref CharacterBase self)
    {
        List<KeyValuePair<ActionID, int>> actionList = actionDictionary.ToList();
        actionList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
        int bob = UnityEngine.Random.Range(0, 2);
        if((int)actionList[bob].Key <= singleEnemyBasedActionConsiderationList.Count)
        {
            singleEnemyBasedActionConsiderationList[(int)actionList[bob].Key].Enact(self);
        }
        else
        {
            nonEnemyActionConsiderationList[(int)actionList[bob].Key - singleEnemyBasedActionConsiderationList.Count].Enact(self);
        }
    }

    #endregion

    public static int ConsiderSingleTileForMovement(ref CharacterBase self, ref Tile tileToConsider)
    {
        int totalTileValue = 0;
        foreach (CharacterBase enemy in self.enemiesInSight)
        {
            foreach (SingleEnemyMovementConsideration consideration in singleEnemyMovementConsiderationList)
            {
                totalTileValue += consideration.ConsiderTile(ref self, enemy, ref tileToConsider);
            }
        }
        foreach (NoEnemyMovementConsideration consideration in noEnemyMovementConsiderationList)
        {
            totalTileValue += consideration.ConsiderTile(ref self);
        }

        topTileScoresList.Add(new KeyValuePair<Tile, int>(tileToConsider, totalTileValue));

        topTileScoresList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
        while(topTileScoresList.Count > numberTilesToRandomlySelectFrom)
        {
            topTileScoresList.RemoveAt(numberTilesToRandomlySelectFrom);
        }
        return totalTileValue;
    }

    public static Tile GetTileToMoveTo()
    {
        Tile outTile = topTileScoresList[Random.Range(0, numberTilesToRandomlySelectFrom)].Key;
        topTileScoresList.Clear();
        return outTile;
    }
}


//To add a consideration to the enemy agent's A.I :
// 1. create {verb of Action}Consideration class as a child of the Consideration class
// 2. add to the relevant considerationList within the above ActionConsiderations class as the others have been.

public enum Weighting
{
    Low = 100,
    Medium = 300,
    High = 600,
    guarantee = 10000
}

public abstract class Consideration
{
    public Consideration(){}
    public int numberOfChecksWithinConsideration = 0;
    private float _tileValue = 0.0f;
    public float tileValue
    {
        get
        {
            return _tileValue;
        }
        set
        {
            numberOfChecksWithinConsideration++;
            _tileValue = value;
        }
    }
    virtual public float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        _tileValue = 0.0f;
        numberOfChecksWithinConsideration = 0;
        return -999.999f;
    }
}


// ConsiderActions
//{
//  foreach player in sight()
// {
//      singleMoveActionEvaluation, singleOverWatchEvaluation, singleDefendEvaluation, singleShootEvaluation
// }
// reloadEvaluation;
//}
// if moving;
// ConsiderMove
//{
//  foreach player in sight()
// {
//      hitChanceEvaluation, FlankingEvaluation, SelfCoverEvaluation, VisibilityEvaluation
// }
// proximityEvaluation;
//}

    //TODO: need a shooting Consideration to choose who to shoot?