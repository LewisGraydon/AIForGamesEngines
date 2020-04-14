using System;
using System.Collections.Generic;
using UnityEngine;

public enum ActionID
{
    Move,
    Overwatch,
    Defend,
    Shoot,
    Reload
};

public abstract class ActionConsideration : IComparable<ActionConsideration>
{
    //protected ActionID _actionID;
    //public ActionID actionID { get => _actionID; }

    protected int _actionValue;
    public int actionValue
    {
        get { return _actionValue; }
    }
    
    // TO BE REMOVED:
    public void DANGEROUSDEBUGSETACTIONVALUE(int newActionValue)
    {
        _actionValue = newActionValue;
    }

    public void ResetValue()
    {
        _actionValue = 0;
    }

    //public abstract void SetActionID();
    protected ActionConsideration()
    {
        //SetActionID();
    }

    public abstract void Enact(CharacterBase self);

    public int CompareTo(ActionConsideration other)
    {
        return (this is SingleEnemyActionConsideration) && (other is NoEnemyActionConsideration) ? -1 : (this is NoEnemyActionConsideration) && (other is SingleEnemyActionConsideration) ? 1 : 0;
    }
}

public abstract class SingleEnemyActionConsideration : ActionConsideration
{
    public abstract void ConsiderAction(CharacterBase self, CharacterBase enemy, ECoverValue agentLevelOfCoverFromEnemy, ECoverValue enemyLevelOfCoverFromAgent);
    //public abstract void CompareValue(int newValue);
}

public abstract class NoEnemyActionConsideration : ActionConsideration
{
    public abstract void ConsiderAction(CharacterBase self);
}

#region ActionConsideration classes

public class MoveConsideration : SingleEnemyActionConsideration
{
    public override void ConsiderAction(CharacterBase self, CharacterBase enemy, ECoverValue agentLevelOfCoverFromEnemy, ECoverValue enemyLevelOfCoverFromAgent)
    {
        _actionValue += self.remainingPips > 1 ? 1 : 0;
        int inCoverFromEnemiesValue = 0;
        foreach (CharacterBase c in self.enemiesInSight)
        {
            //inCoverFromEnemiesValue += (Weighting)self.occupiedTile.ProvidesCoverInDirection(self.transform.position - tileToConsider.transform.position);
            _actionValue += self.isInCover(c) ? 0 : (1 / self.enemiesInSight.Count);
        }
        _actionValue += inCoverFromEnemiesValue;
        _actionValue += self.enemiesInSight.Count < 1 ? 1 : 0;

        //if the enemy is < 1/2 range
    }

    public override void Enact(CharacterBase self)
    {
        // SELF.MOVE BAAAYYYYBBBBAAYYYY.
    }

    public override void SetActionID()
    {
        _actionID = ActionID.Move;
    }
}

//public class OverwatchConsideration : SingleEnemyActionConsideration
//{ 

//    public override int ConsiderAction(CharacterBase self, CharacterBase enemy, ECoverValue agentLevelOfCoverFromEnemy, ECoverValue enemyLevelOfCoverFromAgent)
//    {
//        // if distance to enemy is around 1/2 sight range then for this action
//        if (Vector3.Distance(self.transform.position, tileToConsider.transform.position) < (self.sightDistance / 2))
//        {
//            tileValue += (int)Weighting.Medium;
//        }

//        int AgentInCoverTotalValue = 0.0f;
//        int enemiesInCoverTotalValue = 0.0f;
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
//        tileValue += AgentInCoverTotalValue;
//        tileValue += enemiesInCoverTotalValue;


//        return tileValue;
//    }
//}

public class OverwatchConsideration : SingleEnemyActionConsideration
{
    public override void ConsiderAction(CharacterBase self, CharacterBase enemy, ECoverValue agentLevelOfCoverFromEnemy, ECoverValue enemyLevelOfCoverFromAgent)
    {

        switch (agentLevelOfCoverFromEnemy)
        {
            case ECoverValue.None:
                _actionValue -= (int)Weighting.High / self.enemiesInSight.Count; 
                break;
            case ECoverValue.Half:
                _actionValue += (int)Weighting.Low / self.enemiesInSight.Count;
                break;
            case ECoverValue.Full:
                _actionValue += (int)Weighting.High / self.enemiesInSight.Count;
                break;
            default:
                break;
        }

        switch (enemyLevelOfCoverFromAgent)
        {
            case ECoverValue.None:
                _actionValue += (int)Weighting.High / self.enemiesInSight.Count;
                break;
            case ECoverValue.Half:
                _actionValue += (int)Weighting.Low / self.enemiesInSight.Count;
                break;
            case ECoverValue.Full:
                _actionValue -= (int)Weighting.High / self.enemiesInSight.Count;
                break;
            default:
                break;
        }
    }

    public override void Enact(CharacterBase self)
    {
        self.EnterOverwatchStance();
    }

    public override void SetActionID()
    {
        _actionID = ActionID.Overwatch;
    }
}



//public class DefendConsideration : SingleEnemyActionConsideration
//{
//    public override int ConsiderAction(CharacterBase self, CharacterBase enemy, ECoverValue agentLevelOfCoverFromEnemy, ECoverValue enemyLevelOfCoverFromAgent)
//    {
//        // if distance is > 3/4 sight range then + 
//        if (Vector3.Distance(self.transform.position, tileToConsider.transform.position) > (0.75 * self.sightDistance))
//        {
//            tileValue += (int)Weighting.Medium;
//        }

//        // if agent's level of cover is full then +
//        // if the enemys cover is full then +
//        int AgentInCoverTotalValue = 0.0f;
//        int enemiesInCoverTotalValue = 0.0f;
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
//                    AgentInCoverTotalValue -= (int)Weighting.Medium / self.enemiesInSight.Count;
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
//        tileValue += AgentInCoverTotalValue;
//        tileValue += enemiesInCoverTotalValue;

//        return tileValue;
//    }
//}

public class DefendConsideration : SingleEnemyActionConsideration
{
    public override void ConsiderAction(CharacterBase self, CharacterBase enemy, ECoverValue agentLevelOfCoverFromEnemy, ECoverValue enemyLevelOfCoverFromAgent)
    {
        switch (agentLevelOfCoverFromEnemy)
        {
            case ECoverValue.None:
                _actionValue -= (int)Weighting.High / self.enemiesInSight.Count;
                break;
            case ECoverValue.Half:
                _actionValue -= (int)Weighting.Medium / self.enemiesInSight.Count;
                break;
            case ECoverValue.Full:
                _actionValue += (int)Weighting.Low / self.enemiesInSight.Count;
                break;
            default:
                break;
        }

        switch (enemyLevelOfCoverFromAgent)
        {
            case ECoverValue.None:
                _actionValue += (int)Weighting.High / self.enemiesInSight.Count;
                break;
            case ECoverValue.Half:
                _actionValue += (int)Weighting.Medium / self.enemiesInSight.Count;
                break;
            case ECoverValue.Full:
                _actionValue -= (int)Weighting.Low / self.enemiesInSight.Count;
                break;
            default:
                break;
        }
    }

    public override void Enact(CharacterBase self)
    {
        self.EnterDefenseStance();
    }

    public override void SetActionID()
    {
        _actionID = ActionID.Defend;
    }
}


//public class ShootConsideration : SingleEnemyActionConsideration
//{
//    public override int ConsiderAction(CharacterBase self, CharacterBase enemy, ECoverValue agentLevelOfCoverFromEnemy, ECoverValue enemyLevelOfCoverFromAgent)
//    {
//        /* if shot chance > 60% then +
//         * 
//         * if agent is behind cover then +
//         * 
//         * if pips remaining == 1 then +
//         * 
//         * if the enemy is not in full cover then +
//         * 
//         * 
//         */
//        if (self.occupiedTile.chanceToHit(tileToConsider) >= 60)
//        {
//            tileValue += (int)Weighting.High;
//        }
//        if (self.remainingPips == 1)
//        {
//            tileValue += (int)Weighting.Medium;
//        }
//        int AgentInCoverTotalValue = 0.0f;
//        int enemiesInCoverTotalValue = 0.0f;
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
//        tileValue += AgentInCoverTotalValue;
//        tileValue += enemiesInCoverTotalValue;

//        return tileValue;
//    }
//}

public class ShootConsideration : SingleEnemyActionConsideration
{
    private CharacterBase enemyToAttack;

    public override void ConsiderAction(CharacterBase self, CharacterBase enemy, ECoverValue agentLevelOfCoverFromEnemy, ECoverValue enemyLevelOfCoverFromAgent)
    {
        int thisEnemyShootValue = 0;

        switch (agentLevelOfCoverFromEnemy)
        {
            case ECoverValue.None:
                thisEnemyShootValue -= (int)Weighting.High;
                break;
            case ECoverValue.Half:
                thisEnemyShootValue += (int)Weighting.Medium;
                break;
            case ECoverValue.Full:
                thisEnemyShootValue += (int)Weighting.Low;
                break;
            default:
                Debug.LogError("issue with cover Value returned");
                break;
        }
        switch (enemyLevelOfCoverFromAgent)
        {
            case ECoverValue.None:
                thisEnemyShootValue += (int)Weighting.High;
                break;
            case ECoverValue.Half:
                thisEnemyShootValue += (int)Weighting.Medium;
                break;
            case ECoverValue.Full:
                thisEnemyShootValue -= (int)Weighting.Low;
                break;
            default:
                break;
        }
        if(thisEnemyShootValue > _actionValue)
        {
            enemyToAttack = enemy;
            _actionValue = thisEnemyShootValue;
        }
        //why is there no chanceToHitCalculation in here?
    }

    public override void Enact(CharacterBase self)
    {
        self.AttackCharacter(enemyToAttack);
    }

    public override void SetActionID()
    {
        _actionID = ActionID.Shoot;
    }
}

public class ReloadConsideration : NoEnemyActionConsideration
{
    public override void ConsiderAction(CharacterBase self)
    {
        // if numshots == 0 then +
        // 
        // if numshots == 0 && pips remaining == 1 then guarantee
        //
        // tileValue = 1 - (numshots / maxShots)
        if (self.remainingPips == 1)
        {
            if (self.remainingShots > (self.maxShots / 2))
            {
                _actionValue = 0;
            }
            else if (self.remainingShots > (self.maxShots / 4) && self.remainingShots != 0)
            {
                _actionValue += (int)Weighting.High;
            }
            else if (self.remainingShots == self.maxShots)
            {
                _actionValue = -(int)Weighting.guarantee;
            }
            else
            {
                _actionValue += (int)Weighting.guarantee;
            }
        }
    }

    public override void Enact(CharacterBase self)
    {
        self.Reload();
    }

    public override void SetActionID()
    {
        _actionID = ActionID.Reload;
    }
}
#endregion

