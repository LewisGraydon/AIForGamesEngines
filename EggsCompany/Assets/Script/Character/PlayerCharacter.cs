using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase
{
    // Names not guarenteed to be unique at the moment.
    private string[] nameArray = { "Benedict", "Royale", "Nog", "Beauregard", "Custard", "Meyerbeer", "Florentine", "Flæskeæggekage", "Foo Yung", "Mayo"};
    private string _characterName;

    public string characterName
    {
        get { return _characterName; }
    }

    // Start is called before the first frame update
    void Start()
    {
        onPlayerTeam = true;
        _characterName = nameArray[Random.Range(0, nameArray.Length - 1)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
