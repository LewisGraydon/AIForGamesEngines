using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveFactorID
{
    HitChance,
    Flanking,
    SelfCover,
    SelfVisibility,
    AllyProximity
};


public abstract class MovementConsideration
{
    protected MoveFactorID _moveFactorID;
    public MoveFactorID moveFactorID { get => _moveFactorID; }

    public abstract void setMoveFactorID();

    protected MovementConsideration()
    {
        setMoveFactorID();
    }

}

public abstract class SingleEnemyMovementConsideration : MovementConsideration
{
    public abstract int ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider);

    public abstract int CompareValues(int oldValue, int newValue);
}

public abstract class NoEnemyMovementConsideration : MovementConsideration
{
    public abstract int ConsiderTile(ref CharacterBase self);
}


#region MovementConsideration classes

//public class HitChanceDifferenceConsideration : MovementConsideration
//{
//    public override int ConsiderTile(CharacterBase self, Tile tileToConsider)
//    {
//        base.ConsiderTile(self, tileToConsider);
//        /*
//         * pipCost
//         * foreach seen player;
//         * random value for accuracy
//         *
//         * estimatedHitChance = isAccurate ? hitChanceFrom(tileToConsider) : randomNumber(1,100);
//         * 
//         * if estimatedHitChance > hitChanceFrom(self.currentTile) then + else - 
//        */

//        //TODO GET PIP COST FROM PATHFINDING
//        bool accuracyOfPrediction = Random.Range(0, 1) == 1;
//        int totalHitChanceDifference = 0;
//        foreach (CharacterBase enemy in self.enemiesInSight)
//        {
//            int estimatedHitChance = accuracyOfPrediction ? tileToConsider.chanceToHit(enemy.occupiedTile) : Random.Range(0, 100);
//            if (estimatedHitChance > self.occupiedTile.chanceToHit(enemy.occupiedTile))
//            {
//                totalHitChanceDifference += ((int)Weighting.High / self.enemiesInSight.Count);
//            }
//            else if (estimatedHitChance == 100)
//            {
//                totalHitChanceDifference += ((int)Weighting.guarantee / self.enemiesInSight.Count);
//            }
//            else
//            {
//                totalHitChanceDifference -= ((int)Weighting.Medium / self.enemiesInSight.Count);
//            }
//        }
//        return totalHitChanceDifference;
//    }
//}
public class HitChanceDifferenceConsideration : SingleEnemyMovementConsideration
{
    public override int CompareValues(int oldValue, int newValue)
    {
        return oldValue + newValue;
    }

    public override int ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider)
    {
        int tileValue = 0;
        bool isPredictonAccurate = Random.Range(0, 1) == 1;
        int estimatedHitChance = isPredictonAccurate ? tileToConsider.chanceToHit(enemy.occupiedTile) : Random.Range(0, 100);

        tileValue += estimatedHitChance == 100 ? (int)Weighting.guarantee : 
                                                    estimatedHitChance > self.occupiedTile.chanceToHit(enemy.occupiedTile) ? (int)Weighting.High :
                                                    -(int)Weighting.High;
        return tileValue;
    }

    public override void setMoveFactorID()
    {
        _moveFactorID = MoveFactorID.HitChance;
    }
}


//public class FlankingConsideration : MovementConsideration
//{
//    public override int ConsiderTile(CharacterBase self, Tile tileToConsider) //UNSURE IF  THIS IS NEEDED AS THE HITCHANCE SHOULD BE JUMPED BY THIS
//    {
//        base.ConsiderTile(self, tileToConsider);
//        /*
//         * 
//         * pipCost
//         * foreach seen player
//         * 
//         * if(tileToConsider.isFlanking(player)) then almost guarantee this choice
//         * 
//         * if(player.tile.isFlanking(tileToConsider) then remove some equity
//         * 
//         * 
//        */

//        //TODO GET PIP COST FROM PATHFINDNG
//        int playersFlankedTotalValue = 0;
//        foreach (CharacterBase enemy in self.enemiesInSight)
//        {
//            if (self.occupiedTile.transform.position.x == enemy.transform.position.x ||
//                self.occupiedTile.transform.position.y == enemy.transform.position.y)
//            {
//                playersFlankedTotalValue += ((int)Weighting.High / self.enemiesInSight.Count);
//            }
//        }
//        return playersFlankedTotalValue;
//    }
//}
public class FlankingConsideration : SingleEnemyMovementConsideration
{
    public override int CompareValues(int oldValue, int newValue)
    {
        return oldValue + newValue;
    }

    public override int ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider)
    {
        return self.occupiedTile.transform.position.x == enemy.transform.position.x || self.occupiedTile.transform.position.y == enemy.transform.position.y ?
               (int)Weighting.guarantee : 0;
    }

    public override void setMoveFactorID()
    {
        _moveFactorID = MoveFactorID.Flanking;
    }
}

//public class SelfCoverConsideration : MovementConsideration
//{
//    public override int ConsiderTile(CharacterBase self, Tile tileToConsider)
//    {
//        base.ConsiderTile(self, tileToConsider);
//        /*
//         * pipCost
//         * foreach player
//         * 
//         * is tileToConsider.providingCoverFrom(player);
//         * 
//        */
//        int AgentInCoverTotalValue = 0;
//        int enemiesInCoverTotalValue = 0;
//        foreach (CharacterBase enemy in self.enemiesInSight)
//        {
//            //TODO: calculate direction from enemy to self.
//            // if the agent is in good cover from the enemies then + for this action
//            switch (tileToConsider.ProvidesCoverInDirection(Vector3.forward)) //switch on amount of cover self is in from enemies;
//            {
//                case ECoverValue.None:
//                    AgentInCoverTotalValue -= (int)Weighting.High / self.enemiesInSight.Count;
//                    break;
//                case ECoverValue.Half:
//                    AgentInCoverTotalValue += (int)Weighting.Medium / self.enemiesInSight.Count;
//                    break;
//                case ECoverValue.Full:
//                    AgentInCoverTotalValue += (int)Weighting.Low / self.enemiesInSight.Count;
//                    break;
//                default:
//                    Debug.LogError("issue with cover Value returned");
//                    break;
//            }

//            // if the enemies are not in full cover then + for this action as expect them to move
//            switch (enemy.occupiedTile.ProvidesCoverInDirection(Vector3.back))
//            {
//                case ECoverValue.None:
//                    enemiesInCoverTotalValue += (int)Weighting.High / self.enemiesInSight.Count;
//                    break;
//                case ECoverValue.Half:
//                    enemiesInCoverTotalValue += (int)Weighting.Medium / self.enemiesInSight.Count;
//                    break;
//                case ECoverValue.Full:
//                    enemiesInCoverTotalValue -= (int)Weighting.Low / self.enemiesInSight.Count;
//                    break;
//                default:
//                    break;
//            }
//        }
//        return 0;
//    }
//}

public class SelfCoverConsideration : SingleEnemyMovementConsideration
{
    public override int CompareValues(int oldValue, int newValue)
    {
        return oldValue + newValue;
    }

    public override int ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider)
    {
        ECoverValue tileCoverFromEnemy = tileToConsider.ProvidesCoverInDirection(enemy.transform.position - self.transform.position);
        ECoverValue enemyCoverFromTile = enemy.occupiedTile.ProvidesCoverInDirection(self.transform.position - enemy.transform.position);
        int tileValue = 0;
        switch (tileCoverFromEnemy)
        {
            case ECoverValue.None:
                tileValue -= (int)Weighting.High / self.enemiesInSight.Count;
                break;
            case ECoverValue.Half:
                tileValue += (int)Weighting.Medium / self.enemiesInSight.Count;
                break;
            case ECoverValue.Full:
                tileValue += (int)Weighting.Low / self.enemiesInSight.Count;
                break;
            default:
                Debug.LogError("issue with cover Value returned");
                break;
        }

        switch (enemyCoverFromTile)
        {
            case ECoverValue.None:
                tileValue += (int)Weighting.High / self.enemiesInSight.Count;
                break;
            case ECoverValue.Half:
                tileValue += (int)Weighting.Medium / self.enemiesInSight.Count;
                break;
            case ECoverValue.Full:
                tileValue -= (int)Weighting.Low / self.enemiesInSight.Count;
                break;
            default:
                break;
        }

        return tileValue;
    }

    public override void setMoveFactorID()
    {
        throw new System.NotImplementedException();
    }
}

//public class SelfVisibilityConsideration : MovementConsideration
//{
//    public override int ConsiderTile(CharacterBase self, Tile tileToConsider)
//    {
//        base.ConsiderTile(self, tileToConsider);
//        /* foreach player
//         * 
//         * if tileToConsider.visibleFromTile
//         *  if cowardly/random thing
//         *      +
//         * else
//         *  -
//        */
//        int totalSightValue = 0;
//        bool predicitonAccuracy = Random.Range(0, 1) == 1;
//        foreach (CharacterBase enemy in self.enemiesInSight)
//        {
//            bool willBeVisibleInNewTile = predicitonAccuracy ? tileToConsider.isVisibleFromTile(enemy.occupiedTile) : Random.Range(0, 1) == 0;
//            totalSightValue += willBeVisibleInNewTile ? -((int)Weighting.Medium / self.enemiesInSight.Count) : ((int)Weighting.Medium / self.enemiesInSight.Count);
//        }

//        return totalSightValue;
//    }
//}

public class SelfVisibilityConsideration : SingleEnemyMovementConsideration
{
    public override int CompareValues(int oldValue, int newValue)
    {
        return oldValue + newValue;
    }

    public override int ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider)
    {
        bool isPredictonAccurate = Random.Range(0, 1) == 1;
        bool willEnemyBeVisisbleFromTile = isPredictonAccurate ? tileToConsider.isVisibleFromTile(enemy.occupiedTile) : Random.Range(0, 1) == 0;
        return willEnemyBeVisisbleFromTile ? -((int)Weighting.Medium / self.enemiesInSight.Count) : ((int)Weighting.Medium / self.enemiesInSight.Count);
    }

    public override void setMoveFactorID()
    {
        _moveFactorID = MoveFactorID.SelfVisibility;
    }
}


//public class ProximityToAllyConsideration : MovementConsideration
//{
//    public override int ConsiderTile(CharacterBase self, Tile tileToConsider)
//    {
//        base.ConsiderTile(self, tileToConsider);
//        /*
//            * search within two squares for occupied by enemy of tileToConsider then (unless melee type enemy) -
//            *  
//            *  else 
//            *  
//            *  +
//            * 
//        */
//        return -(int)Weighting.Medium * PathfindingAgent.BreadthFirstAllySearch((INodeSearchable)self.occupiedTile);

//        //foreach (EDirection dir in Enum.GetValues(typeof(EDirection)))
//        //{

//        //}
//        //foreach(Tile tile in self.occupiedTile)
//    }
//}

public class ProximityToAllyConsideration : NoEnemyMovementConsideration
{
    public override int ConsiderTile(ref CharacterBase self)
    {
        return -(int)Weighting.Medium * PathfindingAgent.BreadthFirstAllySearch((INodeSearchable)self.occupiedTile);
    }

    public override void setMoveFactorID()
    {
        _moveFactorID = MoveFactorID.AllyProximity;
    }
}

#endregion
