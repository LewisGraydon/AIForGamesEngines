﻿using System.Collections;
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

    void Awake()
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
            pcScript.characterName = nameList[randomNameInt];
            spawnPointsList[rnd].occupier = pcScript;

            pcScript.occupiedTile = spawnPointsList[rnd];

            // Remove the spawn location and name from the list to ensure that each is unique.
            spawnPointsList.RemoveAt(rnd);
            nameList.RemoveAt(randomNameInt);

            player.transform.SetParent(container.transform, false);
        }
    }
}
