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
        //Set/Find Agent's environement
        _tileGrid = FindObjectOfType<TileGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //A generic implementation of a breadth first search algorithm.
    //Anything that inherits from INodeSearchable should be able to use this.
    //example: Tile inherits from INodeSearchable. If you were to replace the word "node" with "tile"
    //you should see how this works.
    INodeSearchable BreadthFirstBasic(INodeSearchable startNode, INodeSearchable targetNode)
    {
        Queue<INodeSearchable> nodeQueue = new Queue<INodeSearchable>();
        nodeQueue.Enqueue(startNode);

        INodeSearchable currentNode;

        while(nodeQueue.Count > 0)
        {
            currentNode = nodeQueue.Dequeue();

            if (currentNode == targetNode)
            {
                return currentNode;
            }
            else
            {
                currentNode.searched = true;
                foreach (var child in currentNode.children)
                {
                    if (!child.searched && !nodeQueue.Contains(child))
                    {
                        child.parent = currentNode;
                        nodeQueue.Enqueue(child);
                    }
                }
            }
            
        }
        //Queue empty, target not found: return null as a fail state
        return null;

    }

    //List<INodeSearchable> BreadthFirstOccupied(INodeSearchable startNode, INodeSearchable targetNode, int maxRange)
    //{
    //    Queue<INodeSearchable> nodeQueue = new Queue<INodeSearchable>();
    //    nodeQueue.Enqueue(startNode);

    //    INodeSearchable currentNode;

    //    while (nodeQueue.Count > 0)
    //    {
    //        currentNode = nodeQueue.Dequeue();

    //        if (currentNode)
    //        {
    //            return currentNode;
    //        }
    //        else
    //        {
    //            currentNode.searched = true;
    //            foreach (var child in currentNode.children)
    //            {
    //                if (!child.searched && !nodeQueue.Contains(child))
    //                {
    //                    child.parent = currentNode;
    //                    nodeQueue.Enqueue(child);
    //                }
    //            }
    //        }

    //    }
    //    //Queue empty, target not found: return null as a fail state
    //    return null;

    //}

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
