using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    private List<Tile> _tilesList;

    public List<Tile> GetGridTileList()
    {
        return _tilesList;
    }

    // Start is called before the first frame update
    void Start()
    {
        _tilesList = new List<Tile>();

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
            _tilesList[i].CopyNeighborsToChildren();
        }

        foreach (var tile in _tilesList)
        {
            foreach (INodeSearchable child in tile.children)
            {
                Debug.Log(child);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
