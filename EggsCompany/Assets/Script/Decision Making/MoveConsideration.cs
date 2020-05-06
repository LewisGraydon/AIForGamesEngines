using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

#region Movement Consideration Abstract Parent Classes
public abstract class MovementConsideration: IComparable<MovementConsideration>
{
    protected int _movementValue;
    public int movementValue
    {
        get => _movementValue;  
        set
        {
            _movementValue = value;
            numberOfChecksWithinConsideration++;
            _finalValue = ((float)value / (float)(numberOfChecksWithinConsideration * (int)Weighting.SuperHeavy));
        }
    }
    private int numberOfChecksWithinConsideration;
    private float _finalValue;
    public float FinalValue
    {
        get
        {
            numberOfChecksWithinConsideration = 0;
            _movementValue = 0;
            return _finalValue;
        }
    }
    public int CompareTo(MovementConsideration other)
    {
        return (this is SingleEnemyMovementConsideration) && (other is TileOnlyMovementConsideration || other is TileSelfMovementConsideration) ? -1 : (this is TileOnlyMovementConsideration || this is TileSelfMovementConsideration) && (other is SingleEnemyMovementConsideration) ? 1 : 0;
    }

    public virtual void IncreaseMovementValuePerWallCoverLevel(Tile tile, int noCoverModifier, int halfCoverModifier, int fullCoverModifier)
    {
        foreach (WallType wall in tile.walls)
        {
            switch ((ECoverValue)wall.coverValue)
            {
                case ECoverValue.None:
                    movementValue += noCoverModifier;
                    break;
                case ECoverValue.Half:
                    movementValue += halfCoverModifier;
                    break;
                case ECoverValue.Full:
                    movementValue += fullCoverModifier;
                    break;
                default:
                    Debug.LogError("Error Cover value of wall in tile: " + tile);
                    break;
            }
        }
    }


}

public abstract class SingleEnemyMovementConsideration : MovementConsideration
{
    public abstract void ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider);

    public abstract void ConsiderTileWithNoEnemy(ref CharacterBase self, Tile tileToConsider);
}

public abstract class TileOnlyMovementConsideration : MovementConsideration
{
    public abstract void ConsiderTile(ref Tile tileToConsider);
}
public abstract class TileSelfMovementConsideration : MovementConsideration
{
    public abstract void ConsiderTile(CharacterBase self, Tile tileToConsider);
}

#endregion

#region MovementConsideration classes
public class HitChanceDifferenceConsideration : SingleEnemyMovementConsideration
{
    public override void ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider)
    {

        bool isPredictonAccurate = Random.Range(0, 1) == 1;
        int currentHitChance = self.enemiesInSight.Find((pair) => pair.Key == enemy).Value;
        KeyValuePair<bool, int> seenAndHitChanceFromTile = self.FudgedSightHitchance(tileToConsider, enemy.occupiedTile);
        int estimatedHitChance = isPredictonAccurate ? seenAndHitChanceFromTile.Value : Random.Range(0, 100);
        if(seenAndHitChanceFromTile.Key == false || estimatedHitChance < currentHitChance)
        {
            movementValue -= (int)Weighting.SuperHeavy / self.enemiesInSight.Count;
        }
        else if(estimatedHitChance > currentHitChance)
        {
            movementValue += (int)Weighting.SuperHeavy / self.enemiesInSight.Count;
        }
    }

    public override void ConsiderTileWithNoEnemy(ref CharacterBase self, Tile tileToConsider)
    {//could cheat and give if is in direction of players?
        //movementValue -= (int)Weighting.SuperHeavy;
        return;
    }
}

public class FlankingConsideration : SingleEnemyMovementConsideration
{
    public override void ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider)
    {
        if (Mathf.Approximately(tileToConsider.transform.position.x, enemy.transform.position.x) || Mathf.Approximately(tileToConsider.transform.position.y, enemy.transform.position.y)
            && !(Mathf.Approximately(self.transform.position.x, enemy.transform.position.x) || Mathf.Approximately(self.transform.position.y, enemy.transform.position.y)))
        {
            movementValue += (int)Weighting.SuperHeavy / self.enemiesInSight.Count;
        }
    }

    public override void ConsiderTileWithNoEnemy(ref CharacterBase self, Tile tileToConsider)
    {
        //again, could fudge a direction check here?
        //movementValue -= (int)Weighting.SuperHeavy;
        return;
    }
}

public class SelfCoverConsideration : SingleEnemyMovementConsideration
{
    public override void ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider)
    {
        ECoverValue tileCoverFromPlayer = tileToConsider.ProvidesCoverInDirection(enemy.transform.position - self.transform.position);
        ECoverValue AgentCoverOnTile = enemy.occupiedTile.ProvidesCoverInDirection(self.transform.position - enemy.transform.position);
        #region Agent Cover Level Check

        #endregion
        switch (tileCoverFromPlayer)
        {
            case ECoverValue.None:
                movementValue += -(int)Weighting.SuperHeavy / self.enemiesInSight.Count;
                break;
            case ECoverValue.Half:
                movementValue += (int)Weighting.Heavy / self.enemiesInSight.Count;
                break;
            case ECoverValue.Full:
                movementValue += (int)Weighting.Light / self.enemiesInSight.Count;
                break;
            default:
                Debug.LogError("issue with cover Value returned");
                break;
        }
        #region PlayerCharacter Cover Level Check
        switch (AgentCoverOnTile)
        {
            case ECoverValue.None:
                movementValue += -(int)Weighting.SuperHeavy / self.enemiesInSight.Count;
                break;
            case ECoverValue.Half:
                movementValue += (int)Weighting.Light / self.enemiesInSight.Count;
                break;
            case ECoverValue.Full:
                movementValue += (int)Weighting.Heavy/ self.enemiesInSight.Count;
                break;
            default:
                break;
        }
        #endregion

    }

    public override void ConsiderTileWithNoEnemy(ref CharacterBase self, Tile tileToConsider)
    {
        IncreaseMovementValuePerWallCoverLevel(tileToConsider, -(int)Weighting.Light, (int)Weighting.Light, (int)Weighting.Heavy);
    }
}

public class SelfVisibilityConsideration : SingleEnemyMovementConsideration
{
    public override void ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider)
    {
        bool isPredictonAccurate = Random.Range(0, 1) == 1;
        KeyValuePair<bool, int> visibleHitChancePair = self.FudgedSightHitchance(enemy.occupiedTile, tileToConsider);
        bool willEnemyBeVisisbleFromTile = isPredictonAccurate ? visibleHitChancePair.Key : Random.Range(0, 1) == 0;
        movementValue += willEnemyBeVisisbleFromTile ? -((int)Weighting.Heavy / self.enemiesInSight.Count) : ((int)Weighting.Heavy / self.enemiesInSight.Count);
    }

    public override void ConsiderTileWithNoEnemy(ref CharacterBase self, Tile tileToConsider)
    {
        //again could be fudging something;
        //movementValue += -(int)Weighting.SuperHeavy;
        return;
    }
}

public class ProximityToAllyConsideration : TileOnlyMovementConsideration
{
    public override void ConsiderTile(ref Tile tileToConsider)
    {
        movementValue += -(int)Weighting.Light * PathfindingAgent.BreadthFirstAllySearch((INodeSearchable)tileToConsider);
    }
}

public class TileOccupiedConsideration : TileOnlyMovementConsideration
{
    public override void ConsiderTile(ref Tile tileToConsider)
    {
        movementValue += tileToConsider.occupier != null ? -(int)Weighting.SuperHeavy : 0; 
    }
}

public class TileDirectionOfEnemiesConsideration : TileSelfMovementConsideration
{
    public override void ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        Vector3 averagePoisition = Vector3.zero;
        foreach (PlayerCharacter playerCharacter in GameObject.FindObjectsOfType<PlayerCharacter>())
        {
            averagePoisition += playerCharacter.transform.position;
        }
        averagePoisition *= 0.5f;
        Vector3 directionToTileToConsiderFromAgent = (tileToConsider.transform.position - self.occupiedTile.transform.position).normalized;
        Vector3 directionToEnemiesFromTileToConsider = (averagePoisition - tileToConsider.transform.position).normalized;
        Vector3 directionToEnemiesFromAgent = (averagePoisition - self.occupiedTile.transform.position).normalized;

        if(((directionToEnemiesFromAgent.x >= 0 && directionToEnemiesFromTileToConsider.x >= 0) || (directionToEnemiesFromAgent.x >= 0 && directionToEnemiesFromTileToConsider.x >= 0))
            || ((directionToEnemiesFromAgent.x < 0 && directionToEnemiesFromTileToConsider.x < 0) || (directionToEnemiesFromAgent.x < 0 && directionToEnemiesFromTileToConsider.x < 0)))
        {
            movementValue += (int)Weighting.Heavy;
        }
        else
        {
            movementValue += -(int)Weighting.Heavy;
        }
    }
}


#endregion
