using System;
using UnityEngine;
using Random = UnityEngine.Random;

#region Movement Consideration Abstract Parent Classes
public abstract class MovementConsideration: IComparable<MovementConsideration>
{
    protected int _movementValue;
    public int movementValue { get { return _movementValue; } }
    public int CompareTo(MovementConsideration other)
    {
        return (this is SingleEnemyMovementConsideration) && (other is TileOnlyMovementConsideration) ? -1 : (this is TileOnlyMovementConsideration) && (other is SingleEnemyMovementConsideration) ? 1 : 0;
    }
}

public abstract class SingleEnemyMovementConsideration : MovementConsideration
{
    public abstract void ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider);
}

public abstract class TileOnlyMovementConsideration : MovementConsideration
{
    public abstract void ConsiderTile(ref Tile tileToConsider);
}
#endregion

#region MovementConsideration classes
public class HitChanceDifferenceConsideration : SingleEnemyMovementConsideration
{
    public override void ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider)
    {

        bool isPredictonAccurate = Random.Range(0, 1) == 1;
        int estimatedHitChance = isPredictonAccurate ? tileToConsider.chanceToHit(enemy.occupiedTile) : Random.Range(0, 100);

        _movementValue += estimatedHitChance == 100 ? (int)Weighting.guarantee : 
                                                    estimatedHitChance > self.occupiedTile.chanceToHit(enemy.occupiedTile) ? (int)Weighting.High :
                                                    -(int)Weighting.High;
    }
}

public class FlankingConsideration : SingleEnemyMovementConsideration
{
    public override void ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider)
    {
        _movementValue += self.occupiedTile.transform.position.x == enemy.transform.position.x || self.occupiedTile.transform.position.y == enemy.transform.position.y ?
               (int)Weighting.guarantee : 0;
    }
}

public class SelfCoverConsideration : SingleEnemyMovementConsideration
{
    public override void ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider)
    {
        ECoverValue tileCoverFromEnemy = tileToConsider.ProvidesCoverInDirection(enemy.transform.position - self.transform.position);
        ECoverValue enemyCoverFromTile = enemy.occupiedTile.ProvidesCoverInDirection(self.transform.position - enemy.transform.position);

        switch (tileCoverFromEnemy)
        {
            case ECoverValue.None:
                _movementValue -= (int)Weighting.High / self.enemiesInSight.Count;
                break;
            case ECoverValue.Half:
                _movementValue += (int)Weighting.Medium / self.enemiesInSight.Count;
                break;
            case ECoverValue.Full:
                _movementValue += (int)Weighting.Low / self.enemiesInSight.Count;
                break;
            default:
                Debug.LogError("issue with cover Value returned");
                break;
        }

        switch (enemyCoverFromTile)
        {
            case ECoverValue.None:
                _movementValue += (int)Weighting.High / self.enemiesInSight.Count;
                break;
            case ECoverValue.Half:
                _movementValue += (int)Weighting.Medium / self.enemiesInSight.Count;
                break;
            case ECoverValue.Full:
                _movementValue -= (int)Weighting.Low / self.enemiesInSight.Count;
                break;
            default:
                break;
        }

    }
}

public class SelfVisibilityConsideration : SingleEnemyMovementConsideration
{
    public override void ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider)
    {
        bool isPredictonAccurate = Random.Range(0, 1) == 1;
        bool willEnemyBeVisisbleFromTile = isPredictonAccurate ? tileToConsider.isVisibleFromTile(enemy.occupiedTile) : Random.Range(0, 1) == 0;
        _movementValue += willEnemyBeVisisbleFromTile ? -((int)Weighting.Medium / self.enemiesInSight.Count) : ((int)Weighting.Medium / self.enemiesInSight.Count);
    }
}

public class ProximityToAllyConsideration : TileOnlyMovementConsideration
{
    public override void ConsiderTile(ref Tile tileToConsider)
    {
        _movementValue += -(int)Weighting.Medium * PathfindingAgent.BreadthFirstAllySearch((INodeSearchable)tileToConsider);
    }
}

#endregion
