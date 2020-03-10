using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class PlayerSpawn : MonoBehaviour
{
    public Tile[] potentialSpawnPoints;
    public int numberOfOperators;
    public GameObject playerCharacterPrefab;
    public GameObject container;

    private List<string> nameList;
    private List<Tile> spawnPointsList = new List<Tile>();
    
    // Start is called before the first frame update
    void Start()
    {
        nameList = new List<string> { "Benedict", "Royale", "Nog", "Beauregard", "Custard", "Meyerbeer", "Florentine", "Flæskeæggekage", "Foo Yung", "Mayo" };
        spawnPointsList = potentialSpawnPoints.ToList();

        for (int i = 0; i < numberOfOperators; i++)
        {        
            int rnd = Random.Range(0, spawnPointsList.Count);        
            int randomNameInt = Random.Range(0, nameList.Count);

            Vector3 spawnLocation = spawnPointsList[rnd].transform.position;
            spawnLocation.y = 0.5f;
       
            GameObject player = Instantiate(playerCharacterPrefab, spawnLocation, Quaternion.identity);
            PlayerCharacter pcScript = player.GetComponent<PlayerCharacter>();          

            player.name = nameList[randomNameInt];
            pcScript._characterName = nameList[randomNameInt];
            spawnPointsList[rnd].occupier = ETileOccupier.PlayerCharacter;           

            // Remove the spawn location and name from the list to ensure that each is unique.
            spawnPointsList.RemoveAt(rnd);
            nameList.RemoveAt(randomNameInt);

            player.transform.SetParent(container.transform, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
