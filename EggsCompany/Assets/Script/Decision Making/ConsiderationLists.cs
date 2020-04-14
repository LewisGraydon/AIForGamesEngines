using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ConsiderationLists
{
    #region MovementList Variables
    private const int numberTilesToRandomlySelectFrom = 3;
    public static List<KeyValuePair<Tile, int>> topTileScoresList = new List<KeyValuePair<Tile, int>>(numberTilesToRandomlySelectFrom);
    public static List<MovementConsideration> movementConsiderationList = new List<MovementConsideration>()
    {
        new HitChanceDifferenceConsideration(),
        new FlankingConsideration(),
        new SelfCoverConsideration(),
        new SelfVisibilityConsideration(),
        new ProximityToAllyConsideration()
    };
    #endregion

    #region actionList Variables
    public static List<ActionConsideration> actionConsiderationList = new List<ActionConsideration>
    {
        new  MoveConsideration(),
        new  ShootConsideration(),
        new  OverwatchConsideration(),
        new  DefendConsideration(),
        new  ReloadConsideration()
    };
    #endregion

    #region Action Consideration List Functions

    //public static ActionID ConsiderActions(CharacterBase self)
    //{
    //    Dictionary<ActionID, int> actionsDictionary = new Dictionary<ActionID, int>();
    //    actionsDictionary = AssignActionValues(self);
    //    //SortActionList();
    //    return ActionID.Reload;
    //}

    //public static Dictionary<ActionID, int> AssignActionValues(CharacterBase self)
    //{
    //    Dictionary<ActionID, int> actionIdValuePair = new Dictionary<ActionID, int>();
    //    foreach (CharacterBase enemy in self.enemiesInSight)
    //    {
    //        ECoverValue agentCoverFromEnemy = self.occupiedTile.ProvidesCoverInDirection(enemy.transform.position - self.transform.position);
    //        ECoverValue enemtCoverFromAgent = self.occupiedTile.ProvidesCoverInDirection(self.transform.position - enemy.transform.position);
    //        foreach (SingleEnemyActionConsideration consideration in singleEnemyBasedActionConsiderationList)
    //        {
    //            if(actionIdValuePair.ContainsKey(consideration.actionID))
    //            {
    //                actionIdValuePair.Add(consideration.actionID, 0);
    //            }
    //            actionIdValuePair[consideration.actionID] = consideration.CompareValue(actionIdValuePair[consideration.actionID], consideration.ConsiderAction(self, enemy, agentCoverFromEnemy, enemtCoverFromAgent));
    //        }
    //    }
    //    foreach(NoEnemyActionConsideration consideration in nonEnemyActionConsiderationList)
    //    {
    //        if(actionIdValuePair.ContainsKey(consideration.actionID))
    //        {
    //            actionIdValuePair.Add(consideration.actionID, 0);
    //        }
    //        actionIdValuePair[consideration.actionID] = consideration.ConsiderAction(self);
    //    }
    //    return actionIdValuePair;
    //}

    public static void AssignActionValues(CharacterBase self)
    {
        //sort list so that the singleEnemyActionConsiderations are all at the front of the list, custom CompareTo function within the ActionConsideration Class should changes need to be made to order or new child class is created.
        actionConsiderationList.Sort((ActionConsideration val1, ActionConsideration val2) =>
        {
            return val1.CompareTo(val2);
        });

        for (int i = 0; i < actionConsiderationList.Count; i++)
        {

            for (int enemyCounter = 0; enemyCounter < self.enemiesInSight.Count; enemyCounter++)
            {
                //foreach enemy, calculate the cover from and for the agent (could be expensive hence this is calculated once and the actions are the more commonly calculated part.
                ECoverValue agentCoverFromEnemy = self.occupiedTile.ProvidesCoverInDirection(self.enemiesInSight[enemyCounter].transform.position - self.transform.position);
                ECoverValue enemyCoverFromAgent = self.occupiedTile.ProvidesCoverInDirection(self.transform.position - self.enemiesInSight[enemyCounter].transform.position);
                int startI = i;
                while (actionConsiderationList[i] is SingleEnemyActionConsideration)
                {
                    (actionConsiderationList[i] as SingleEnemyActionConsideration).ConsiderAction(self, self.enemiesInSight[enemyCounter], agentCoverFromEnemy, enemyCoverFromAgent);
                    i++;
                }
                i = enemyCounter != (self.enemiesInSight.Count - 1) ? startI // if not the end of the enemies then reset the i counter;
                                    : i + 1 < actionConsiderationList.Count ? i + 1 // if the end of the enemies but not the last consideration, increment I for the rest of the consideration checks.
                                    : i; // end of consideration list so leave as last i so the outer for loop ends as normal. 
            }
            if(actionConsiderationList[i] is NoEnemyActionConsideration)
            {
                (actionConsiderationList[i] as NoEnemyActionConsideration).ConsiderAction(self);
            }
        }
        //actions all evaluated by this point
        actionConsiderationList.Sort((ActionConsideration consideration1, ActionConsideration consideration2) =>
        {
            return consideration2.actionValue.CompareTo(consideration1.actionValue);
        }); //order list by value descending;
        actionConsiderationList[UnityEngine.Random.Range(0, (self as EnemyCharacter).actionVariance)].Enact(self);
    }

    //public static void EnactTopAction(CharacterBase self)
    //{
    //    actionConsiderationList.Sort((ActionConsideration consideration1, ActionConsideration consideration2) =>
    //    {
    //        return consideration2.actionValue.CompareTo(consideration1.actionValue);
    //    }); //order list by value descending;
    //    int actionNumber = UnityEngine.Random.Range(0, (self as EnemyCharacter).actionVariance);
    //    actionConsiderationList[actionNumber].Enact(self);
    //}

    //public static List<KeyValuePair<ActionID, int>> SortActionList(ref Dictionary<ActionID, int> actionDictionary, CharacterBase self)
    //{
    //    List<KeyValuePair<ActionID, int>> actionList = actionDictionary.ToList();
    //    actionList.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));
    //    return actionList;
    //    int bob = UnityEngine.Random.Range(0, 2);
    //    if ((int)actionList[bob].Key <= singleEnemyBasedActionConsiderationList.Count)
    //    {
    //        singleEnemyBasedActionConsiderationList[(int)actionList[bob].Key].Enact(self);
    //    }
    //    else
    //    {
    //        nonEnemyActionConsiderationList[(int)actionList[bob].Key - singleEnemyBasedActionConsiderationList.Count].Enact(self);
    //    }
    //}

    //public static void EnactRandomAction(int actionVariance, CharacterBase characterToTakeAction, List<KeyValuePair<ActionID, int>> actionList)
    //{
    //    int randomActionSelection = UnityEngine.Random.Range(0, actionVariance);
    //    if ((int)actionList[randomActionSelection].Key <= singleEnemyBasedActionConsiderationList.Count)
    //    {
    //        singleEnemyBasedActionConsiderationList[(int)actionList[randomActionSelection].Key].Enact(characterToTakeAction);
    //    }
    //    else
    //    {
    //        nonEnemyActionConsiderationList[(int)actionList[randomActionSelection].Key - singleEnemyBasedActionConsiderationList.Count].Enact(characterToTakeAction);
    //    }
    //    // alternately:
    //    if (actionList[randomActionSelection] is SingleEnemyActionConsideration)
    //    {

    //    }
        
    //}

    #endregion

    #region movement list functions

    public static void ConsiderSingleTileForMovement(CharacterBase self, Tile tileToConsider)
    {
        movementConsiderationList.Sort((MovementConsideration consideration1, MovementConsideration consideration2) =>
        {
            return consideration1.CompareTo(consideration2);
        });

        int tilesValue = 0;
        for (int i = 0; i < movementConsiderationList.Count; i++)
        {
            if(movementConsiderationList[i] is SingleEnemyMovementConsideration)
            {
                foreach(CharacterBase enemy in self.enemiesInSight)
                {
                    (movementConsiderationList[i] as SingleEnemyMovementConsideration).ConsiderTile(ref self, enemy, ref tileToConsider);
                } //possibly not ideal as calculating similar values given number of player characters seen a lot of times but in case some of the values replace rather than add it is necessary.
            }
            else if(movementConsiderationList[i] is TileOnlyMovementConsideration)
            {
                (movementConsiderationList[i] as TileOnlyMovementConsideration).ConsiderTile(ref tileToConsider);
            }
            tilesValue += movementConsiderationList[i].movementValue;
        }

        topTileScoresList.Add(new KeyValuePair<Tile, int>(tileToConsider, tilesValue));

        topTileScoresList.Sort((pair1, pair2) =>
        {
            return pair2.Value.CompareTo(pair1.Value);
        });
        while(topTileScoresList.Count > numberTilesToRandomlySelectFrom)
        {
            topTileScoresList.RemoveAt(numberTilesToRandomlySelectFrom);
        }
    }

    //public static int ConsiderSingleTileForMovement(CharacterBase self, Tile tileToConsider)
    //{
        //int totalTileValue = 0;
        //foreach (CharacterBase enemy in self.enemiesInSight)
        //{
        //    foreach (SingleEnemyMovementConsideration consideration in singleEnemyMovementConsiderationList)
        //    {
        //        totalTileValue += consideration.ConsiderTile(ref self, enemy, ref tileToConsider);
        //    }
        //}
        //foreach (NoEnemyMovementConsideration consideration in noEnemyMovementConsiderationList)
        //{
        //    totalTileValue += consideration.ConsiderTile(ref self);
        //}

        //topTileScoresList.Add(new KeyValuePair<Tile, int>(tileToConsider, totalTileValue));

        //topTileScoresList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
        //while(topTileScoresList.Count > numberTilesToRandomlySelectFrom)
        //{
        //    topTileScoresList.RemoveAt(numberTilesToRandomlySelectFrom);
        //}
        //return totalTileValue;
    //}

    public static Tile GetTileToMoveTo()
    {
        Tile outTile = topTileScoresList[Random.Range(0, numberTilesToRandomlySelectFrom)].Key;
        topTileScoresList.Clear();
        return outTile;
    }
    #endregion
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


// ConsiderActions OLD IMPLEMENTATION IDEA
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


#region Messy initial  implementation for various types of action and movement; KEPT FOR PROSPERITY AND MEMORY AIDING;
// initially was going to have many different lists for each type of consideration for action then loop through them one by one to consider all possible actions etc.
// would be awkward to use and did not take full advantage of the inheritance

//    public static List<SingleEnemyActionConsideration> singleEnemyBasedActionConsiderationList = new List<SingleEnemyActionConsideration>()
//    {
//        new MoveConsideration(),
//        new ShootConsideration(),
//        new OverwatchConsideration(),
//        new DefendConsideration()
//    };
//    public static List<NoEnemyActionConsideration> nonEnemyActionConsiderationList = new List<NoEnemyActionConsideration>()
//    {
//        new ReloadConsideration()
//    };
//    public static List<SingleEnemyMovementConsideration> singleEnemyMovementConsiderationList = new List<SingleEnemyMovementConsideration>()
//    {
//        new HitChanceDifferenceConsideration(),
//        new FlankingConsideration(),
//        new SelfCoverConsideration(),
//        new SelfVisibilityConsideration(),
//    };
//    public static List<NoEnemyMovementConsideration> noEnemyMovementConsiderationList = new List<NoEnemyMovementConsideration>()
//{
//    new ProximityToAllyConsideration()
//};
#endregion