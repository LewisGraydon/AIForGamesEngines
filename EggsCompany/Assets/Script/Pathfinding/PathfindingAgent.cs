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

    public static int BreadthFirstAllySearch(INodeSearchable startNode)
    {
        int numberOfAllys = 0;
        Queue<INodeSearchable> nodeQueue = new Queue<INodeSearchable>();
        nodeQueue.Enqueue(startNode);

        INodeSearchable currentNode;

        while (nodeQueue.Count > 0)
        {
            currentNode = nodeQueue.Dequeue();

            if ((currentNode as Tile).occupier == ETileOccupier.EnemyCharacter)
            {
                numberOfAllys++;
            }
            else
            {
                currentNode.searched = true;
                if(currentNode.parent == startNode || currentNode == startNode)
                {
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

        }
        //Queue empty, target not found: return null as a fail state
        return numberOfAllys;
    }

    //An implementation of a best first search which uses a single heuristic.
    //Currently unfinished. The current implementation is very greedy, and if anything were
    //to block its path the search would fail.
    //A priority queue with a self-balancing binary tree would be better optimization.
    //For now, a list that sorts after an inser will work for a quick/dirty implementaion.
    //That is the next current step.
    INodeSearchable BestFirst(INodeSearchable startNode, INodeSearchable targetNode, EHeuristic heuristic)
    {
        Queue<INodeSearchable> nodeQueue = new Queue<INodeSearchable>();
        nodeQueue.Enqueue(startNode);

        

        INodeSearchable currentNode;
        INodeSearchable bestNode = null;
        float bestDistance = 0;

        while (nodeQueue.Count > 0)
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

                        switch (heuristic)
                        {
                            case EHeuristic.Distance:
                                //Checking what the node is: We could attach an enum to the interface which allows us
                                //to know what it is. Maybe? Could be bad coding practice; do research when possible.
                                //Add error checking.
                                Tile targetTile = targetNode as Tile;
                                Tile childTile = child as Tile;

                                float childMagnitude = childTile.transform.position.magnitude;
                                float targetMagnitude = targetTile.transform.position.magnitude;
                                float distance = targetMagnitude - childMagnitude;

                                if(distance < bestDistance || bestDistance == 0)
                                {
                                    bestNode = child;
                                    bestDistance = distance;
                                }

                                break;
                            case EHeuristic.Manhattan:
                                throw new System.NotImplementedException("Manhattan Style Distance Calculation not yet implemented.");
                                break;
                            default:
                                bestNode = null;
                                Debug.LogError("No heuristic provided for Pathfinding Agent func BestFirst search with start: "
                                    + startNode + " and target: " + targetNode);
                                break;
                        }                     
                    }
                }

                if(bestNode != null)
                {
                    nodeQueue.Enqueue(bestNode);
                    bestNode = null;
                }

            }

        }
        //Queue empty, target not found: return null as a fail state
        return null;

    }


    //For path part of pathfinding


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
