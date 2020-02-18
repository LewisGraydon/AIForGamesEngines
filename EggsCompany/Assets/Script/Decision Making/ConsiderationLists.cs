using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ConsiderationLists
{
    public static List<Consideration> actionConsiderationList = new List<Consideration>()
    {
        new MoveConsideration(),
        new ReloadConsideration(),
        new ShootConsideration(),
        new OverwatchConsideration(),
        new DefendConsideration()
    };

    public static List<Consideration> moveConsiderationList = new List<Consideration>()
    {
        new HitChanceDifferenceConsideration(),
        new FlankingConsideration(),
        new SelfCoverConsideration(),
        new SelfVisibilityConsideration(),
        new ProximityToAllyConsideration()
    };
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

#region Action Consideration classes

public class MoveConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        tileValue += self.remainingPips > 1 ? 1.0f : 0.0f;
        float inCoverFromEnemiesValue = 0.0f;
        foreach (CharacterBase c in self.enemiesInSight)
        {
            inCoverFromEnemiesValue += self.isInCover(c) ? 0.0f : (1.0f / self.enemiesInSight.Count);
        }
        tileValue += inCoverFromEnemiesValue;
        tileValue += self.enemiesInSight.Count < 1 ? 1.0f : 0.0f;

        //if the enemy is < 1/2 range
        return tileValue;
    }
}

public class OverwatchConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        // if distance to enemy is around 1/2 sight range then for this action
        if (Vector3.Distance(self.transform.position, tileToConsider.transform.position) < (self.sightDistance / 2))
        {
            tileValue += (int)Weighting.Medium;
        }
        
        float AgentInCoverTotalValue = 0.0f;
        float enemiesInCoverTotalValue = 0.0f;
        foreach (CharacterBase enemy in self.enemiesInSight)
        {
            //TODO: calculate direction from enemy to self.
            // if the agent is in good cover from the enemies then + for this action
            switch (tileToConsider.ProvidesCoverInDirection(Vector3.forward)) //switch on amount of cover self is in from enemies;
            {
                case coverValue.None:
                    AgentInCoverTotalValue -= (int)Weighting.High / self.enemiesInSight.Count;
                    break;
                case coverValue.Half:
                    AgentInCoverTotalValue += (int)Weighting.Medium / self.enemiesInSight.Count;
                    break;
                case coverValue.Full:
                    AgentInCoverTotalValue += (int)Weighting.Low / self.enemiesInSight.Count;
                    break;
                default:
                    Debug.LogError("issue with cover Value returned");
                    break;
            }

            // if the enemies are not in full cover then + for this action as expect them to move
            switch (enemy.occupiedTile.ProvidesCoverInDirection(Vector3.back))
            {
                case coverValue.None:
                    enemiesInCoverTotalValue += (int)Weighting.High / self.enemiesInSight.Count;
                    break;
                case coverValue.Half:
                    enemiesInCoverTotalValue += (int)Weighting.Medium / self.enemiesInSight.Count;
                    break;
                case coverValue.Full:
                    enemiesInCoverTotalValue -= (int)Weighting.Low / self.enemiesInSight.Count;
                    break;
                default:
                    break;
            }
        }
        tileValue += AgentInCoverTotalValue;
        tileValue += enemiesInCoverTotalValue;


        return tileValue;
    }
}

public class DefendConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        // if distance is > 3/4 sight range then + 
        if(Vector3.Distance(self.transform.position, tileToConsider.transform.position) > (0.75 * self.sightDistance))
        {
            tileValue += (int)Weighting.Medium;
        }

        // if agent's level of cover is full then +
        // if the enemys cover is full then +
        float AgentInCoverTotalValue = 0.0f;
        float enemiesInCoverTotalValue = 0.0f;
        foreach (CharacterBase enemy in self.enemiesInSight)
        {
            //TODO: calculate direction from enemy to self.
            // if the agent is in good cover from the enemies then + for this action
            switch (tileToConsider.ProvidesCoverInDirection(Vector3.forward)) //switch on amount of cover self is in from enemies;
            {
                case coverValue.None:
                    AgentInCoverTotalValue -= (int)Weighting.High / self.enemiesInSight.Count;
                    break;
                case coverValue.Half:
                    AgentInCoverTotalValue -= (int)Weighting.Medium / self.enemiesInSight.Count;
                    break;
                case coverValue.Full:
                    AgentInCoverTotalValue += (int)Weighting.Low / self.enemiesInSight.Count;
                    break;
                default:
                    Debug.LogError("issue with cover Value returned");
                    break;
            }

            // if the enemies are not in full cover then + for this action as expect them to move
            switch (enemy.occupiedTile.ProvidesCoverInDirection(Vector3.back))
            {
                case coverValue.None:
                    enemiesInCoverTotalValue += (int)Weighting.High / self.enemiesInSight.Count;
                    break;
                case coverValue.Half:
                    enemiesInCoverTotalValue += (int)Weighting.Medium / self.enemiesInSight.Count;
                    break;
                case coverValue.Full:
                    enemiesInCoverTotalValue -= (int)Weighting.Low / self.enemiesInSight.Count;
                    break;
                default:
                    break;
            }
        }
        tileValue += AgentInCoverTotalValue;
        tileValue += enemiesInCoverTotalValue;

        return tileValue;
    }
}

public class ShootConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        /* if shot chance > 60% then +
         * 
         * if agent is behind cover then +
         * 
         * if pips remaining == 1 then +
         * 
         * if the enemy is not in full cover then +
         * 
         * 
         */
        if (self.occupiedTile.chanceToHit(tileToConsider) >= 60)
        {
            tileValue += (int)Weighting.High;
        }
        if (self.remainingPips == 1)
        {
            tileValue += (int)Weighting.Medium;
        }
        float AgentInCoverTotalValue = 0.0f;
        float enemiesInCoverTotalValue = 0.0f;
        foreach (CharacterBase enemy in self.enemiesInSight)
        {
            //TODO: calculate direction from enemy to self.
            // if the agent is in good cover from the enemies then + for this action
            switch (tileToConsider.ProvidesCoverInDirection(Vector3.forward)) //switch on amount of cover self is in from enemies;
            {
                case coverValue.None:
                    AgentInCoverTotalValue -= (int)Weighting.High / self.enemiesInSight.Count;
                    break;
                case coverValue.Half:
                    AgentInCoverTotalValue += (int)Weighting.Medium / self.enemiesInSight.Count;
                    break;
                case coverValue.Full:
                    AgentInCoverTotalValue += (int)Weighting.Low / self.enemiesInSight.Count;
                    break;
                default:
                    Debug.LogError("issue with cover Value returned");
                    break;
            }

            // if the enemies are not in full cover then + for this action as expect them to move
            switch (enemy.occupiedTile.ProvidesCoverInDirection(Vector3.back))
            {
                case coverValue.None:
                    enemiesInCoverTotalValue += (int)Weighting.High / self.enemiesInSight.Count;
                    break;
                case coverValue.Half:
                    enemiesInCoverTotalValue += (int)Weighting.Medium / self.enemiesInSight.Count;
                    break;
                case coverValue.Full:
                    enemiesInCoverTotalValue -= (int)Weighting.Low / self.enemiesInSight.Count;
                    break;
                default:
                    break;
            }
        }
        tileValue += AgentInCoverTotalValue;
        tileValue += enemiesInCoverTotalValue;

        return tileValue;
    }
}

public class ReloadConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        // if numshots == 0 then +
        // 
        // if numshots == 0 && pips remaining == 1 then guarantee
        //
        // tileValue = 1 - (numshots / maxShots)
        if (self.remainingPips == 1)
        {
            if (self.remainingShots > (self.maxShots / 2))
            {
                tileValue = 0;
            }
            else if (self.remainingShots > (self.maxShots / 4) && self.remainingShots != 0)
            {
                tileValue += (int)Weighting.High;
            }
            else
            {
                tileValue += (int)Weighting.guarantee;
            }
        }
        return tileValue;
    }
}
#endregion

#region Move Consideration classes

public class HitChanceDifferenceConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        /*
         * pipCost
         * foreach seen player;
         * random value for accuracy
         *
         * estimatedHitChance = isAccurate ? hitChanceFrom(tileToConsider) : randomNumber(1,100);
         * 
         * if estimatedHitChance > hitChanceFrom(self.currentTile) then + else - 
        */

        //TODO GET PIP COST FROM PATHFINDING
        bool accuracyOfPrediction = Random.Range(0, 1) == 1;
        float totalHitChanceDifference = 0.0f;
        foreach(CharacterBase enemy in self.enemiesInSight)
        {
            int estimatedHitChance = accuracyOfPrediction ? tileToConsider.chanceToHit(enemy.occupiedTile) : Random.Range(0, 100);
            if (estimatedHitChance > self.occupiedTile.chanceToHit(enemy.occupiedTile))
            {
                totalHitChanceDifference += ((int)Weighting.High / self.enemiesInSight.Count);
            }
            else if (estimatedHitChance == 100)
            {
                totalHitChanceDifference += ((int)Weighting.guarantee / self.enemiesInSight.Count);
            }
            else
            {
                totalHitChanceDifference -= ((int)Weighting.Medium / self.enemiesInSight.Count);
            }
        }
        tileValue += totalHitChanceDifference;

        return tileValue;
    }
}

public class FlankingConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider) //UNSURE IF  THIS IS NEEDED AS THE HITCHANCE SHOULD BE JUMPED BY THIS
    {
        base.ConsiderTile(self, tileToConsider);
        /*
         * 
         * pipCost
         * foreach seen player
         * 
         * if(tileToConsider.isFlanking(player)) then almost guarantee this choice
         * 
         * if(player.tile.isFlanking(tileToConsider) then remove some equity
         * 
         * 
        */

        //TODO GET PIP COST FROM PATHFINDING
        float playersFlankedTotalValue = 0.0f;
        foreach (CharacterBase enemy in self.enemiesInSight)
        {
            if(self.occupiedTile.transform.position.x == enemy.transform.position.x ||
                self.occupiedTile.transform.position.y == enemy.transform.position.y)
            {
                playersFlankedTotalValue += ((int)Weighting.High / self.enemiesInSight.Count);
            }
        }
        tileValue += playersFlankedTotalValue;
        return tileValue;
    }
}

public class SelfCoverConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        /*
         * pipCost
         * foreach player
         * 
         * is tileToConsider.providingCoverFrom(player);
         * 
        */
        float AgentInCoverTotalValue = 0.0f;
        float enemiesInCoverTotalValue = 0.0f;
        foreach (CharacterBase enemy in self.enemiesInSight)
        {
            //TODO: calculate direction from enemy to self.
            // if the agent is in good cover from the enemies then + for this action
            switch (tileToConsider.ProvidesCoverInDirection(Vector3.forward)) //switch on amount of cover self is in from enemies;
            {
                case coverValue.None:
                    AgentInCoverTotalValue -= (int)Weighting.High / self.enemiesInSight.Count;
                    break;
                case coverValue.Half:
                    AgentInCoverTotalValue += (int)Weighting.Medium / self.enemiesInSight.Count;
                    break;
                case coverValue.Full:
                    AgentInCoverTotalValue += (int)Weighting.Low / self.enemiesInSight.Count;
                    break;
                default:
                    Debug.LogError("issue with cover Value returned");
                    break;
            }

            // if the enemies are not in full cover then + for this action as expect them to move
            switch (enemy.occupiedTile.ProvidesCoverInDirection(Vector3.back))
            {
                case coverValue.None:
                    enemiesInCoverTotalValue += (int)Weighting.High / self.enemiesInSight.Count;
                    break;
                case coverValue.Half:
                    enemiesInCoverTotalValue += (int)Weighting.Medium / self.enemiesInSight.Count;
                    break;
                case coverValue.Full:
                    enemiesInCoverTotalValue -= (int)Weighting.Low / self.enemiesInSight.Count;
                    break;
                default:
                    break;
            }
        }
        return 0.0f;
    }
}

public class SelfVisibilityConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        /* foreach player
         * 
         * if tileToConsider.visibleFromTile
         *  if cowardly/random thing
         *      +
         * else
         *  -
        */
        float totalSightValue = 0.0f;
        bool predicitonAccuracy = Random.Range(0, 1) == 1;
        foreach (CharacterBase enemy in self.enemiesInSight)
        {
            bool willBeVisibleInNewTile = predicitonAccuracy ? tileToConsider.isVisibleFromTile(enemy.occupiedTile) : Random.Range(0, 1) == 0;
            totalSightValue += willBeVisibleInNewTile ? -((int)Weighting.Medium / self.enemiesInSight.Count) : ((int)Weighting.Medium / self.enemiesInSight.Count);
        }
        
        return totalSightValue;
    }
}

public class ProximityToAllyConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        /*
            * search within two squares for occupied by enemy of tileToConsider then (unless melee type enemy) -
            *  
            *  else 
            *  
            *  +
            * 
        */
        return -(int)Weighting.Medium * PathfindingAgent.BreadthFirstAllySearch((INodeSearchable)self.occupiedTile);

        //foreach (EDirection dir in Enum.GetValues(typeof(EDirection)))
        //{

        //}
        //foreach(Tile tile in self.occupiedTile)
    }
}
#endregion
