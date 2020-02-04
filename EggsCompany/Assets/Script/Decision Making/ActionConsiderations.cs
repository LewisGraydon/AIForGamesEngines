using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActionConsiderations
{
    public static List<Consideration> considerationList = new List<Consideration>() { new MoveConsideration(), new ReloadConsideration(), new ShootConsideration(), new OverwatchConsideration(), new DefendConsideration() };
}

//To add a consideration to the enemy agent's A.I :
// 1. create {verb of Action}Consideration class as a child of the Consideration class
// 2. add to the considerationList within the above ActionConsiderations class as the others have been.
#region Consideration classes
public abstract class Consideration
{
    public Consideration() { }
    virtual public float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        return -999.999f;
    }
}

public class MoveConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        return 0.0f;
    }
}

public class OverwatchConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        return 0.0f;
    }
}

public class DefendConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        return 0.0f;
    }
}

public class ShootConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        return 0.0f;
    }
}

public class ReloadConsideration : Consideration
{
    public override float ConsiderTile(CharacterBase self, Tile tileToConsider)
    {
        return 0.0f;
    }
}
#endregion
