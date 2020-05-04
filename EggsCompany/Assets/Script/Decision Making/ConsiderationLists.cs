using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
public enum Weighting
{
    Low = 100,
    Medium = 300,
    High = 600,
    guarantee = 10000
}

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
    public static void MakeDecision(CharacterBase self)
    {
        //sort list so that the singleEnemyActionConsiderations are all at the front of the list, custom CompareTo function within the ActionConsideration Class should changes need to be made to order or new child class is created.
        actionConsiderationList.Sort((ActionConsideration val1, ActionConsideration val2) =>
        {
            return val1.CompareTo(val2);
        });

        for (int i = 0; i < actionConsiderationList.Count; i++)
        {
            if(self.enemiesInSight.Count != 0)
            {
                foreach (KeyValuePair<CharacterBase, int> EnemyHitChancePair in self.enemiesInSight)
                {
                    //foreach enemy, calculate the cover from and for the agent (could be expensive hence this is calculated once and the actions are the more commonly calculated part.
                    ECoverValue agentCoverFromEnemy = self.occupiedTile.ProvidesCoverInDirection(EnemyHitChancePair.Key.gameObject.transform.position - self.transform.position);
                    ECoverValue enemyCoverFromAgent = self.occupiedTile.ProvidesCoverInDirection(self.transform.position - EnemyHitChancePair.Key.gameObject.transform.position);
                    int firstSingleEnemyActionConsiderationIndex = i;
                    while (actionConsiderationList[i] is SingleEnemyActionConsideration)
                    {
                        (actionConsiderationList[i] as SingleEnemyActionConsideration).ConsiderAction(self, EnemyHitChancePair.Key, agentCoverFromEnemy, enemyCoverFromAgent);
                        i++;
                    }
                    i = EnemyHitChancePair.Key != self.enemiesInSight.ElementAt(self.enemiesInSight.Count - 1).Key ? firstSingleEnemyActionConsiderationIndex // if not the end of the enemies then reset the i counter;
                                                                        : i;
                }
            }
            else
            {
                while (actionConsiderationList[i] is SingleEnemyActionConsideration)
                {
                    (actionConsiderationList[i] as SingleEnemyActionConsideration).ConsiderActionWithNoEnemyInSight(self);
                    i++;
                }
            }
            if(i <= actionConsiderationList.Count && actionConsiderationList[i] is NoEnemyActionConsideration)
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
        foreach(ActionConsideration ac in actionConsiderationList)
        {
            ac.ResetValue();
        }
    }
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
                foreach(KeyValuePair<CharacterBase, int> enemyHitChancePair in self.enemiesInSight)
                {
                    (movementConsiderationList[i] as SingleEnemyMovementConsideration).ConsiderTile(ref self, enemyHitChancePair.Key, ref tileToConsider);
                } //calculates for all seen enemies per consideration as some considerations replace?
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

    public static Tile GetTileToMoveTo()
    {
        Tile outTile = topTileScoresList[Random.Range(0, numberTilesToRandomlySelectFrom)].Key;
        topTileScoresList.Clear();
        return outTile;
    }
    #endregion
}
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