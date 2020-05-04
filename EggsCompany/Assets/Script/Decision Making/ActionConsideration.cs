using System;
using System.Collections.Generic;
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
        _actionValue += self.actionPips > 1 ? (int)Weighting.High : 0;

        foreach (KeyValuePair<CharacterBase, int> c in self.enemiesInSight)
        {
            //inCoverFromEnemiesValue += (Weighting)self.occupiedTile.ProvidesCoverInDirection(self.transform.position - tileToConsider.transform.position);
            //_actionValue += self.isInCover(c.Key) ? 0 : (1 / self.enemiesInSight.Count);
            Debug.Log("Chris has forgetting to fix the moveConsideration which is still using isInCover, I mean for fuck sake it is not even named correctly.");
        }
        //if the enemy is < 1/2 range
        Debug.Log("Chris has forgotten to add the range calculation to the MoveConsideration.");
        _actionValue += self.enemiesInSight.Count < 2 ? (int)Weighting.Medium : -(int)Weighting.Low;

    }

    public override void ConsiderActionWithNoEnemyInSight(CharacterBase self)
    {
        _actionValue += self.actionPips > 1 ? (int)Weighting.High : 0;
        _actionValue += (int)Weighting.guarantee;
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

    public override void ConsiderActionWithNoEnemyInSight(CharacterBase self)
    {
        for(int i = 0; i < (int)EWallDirection.Error; i++)
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

    public override void ConsiderActionWithNoEnemyInSight(CharacterBase self)
    {
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
        //adjustments based on pip value;
        if(self.actionPips < self.MaximumActionPips)
        {
            _actionValue += (int)Weighting.Medium;
        }
        else
        {
            _actionValue -= (int)Weighting.Medium;
        }
    }

    public override void Enact(CharacterBase self)
    {
        self.Reload();
    }
}
#endregion

