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
    //A priority queue with a self-balancing binary tree would be better optimization.
    //For now, a list that sorts after an insert will work for a quick/dirty implementaion.
    //That is the next current step.
    INodeSearchable BestFirst(INodeSearchable startNode, INodeSearchable targetNode, EHeuristic heuristic)
    {
        //Queue<INodeSearchable> nodeQueue = new Queue<INodeSearchable>();
        //nodeQueue.Enqueue(startNode);
        List<INodeSearchable> nodeList = new List<INodeSearchable>();
        nodeList.Add(startNode);

        INodeSearchable currentNode;
        //INodeSearchable bestNode = null;
        //float bestDistance = 0;

        while (nodeList.Count > 0)
        {
            //currentNode = nodeQueue.Dequeue();
            currentNode = nodeList[0];
            nodeList.RemoveAt(0);

            if (currentNode == targetNode)
            {
                return currentNode;
            }
            else
            {
                currentNode.searched = true;

                foreach (var child in currentNode.children)
                {
                    if (!child.searched && !nodeList.Contains(child))
                    {
                        child.parent = currentNode;

                        switch (heuristic)
                        {
                            case EHeuristic.Distance:
                                //Checking what the node is: We could attach an enum to the interface which allows us
                                //to know what it is. Maybe? Could be bad coding practice; do research when possible.
                                //Add error checking. eg: expected object tile got object {x}
                                Tile targetTile = targetNode as Tile;
                                Tile childTile = child as Tile;

                                float childMagnitude = childTile.transform.position.magnitude;
                                float targetMagnitude = targetTile.transform.position.magnitude;
                                childTile.distanceToTarget = targetMagnitude - childMagnitude;

                                //if(distance < bestDistance || bestDistance == 0)
                                //{
                                //    bestNode = child;
                                //    bestDistance = distance;
                                //}

                                nodeList.Add(childTile);
                                TileDistanceComparison testcompare = new TileDistanceComparison();
                                nodeList.Sort(testcompare);
                                break;
                            case EHeuristic.Manhattan:
                                throw new System.NotImplementedException("Manhattan Style Distance Calculation not yet implemented.");
                                break;
                            default:
                                //bestNode = null;
                                Debug.LogError("No heuristic provided for Pathfinding Agent func BestFirst search with start: "
                                    + startNode + " and target: " + targetNode);
                                break;
                        }                     
                    }
                }



                //if(bestNode != null)
                //{
                //    nodeQueue.Enqueue(bestNode);
                //    bestNode = null;
                //}

            }

        }
        //Queue empty, target not found: return null as a fail state
        return null;

    }


    //A Dijkstra Search implementation focused on finding the most efficent way to move to a target tile given the environemental cost to get there.
    INodeSearchable DijkstraSearch(INodeSearchable startNode, INodeSearchable targetNode, List<INodeSearchable> searchSet)
    {

        INodeSearchable currentNode;
        currentNode = startNode;

        //Is there a way to assign a child list directly? Would save several operations here

        foreach (var node in searchSet)
        {
            node.Cost = null;
        }
        //Ensures we don't check our starting node twice, and that our starting node is at the front of our "queue"
        searchSet.Remove(currentNode);
        searchSet.Insert(0, currentNode);
        currentNode.Cost = 0;

        while (searchSet.Count > 0)
        {
            currentNode = searchSet[0];
            searchSet.RemoveAt(0);
            searchSet.TrimExcess();

            if (currentNode == targetNode)
            {
                return targetNode;
            }
            else if (currentNode.Cost == null)
            {
                //Remaining nodes are assumed unreachable, no path to target, return null as a fail state
                return null;
            }
            else
            {
                foreach (var child in currentNode.children)
                {
                    //calculate cost to reach child (temp value used for psudocode)

                    int? calculatedCost = currentNode.Cost /* + CalculateCostToReachChild() */;
                    //Assign that cost to child if: child cost value is null OR < calced cost
                    //If assigning cost: make parent current node
                    if(calculatedCost < child.Cost || child.Cost == null)
                    {
                        child.Cost = calculatedCost;
                        child.parent = currentNode;
                    }



                }

            }

            //CREATE COMPARER FOR SORT SEE NOTES BELLOW.
            searchSet.Sort();

        }

        //Next Steps:
        //Calculate cost for reaching child
        //Create comparer for nodesearchable for cost, let null cost be "greater than" other values to shove them to the back of the list druing a sort.

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
