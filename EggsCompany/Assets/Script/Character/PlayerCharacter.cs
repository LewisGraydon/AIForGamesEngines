using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{

    private string _characterName;
    public string characterName
    {
        get { return _characterName; }
        set 
        {
            _characterName = value;
        }
    }
    

    public override void AttackCharacter(CharacterBase otherCharacter)
    {
        Debug.Log("A player is attacking!");
        base.AttackCharacter(otherCharacter);
    }

    public override void EnterOverwatchStance()
    {
        Debug.Log(this + " is on eggde... (overwatch)");

        gsmScript.playerContainer.GetComponent<PlayerManager>().overwatchingPlayers.Add(this);

        base.EnterOverwatchStance();

    }

}
