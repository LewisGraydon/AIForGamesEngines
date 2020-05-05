using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region parent classes

public abstract class ActionConsideration : IComparable<ActionConsideration>
{
    protected int _actionValue;
    public int actionValue
    {
        get { return _actionValue; }
    }

    public void ResetValue()
    {
        _actionValue = 0;
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

    /// <summary>
    /// Attribute score value to the action based solely on the character usually using solely the occupied tile or nothing.
    /// </summary>
    /// <param name="self">the character to be evaluating the decision</param>
    public abstract void ConsiderActionWithNoEnemyInSight(CharacterBase self);
}

public abstract class NoEnemyActionConsideration : ActionConsideration
{
    public abstract void ConsiderAction(CharacterBase self);
}
#endregion

#region ActionConsideration classes

public class MoveConsideration : SingleEnemyActionConsideration
{
    public override void ConsiderAction(CharacterBase self, CharacterBase enemy, ECoverValue agentLevelOfCoverFromEnemy, ECoverValue enemyLevelOfCoverFromAgent)
    {
        #region Remaining Pips Check
        _actionValue += self.actionPips > 1 ? ((int)Weighting.High / self.enemiesInSight.Count) : 0;
        #endregion

        #region Proximity To Players Check
        float distanceBetween = Vector3.Distance(self.transform.position, enemy.transform.position);
        if(distanceBetween <= self.closeDistanceCap)
        {
            _actionValue -= ((int)Weighting.Medium / self.enemiesInSight.Count);
        }
        else if (distanceBetween <= self.middleDistanceCap)
        {
            _actionValue += ((int)Weighting.Low / self.enemiesInSight.Count);
        }
        else
        {
            _actionValue += ((int)Weighting.Medium / self.enemiesInSight.Count);
        }
        #endregion

        #region Enemy Overwatch Check
        if(enemy.isOverwatching)
        {
            _actionValue += ((int)Weighting.Medium / self.enemiesInSight.Count);
        }
        #endregion

        #region Agent Cover Check
        switch (agentLevelOfCoverFromEnemy)
        {
            case ECoverValue.None:
                _actionValue += ((int)Weighting.High / self.enemiesInSight.Count);
                break;
            case ECoverValue.Half:
                _actionValue += ((int)Weighting.Medium / self.enemiesInSight.Count);
                break;
            case ECoverValue.Full:
                _actionValue -= ((int)Weighting.Low / self.enemiesInSight.Count);
                break;
            default:
                break;
        }
        #endregion
        //_actionValue += self.enemiesInSight.Count < 2 ? (int)Weighting.Medium : -(int)Weighting.Low;
    }

    public override void ConsiderActionWithNoEnemyInSight(CharacterBase self)
    {
        _actionValue += (int)Weighting.Medium;
        #region Remaining Pips Check
        _actionValue += self.actionPips > 1 ? (int)Weighting.High : 0;
        #endregion

        #region Agent Generic Cover Check
        switch (self.occupiedTile.GetGenericCoverValue())
        {
            case ECoverValue.None:
                _actionValue += (int)Weighting.High;
                break;
            case ECoverValue.Half:
                _actionValue += (int)Weighting.High;
                break;
            case ECoverValue.Full:
                _actionValue += (int)Weighting.Medium; 
                break;
            case ECoverValue.Error:
                _actionValue += (int)Weighting.guarantee; 
                break;
            default:
                break;
        }
        #endregion

        #region Agent Ally Proximity Check
        int nearbyAllies = PathfindingAgent.BreadthFirstAllySearch(self.occupiedTile);
        _actionValue += nearbyAllies > 2 ? -(int)Weighting.Medium : (int)Weighting.High;
        #endregion
    }

    public override void Enact(CharacterBase self)
    {
        Debug.Log("Character: " + self.name + " is moving");
        (self as EnemyCharacter).moveDecision();
    }
}

public class OverwatchConsideration : SingleEnemyActionConsideration
{
    public override void ConsiderAction(CharacterBase self, CharacterBase enemy, ECoverValue agentLevelOfCoverFromEnemy, ECoverValue enemyLevelOfCoverFromAgent)
    {
        #region Agent Cover Check
        switch (agentLevelOfCoverFromEnemy)
        {
            case ECoverValue.None:
                _actionValue -= ((int)Weighting.High / self.enemiesInSight.Count); 
                break;
            case ECoverValue.Half:
                _actionValue += ((int)Weighting.Low / self.enemiesInSight.Count);
                break;
            case ECoverValue.Full:
                _actionValue += ((int)Weighting.High / self.enemiesInSight.Count);
                break;
            default:
                break;
        }
        #endregion

        #region PlayerCharacter Cover Check
        switch (enemyLevelOfCoverFromAgent)
        {
            case ECoverValue.None:
                _actionValue += ((int)Weighting.High / self.enemiesInSight.Count);
                break;
            case ECoverValue.Half:
                _actionValue += ((int)Weighting.Low / self.enemiesInSight.Count);
                break;
            case ECoverValue.Full:
                _actionValue -= ((int)Weighting.Medium / self.enemiesInSight.Count);
                break;
            default:
                break;
        }
        #endregion

        #region Ammo Check
        _actionValue += self.ammunition >= (self.MaximumAmmunition >> 1) ? ((int)Weighting.Medium / self.enemiesInSight.Count) : -((int)Weighting.Medium / self.enemiesInSight.Count);
        #endregion

        #region Proximity To Players Check
        float distanceBetween = Vector3.Distance(self.transform.position, enemy.transform.position);
        if (distanceBetween <= self.closeDistanceCap)
        {
            _actionValue -= ((int)Weighting.Medium / self.enemiesInSight.Count);
        }
        else if (distanceBetween <= self.middleDistanceCap)
        {
            _actionValue += ((int)Weighting.Medium / self.enemiesInSight.Count);
        }
        else
        {
            _actionValue += ((int)Weighting.Low / self.enemiesInSight.Count);
        }
        #endregion
    }

    public override void ConsiderActionWithNoEnemyInSight(CharacterBase self)
    {
        #region Agent Generic Cover Check
        for (int i = 0; i < (int)EWallDirection.Error; i++)
        {
            if (self.occupiedTile.walls[i].coverValue == 0)
            {
                _actionValue -= (int)Weighting.Medium;
            }
            else if (self.occupiedTile.walls[i].coverValue == (int)ECoverValue.Half)
            {
                _actionValue += (int)Weighting.Medium;
            }
            else if (self.occupiedTile.walls[i].coverValue == (int)ECoverValue.Full)
            {
                _actionValue += (int)Weighting.High;
            }
        }
        #endregion
    }

    public override void Enact(CharacterBase self)
    {
        self.EnterOverwatchStance();
    }
}

public class DefendConsideration : SingleEnemyActionConsideration
{
    public override void ConsiderAction(CharacterBase self, CharacterBase enemy, ECoverValue agentLevelOfCoverFromEnemy, ECoverValue enemyLevelOfCoverFromAgent)
    {
        #region Agent Cover Check
        switch (agentLevelOfCoverFromEnemy)
        {
            case ECoverValue.None:
                _actionValue -= ((int)Weighting.High / self.enemiesInSight.Count);
                break;
            case ECoverValue.Half:
                _actionValue += ((int)Weighting.High / self.enemiesInSight.Count);
                break;
            case ECoverValue.Full:
                _actionValue += ((int)Weighting.Low / self.enemiesInSight.Count);
                break;
            default:
                break;
        }
        #endregion

        #region Player Character Cover Check
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
        #endregion

        #region Chance To Hit Player Check
        int chanceToHit = self.enemiesInSight.Find((characterHitChancePair) => { return characterHitChancePair.Key == enemy; }).Value;
        if(chanceToHit > 70)
        {
            _actionValue -= (int)Weighting.High / self.enemiesInSight.Count;
        }
        else if(chanceToHit > 50)
        {
            _actionValue += (int)Weighting.Low / self.enemiesInSight.Count;
        }
        else
        {
            _actionValue += (int)Weighting.High / self.enemiesInSight.Count;
        }
        #endregion

        #region Number of Players In Sight Check
        _actionValue += self.enemiesInSight.Count > 2 ? ((int)Weighting.Low / self.enemiesInSight.Count) : -((int)Weighting.Medium / self.enemiesInSight.Count);
        #endregion
    }

    public override void ConsiderActionWithNoEnemyInSight(CharacterBase self)
    {
        #region Agent Generic Cover Check
        switch (self.occupiedTile.GetGenericCoverValue())
        {
            case ECoverValue.None:
                _actionValue += (int)Weighting.Medium;
                break;
            case ECoverValue.Half:
                _actionValue += (int)Weighting.High;
                break;
            case ECoverValue.Full:
                _actionValue += (int)Weighting.Low;
                break;
            default:
                break;
        }
        #endregion

        #region Agent Ally Proximity Check
        int nearbyAllies = PathfindingAgent.BreadthFirstAllySearch(self.occupiedTile);
        _actionValue += nearbyAllies > 2 ? -(int)Weighting.Medium : (int)Weighting.High;
        #endregion
    }

    public override void Enact(CharacterBase self)
    {
        self.EnterDefenseStance();
    }
}

public class ShootConsideration : SingleEnemyActionConsideration
{
    private CharacterBase enemyToAttack;

    public override void ConsiderAction(CharacterBase self, CharacterBase enemy, ECoverValue agentLevelOfCoverFromEnemy, ECoverValue enemyLevelOfCoverFromAgent)
    {
        int thisEnemyShootValue = 0;
        #region Agent Cover Check
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
        #endregion

        #region PlayerCharacter Cover Check
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
        #endregion

        #region PlayerCharacter Hit Chance Check
        int chanceToHit = self.enemiesInSight.Find((characterHitChancePair) => { return characterHitChancePair.Key == enemy; }).Value;
        if (chanceToHit > 70)
        {
            _actionValue += (int)Weighting.High;
        }
        else if (chanceToHit > 50)
        {
            _actionValue += (int)Weighting.Medium;
        }
        else
        {
            _actionValue -= (int)Weighting.Low;
        }
        #endregion

        #region Evaluate Whether This Shot Represents the Shooting Action
        if (thisEnemyShootValue > _actionValue)
        {
            enemyToAttack = enemy;
            _actionValue = thisEnemyShootValue;
        }
        #endregion
        //why is there no chanceToHitCalculation in here?
    }

    public override void ConsiderActionWithNoEnemyInSight(CharacterBase self)
    {
        _actionValue -= (int)Weighting.guarantee;
    }

    public override void Enact(CharacterBase self)
    {
        if (enemyToAttack != null)
            self.AttackCharacter(enemyToAttack);
        else
            (self as EnemyCharacter).moveDecision();
    }
}

public class ReloadConsideration : NoEnemyActionConsideration
{
    public override void ConsiderAction(CharacterBase self)
    {
        #region Remaining Ammunition Check
        if (self.ammunition > (self.MaximumAmmunition / 2))
        {
            _actionValue -= (int)Weighting.High;
        }
        else if (self.ammunition > (self.MaximumAmmunition / 4) && self.ammunition != 0)
        {
            _actionValue += (int)Weighting.High;
        }
        else if (self.ammunition == self.MaximumAmmunition)
        {
            _actionValue = -(int)Weighting.guarantee;
        }
        else
        {
            _actionValue += (int)Weighting.guarantee;
        }
        #endregion
        //adjustments based on pip value;
        #region Action Pip Check
        if (self.actionPips < self.MaximumActionPips)
        {
            _actionValue += (int)Weighting.Medium;
        }
        else
        {
            _actionValue -= (int)Weighting.Medium;
        }
        #endregion
    }

    public override void Enact(CharacterBase self)
    {
        self.Reload();
    }
}
#endregion

