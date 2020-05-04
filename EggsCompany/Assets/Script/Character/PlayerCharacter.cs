using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{

    private List<PlayerCharacter> overwatchers;
    private string _characterName;
    public string characterName
    {
        get { return _characterName; }
        set 
        {
            _characterName = value;
        }
    }

    private void Start()
    {
        overwatchers = gsmScript.playerContainer.GetComponent<PlayerManager>().overwatchingPlayers;

    }

    public override void AttackCharacter(CharacterBase otherCharacter)
    {
        Debug.Log("A player is attacking!");
        base.AttackCharacter(otherCharacter);
    }

    public override void EnterOverwatchStance()
    {
        Debug.Log("Player is watching you...");
        overwatchers.Add(this);



        base.EnterOverwatchStance();

    }

}
