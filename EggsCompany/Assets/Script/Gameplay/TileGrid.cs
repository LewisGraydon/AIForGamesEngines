using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public int gridX;
    public int gridY;

    private List<Tile> _tilesList;

    // Start is called before the first frame update
    void Start()
    {
        _tilesList = new List<Tile>(gridX * gridY);

        var temparray = FindObjectsOfType<Tile>();

        foreach (var tile in temparray)
        {
            _tilesList.Add(tile);
        }

        //for(int i = 0; i < tilesList.Capacity; i++)
        //{
        //    Tile tempTile = this.gameObject.AddComponent(typeof(Tile)) as Tile;
        //    tilesList.Add(tempTile);
        //}

        for(int i = 0; i < _tilesList.Count; i++)
        {

            _tilesList[i].NeighborListFill();

            if (i - gridX >= 0) {
                _tilesList[i].AssignNeighbor(EDirection.North, _tilesList[i - gridX]);
            }
            else
            {
                _tilesList[i].AssignNeighbor(EDirection.North, null);
            }

            if(i + 1 < _tilesList.Count)
            {
                _tilesList[i].AssignNeighbor(EDirection.East, _tilesList[i + 1]);
            }
            else
            {
                _tilesList[i].AssignNeighbor(EDirection.East, null);
            }

            if(i + gridX < _tilesList.Count)
            {
                _tilesList[i].AssignNeighbor(EDirection.South, _tilesList[i + gridX]);
            }
            else
            {
                _tilesList[i].AssignNeighbor(EDirection.South, null);

            }

            if (i - 1 >= 0)
            {
                _tilesList[i].AssignNeighbor(EDirection.West, _tilesList[i - 1]);
            }
            else
            {
                _tilesList[i].AssignNeighbor(EDirection.West, null);
            }
        }

        foreach (var tile in _tilesList)
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
