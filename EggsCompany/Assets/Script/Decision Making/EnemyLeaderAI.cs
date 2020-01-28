using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLeaderAI : MonoBehaviour
{
    public enum CommandedAIState
    {
        reload = 0, // move to best form of cover (either cover or one pips movement away) and then reload
        fortify = 1, // move to best form of cover and Overwatch or Dodge/defend/whatever the fuck we're calling it.
        advance = 2, // move to best form of cover (limit of shooting distance if no actual cover in the way and cannot get right next to player) then shoot (possible the best location is where it already is).
        flank = 3, // *Given to one or two enemies where the other would be told to fortify* move around players position, then move again if necessary or if >70% shot chance shoot.
        retreat = 4, // move away from players in decided direction and if in cover overwatch or if necessary use both movement. (possibly if it thinks one move takes it out of sight even with no cover overwatch)
    };

    struct TestTile { };
    struct TestCharacter { };

    private TestCharacter[] testCharacters = new TestCharacter[1];
    
    


    void makeDecision()
    {
        if(CanSeePlayer())
        {

        }
        else
        {

        }
    }






    TestTile[] GetPotentialMoveTiles()
    {
        return new TestTile[1];
    }


    bool CanSeePlayer()
    {
        //should loop through characters to under command and see if they have sight of player things still;
        return false;
    }

    bool isInCover()
    {
        return false;
    }
}
