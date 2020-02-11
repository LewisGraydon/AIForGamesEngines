using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A Utility class that gameobjects may use to call and obtain pathfinding information.
//If we expand to two tilegrids in scene (verticality) we will need to restructure this class.
public class PathfindingAgent : MonoBehaviour
{

    private TileGrid _tileGrid;

    // Start is called before the first frame update
    void Start()
    {

        _tileGrid = FindObjectOfType<TileGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //A basic implementation of a breadth first search algorithm.
    //Wip
    void BreadthFirstBasic(Tile startTile, Tile targetTile)
    {
        Queue<Tile> tileQueue = new Queue<Tile>();
        tileQueue.Enqueue(startTile);

        Tile currentTile = new Tile();

        while(tileQueue.Count > 0)
        {
            currentTile = tileQueue.Dequeue();

            if (currentTile == targetTile)
            {
                //Return Found Tile
            }
            else
            {
                //Marked tile as searched
                //
            }
            


        }

    }

    //@Desc: A function that finds all tiles a unit can move to with the next move action.
    //@Param - moveRange : The maximum tile distance the given unit can move with a single movement pip.
    //@Return: A list of Tiles that the unit can move to.
    //Notes: Might be able to change param to egg/unit object for ease of use. IE object could pass self.
    public List<Tile> FindTileRange(int moveRange)
    {







        //This is just to stop the compiler from throwing an error while I work on the prototype
        List<Tile> templist;
        templist = new List<Tile>();
        return templist;
    }




}
