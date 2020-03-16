using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public Tile[] spawnPoints;
    public GameObject container;
    public GameObject enemyCharacterPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < spawnPoints.Length; i++)
        {
            Vector3 spawnLocation = spawnPoints[i].transform.position;
            spawnLocation.y = 1.0f;

            GameObject enemy = Instantiate(enemyCharacterPrefab, spawnLocation, Quaternion.identity);
            spawnPoints[i].occupier = ETileOccupier.EnemyCharacter;

            enemy.name = "Beehive " + i;
            enemy.transform.SetParent(container.transform, false);
        }
    }
}
