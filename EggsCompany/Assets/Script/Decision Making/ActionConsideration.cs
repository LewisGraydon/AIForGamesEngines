using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region parent classes

public abstract class ActionConsideration : IComparable<ActionConsideration>
{
    private int numberOfChecksWithinConsideration = 0;
    protected int _actionValue;
    public int actionValue
    {
        get { return _actionValue; }
        set
        {
            _actionValue = value;
            numberOfChecksWithinConsideration++;
            _finalValue = ((float)value/ ((float)numberOfChecksWithinConsideration * (float)Weighting.SuperHeavy)); 
        }
    }
    protected float _finalValue;
    public float FinalValue { 
        get
        {
            numberOfChecksWithinConsideration = 0;
            return _finalValue; 
        }
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

    public virtual void IncreaseActionValuePerWallCoverLevel(Tile tile, int noCoverModifier, int halfCoverModifier, int fullCoverModifier)
    {
        foreach(WallType wall in tile.walls)
        {
            switch ((ECoverValue)wall.coverValue)
            {
                case ECoverValue.None:
                    actionValue += noCoverModifier;
                    break;
                case ECoverValue.Half:
                    actionValue += halfCoverModifier;
                    break;
                case ECoverValue.Full:
                    actionValue += fullCoverModifier;
                    break;
                default:
                    Debug.LogError("Error Cover value of wall in tile: " + tile);
                    break;
            }
        }
    }
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
        actionValue += self.actionPips > 1 ? ((int)Weighting.SuperHeavy / self.enemiesInSight.Count) : 0;
        #endregion

        #region Proximity To Players Check
        float distanceBetween = Vector3.Distance(self.transform.position, enemy.transform.position);
        if(distanceBetween <= self.closeDistanceCap)
        {
            actionValue -= ((int)Weighting.Heavy / self.enemiesInSight.Count);
        }
        else if (distanceBetween <= self.middleDistanceCap)
        {
            actionValue += ((int)Weighting.Light / self.enemiesInSight.Count);
        }
        else
        {
            actionValue += ((int)Weighting.Heavy / self.enemiesInSight.Count);
        }
        #endregion

        #region Enemy Overwatch Check
        if(enemy.isOverwatching)
        {
            actionValue -= ((int)Weighting.Heavy / self.enemiesInSight.Count);
        }
        #endregion

        #region Agent Cover Check
        switch (agentLevelOfCoverFromEnemy)
        {
            case ECoverValue.None:
                actionValue += ((int)Weighting.SuperHeavy / self.enemiesInSight.Count);
                break;
            case ECoverValue.Half:
                actionValue += ((int)Weighting.Heavy / self.enemiesInSight.Count);
                break;
            case ECoverValue.Full:
                actionValue -= ((int)Weighting.Light / self.enemiesInSight.Count);
                break;
            default:
                break;
        }
        #endregion
        //actionValue += self.enemiesInSight.Count < 2 ? (int)Weighting.Medium : -(int)Weighting.Low;
    }

    public override void ConsiderActionWithNoEnemyInSight(CharacterBase self)
    {
        actionValue += (int)Weighting.Heavy;
        #region Remaining Pips Check
        actionValue += self.actionPips > 1 ? (int)Weighting.SuperHeavy : 0;
        #endregion

        #region Agent Generic Cover Check
        IncreaseActionValuePerWallCoverLevel(self.occupiedTile, (int)Weighting.SuperHeavy, -(int)Weighting.Light, -(int)Weighting.Heavy);
        #endregion

        #region Agent Ally Proximity Check
        int nearbyAllies = PathfindingAgent.BreadthFirstAllySearch(self.occupiedTile);
        actionValue += nearbyAllies > 2 ? -(int)Weighting.Heavy : (int)Weighting.SuperHeavy;
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
                actionValue -= ((int)Weighting.SuperHeavy / self.enemiesInSight.Count); 
                break;
            case ECoverValue.Half:
                actionValue += ((int)Weighting.Light / self.enemiesInSight.Count);
                break;
            case ECoverValue.Full:
                actionValue += ((int)Weighting.SuperHeavy / self.enemiesInSight.Count);
                break;
            default:
                break;
        }
        #endregion

        #region PlayerCharacter Cover Check
        switch (enemyLevelOfCoverFromAgent)
        {
            case ECoverValue.None:
                actionValue += ((int)Weighting.SuperHeavy / self.enemiesInSight.Count);
                break;
            case ECoverValue.Half:
                actionValue += ((int)Weighting.Light / self.enemiesInSight.Count);
                break;
            case ECoverValue.Full:
                actionValue -= ((int)Weighting.Heavy / self.enemiesInSight.Count);
                break;
            default:
                break;
        }
        #endregion

        #region Ammo Check
        actionValue += self.ammunition >= (self.MaximumAmmunition >> 1) ? ((int)Weighting.Heavy / self.enemiesInSight.Count) : -((int)Weighting.Heavy / self.enemiesInSight.Count);
        #endregion

        #region Proximity To Players Check
        float distanceBetween = Vector3.Distance(self.transform.position, enemy.transform.position);
        if (distanceBetween <= self.closeDistanceCap)
        {
            actionValue -= ((int)Weighting.Heavy / self.enemiesInSight.Count);
        }
        else if (distanceBetween <= self.middleDistanceCap)
        {
            actionValue += ((int)Weighting.Heavy / self.enemiesInSight.Count);
        }
        else
        {
            actionValue += ((int)Weighting.Light / self.enemiesInSight.Count);
        }
        #endregion
    }

    public override void ConsiderActionWithNoEnemyInSight(CharacterBase self)
    {
        #region Agent Generic Cover Check
        IncreaseActionValuePerWallCoverLevel(self.occupiedTile, -(int)Weighting.SuperHeavy, -(int)Weighting.Light, (int)Weighting.Heavy);
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
                actionValue -= ((int)Weighting.SuperHeavy / self.enemiesInSight.Count);
                break;
            case ECoverValue.Half:
                actionValue += ((int)Weighting.SuperHeavy / self.enemiesInSight.Count);
                break;
            case ECoverValue.Full:
                actionValue += ((int)Weighting.Light / self.enemiesInSight.Count);
                break;
            default:
                break;
        }
        #endregion

        #region Player Character Cover Check
        switch (enemyLevelOfCoverFromAgent)
        {
            case ECoverValue.None:
                actionValue += (int)Weighting.SuperHeavy / self.enemiesInSight.Count;
                break;
            case ECoverValue.Half:
                actionValue += (int)Weighting.Heavy / self.enemiesInSight.Count;
                break;
            case ECoverValue.Full:
                actionValue -= (int)Weighting.Light / self.enemiesInSight.Count;
                break;
            default:
                break;
        }
        #endregion

        #region Chance To Hit Player Check
        int chanceToHit = self.enemiesInSight.Find((characterHitChancePair) => { return characterHitChancePair.Key == enemy; }).Value;
        if(chanceToHit > 70)
        {
            actionValue -= (int)Weighting.SuperHeavy / self.enemiesInSight.Count;
        }
        else if(chanceToHit > 50)
        {
            actionValue += (int)Weighting.Light / self.enemiesInSight.Count;
        }
        else
        {
            actionValue += (int)Weighting.SuperHeavy / self.enemiesInSight.Count;
        }
        #endregion

        #region Number of Players In Sight Check
        actionValue += self.enemiesInSight.Count > 2 ? ((int)Weighting.Light / self.enemiesInSight.Count) : -((int)Weighting.Heavy / self.enemiesInSight.Count);
        #endregion
    }

    public override void ConsiderActionWithNoEnemyInSight(CharacterBase self)
    {
        #region Agent Generic Cover Check
        IncreaseActionValuePerWallCoverLevel(self.occupiedTile, (int)Weighting.Light, (int)Weighting.Heavy, (int)Weighting.Feather);
        #endregion

        #region Agent Ally Proximity Check
        int nearbyAllies = PathfindingAgent.BreadthFirstAllySearch(self.occupiedTile);
        actionValue += nearbyAllies > 2 ? -(int)Weighting.Heavy : (int)Weighting.SuperHeavy;
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
                thisEnemyShootValue -= (int)Weighting.Heavy;
                break;
            case ECoverValue.Half:
                thisEnemyShootValue += (int)Weighting.Heavy;
                break;
            case ECoverValue.Full:
                thisEnemyShootValue += (int)Weighting.SuperHeavy;
                break;
            default:
                Debug.LogError("issue with cover Value returned");
                break;
        }
        #endregion

        #region PlayerCharacter Hit Chance Check
        int chanceToHit = self.enemiesInSight.Find((characterHitChancePair) => { return characterHitChancePair.Key == enemy; }).Value;
        if (chanceToHit > 70)
        {
            actionValue += (int)Weighting.SuperHeavy;
        }
        else if (chanceToHit > 50)
        {
            actionValue += (int)Weighting.Heavy;
        }
        else
        {
            actionValue -= (int)Weighting.Light;
        }
        #endregion

        #region Evaluate Whether This Shot Represents the Shooting Action
        if (thisEnemyShootValue > actionValue || enemyToAttack == null)
        {
            enemyToAttack = enemy;
            actionValue = thisEnemyShootValue;
        }
        #endregion
        //why is there no chanceToHitCalculation in here?
    }

    public override void ConsiderActionWithNoEnemyInSight(CharacterBase self)
    {
        actionValue -= (int)Weighting.SuperHeavy;
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
            actionValue -= (int)Weighting.SuperHeavy;
        }
        else if (self.ammunition > (self.MaximumAmmunition / 4) && self.ammunition != 0)
        {
            actionValue += (int)Weighting.SuperHeavy;
        }
        else if (self.ammunition == self.MaximumAmmunition)
        {
            actionValue = -(int)Weighting.SuperHeavy;
        }
        else
        {
            actionValue += (int)Weighting.SuperHeavy;
        }
        #endregion
        //adjustments based on pip value;
        #region Action Pip Check
        if (self.actionPips < self.MaximumActionPips)
        {
            actionValue += (int)Weighting.Heavy;
        }
        else
        {
            actionValue -= (int)Weighting.Heavy;
        }
        #endregion
    }

    public override void Enact(CharacterBase self)
    {
        self.Reload();
    }
}
#endregion

