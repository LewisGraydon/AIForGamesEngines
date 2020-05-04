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
}
