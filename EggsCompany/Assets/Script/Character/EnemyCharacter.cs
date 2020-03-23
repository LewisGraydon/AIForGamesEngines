using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : CharacterBase
{
    public string _characterName;

    public string characterName
    {
        get { return _characterName; }
    }

    // Start is called before the first frame update
    void Start()
    {
        onPlayerTeam = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
