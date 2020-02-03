using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public int gridX;
    public int gridY;

    private List<Tile> tilesList;

    // Start is called before the first frame update
    void Start()
    {
        tilesList = new List<Tile>(gridX * gridY);

        for(int i = 0; i < tilesList.Capacity; i++)
        {
            Tile tempTile = this.gameObject.AddComponent(typeof(Tile)) as Tile;
            tilesList.Add(tempTile);
        }

        for(int i = 0; i < tilesList.Count; i++)
        {
            
            if (i - gridX >= 0) {
                tilesList[i].AssignNeighbor(EDirection.North, tilesList[i - gridX]);
            }
            else
            {
                tilesList[i].AssignNeighbor(EDirection.North, null);
            }

            if(i + 1 < tilesList.Count)
            {
                tilesList[i].AssignNeighbor(EDirection.East, tilesList[i + 1]);
            }
            else
            {
                tilesList[i].AssignNeighbor(EDirection.East, null);
            }

            if(i + gridX < tilesList.Count)
            {
                tilesList[i].AssignNeighbor(EDirection.South, tilesList[i + gridX]);
            }
            else
            {
                tilesList[i].AssignNeighbor(EDirection.South, null);

            }

            if (i - 1 >= 0)
            {
                tilesList[i].AssignNeighbor(EDirection.West, tilesList[i - 1]);
            }
            else
            {
                tilesList[i].AssignNeighbor(EDirection.West, null);
            }
        }

        foreach (var tile in tilesList)
        {
            Debug.Log(tile);
        }
        Debug.Log("testing");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
