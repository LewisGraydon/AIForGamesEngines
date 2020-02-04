﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        new HitChanceConsideration(),
        new FlankingConsideration(),
        new SelfCoverConsideration(),
        new EnemyCoverConsideration(),
        new SelfVisibilityConsideration(),
        new EnemyVisibilityConsideration(),
        new ProximityToAllyConsideration()
    };
}

//To add a consideration to the enemy agent's A.I :
// 1. create {verb of Action}Consideration class as a child of the Consideration class
// 2. add to the relevant considerationList within the above ActionConsiderations class as the others have been.
public abstract class Consideration
{
    public Consideration()
    {

    }
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
        return tileValue / numberOfChecksWithinConsideration;
    }
}

public class OverwatchConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        // if distance to enemy is around 1/2 sight range then for this action

        // if the agent is in good cover from the enemies then + for this action

        // if the enemies are not in full cover then + for this action
        return 0.0f;
    }
}

public class DefendConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        // if distance is > 1/2 sight range then + 

        // if agent's level of cover is not full then +

        // if the enemys cover is full then +
        return 0.0f;
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
        return 0.0f;
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
        return 0.0f;
    }
}
#endregion

#region Move Consideration classes

public class HitChanceConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        return 0.0f;
    }
}

public class FlankingConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        return 0.0f;
    }
}

public class SelfCoverConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        return 0.0f;
    }
}

public class EnemyCoverConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        return 0.0f;
    }
}

public class SelfVisibilityConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        return 0.0f;
    }
}

public class EnemyVisibilityConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        return 0.0f;
    }
}

public class ProximityToAllyConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        base.ConsiderTile(self, tileToConsider);
        return 0.0f;
    }
}
#endregion
