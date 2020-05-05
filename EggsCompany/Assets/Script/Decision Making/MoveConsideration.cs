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
    public int movementValue { get { return _movementValue; } }
    public int CompareTo(MovementConsideration other)
    {
        return (this is SingleEnemyMovementConsideration) && (other is TileOnlyMovementConsideration) ? -1 : (this is TileOnlyMovementConsideration) && (other is SingleEnemyMovementConsideration) ? 1 : 0;
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
            _movementValue -= (int)Weighting.High / self.enemiesInSight.Count;
        }
        else if(estimatedHitChance > currentHitChance)
        {
            _movementValue += (int)Weighting.High / self.enemiesInSight.Count;
        }
    }

    public override void ConsiderTileWithNoEnemy(ref CharacterBase self, Tile tileToConsider)
    {//could cheat and give if is in direction of players?
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
            _movementValue += (int)Weighting.High / self.enemiesInSight.Count;
        }
    }

    public override void ConsiderTileWithNoEnemy(ref CharacterBase self, Tile tileToConsider)
    {
        //again, could fudge a direction check here?
        return;
    }
}

public class SelfCoverConsideration : SingleEnemyMovementConsideration
{
    public override void ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider)
    {
        ECoverValue tileCoverFromEnemy = tileToConsider.ProvidesCoverInDirection(enemy.transform.position - self.transform.position);
        ECoverValue enemyCoverFromTile = enemy.occupiedTile.ProvidesCoverInDirection(self.transform.position - enemy.transform.position);
        #region Agent Cover Level Check

        #endregion
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
        #region PlayerCharacter Cover Level Check
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
        #endregion

    }

    public override void ConsiderTileWithNoEnemy(ref CharacterBase self, Tile tileToConsider)
    {
        if(tileToConsider.GetGenericCoverValue() > self.occupiedTile.GetGenericCoverValue())
        {
            _movementValue += (int)Weighting.High;
        }
        else
        {
            _movementValue -= (int)Weighting.High;
        }
    }
}

public class SelfVisibilityConsideration : SingleEnemyMovementConsideration
{
    public override void ConsiderTile(ref CharacterBase self, CharacterBase enemy, ref Tile tileToConsider)
    {
        bool isPredictonAccurate = Random.Range(0, 1) == 1;
        KeyValuePair<bool, int> visibleHitChancePair = self.FudgedSightHitchance(enemy.occupiedTile, tileToConsider);
        bool willEnemyBeVisisbleFromTile = isPredictonAccurate ? visibleHitChancePair.Key : Random.Range(0, 1) == 0;
        _movementValue += willEnemyBeVisisbleFromTile ? -((int)Weighting.Medium / self.enemiesInSight.Count) : ((int)Weighting.Medium / self.enemiesInSight.Count);
    }

    public override void ConsiderTileWithNoEnemy(ref CharacterBase self, Tile tileToConsider)
    {
        //again could be fudging something;
        return;
    }
}

public class ProximityToAllyConsideration : TileOnlyMovementConsideration
{
    public override void ConsiderTile(ref Tile tileToConsider)
    {
        _movementValue += -(int)Weighting.Medium * PathfindingAgent.BreadthFirstAllySearch((INodeSearchable)tileToConsider);
    }
}

public class TileOccupiedConsideration : TileOnlyMovementConsideration
{
    public override void ConsiderTile(ref Tile tileToConsider)
    {
        _movementValue += tileToConsider.occupier != null ? -(int)Weighting.guarantee : 0; 
    }
}

#endregion
