﻿using System.Collections;
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
                    if (child == null)
                    {
                        continue;
                    }
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
                        if (child == null)
                        {
                            continue;
                        }
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
                    if (child == null)
                    {
                        continue;
                    }
                    if (!child.searched && !nodeList.Contains(child))
                    {
                        child.parent = currentNode;
                        CalculateHeuristic(targetNode, child, heuristic);

                        nodeList.Add(child);
                        TileDistanceComparison compare = new TileDistanceComparison();
                        nodeList.Sort(compare);

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
    INodeSearchable DijkstraSearch(INodeSearchable startNode, INodeSearchable targetNode, List<INodeSearchable> searchSet, ECostType costType)
    {

        INodeSearchable currentNode;
        currentNode = startNode;

        foreach (var node in searchSet)
        {
            node.DijkstraCost = null;
        }
        //Ensures we don't check our starting node twice, and that our starting node is at the front of our "queue"
        searchSet.Remove(currentNode);
        searchSet.Insert(0, currentNode);
        currentNode.DijkstraCost = 0;

        while (searchSet.Count > 0)
        {
            currentNode = searchSet[0];
            searchSet.RemoveAt(0);
            searchSet.TrimExcess();

            if (currentNode == targetNode)
            {
                return targetNode;
            }
            else if (currentNode.DijkstraCost == null)
            {
                //Remaining nodes are assumed unreachable, no path to target, return null as a fail state
                return null;
            }
            else
            {
                foreach (var child in currentNode.children)
                {
                    if (child == null)
                    {
                        continue;
                    }
                    CalculateDijkstra(currentNode, child, costType);
                    child.TotalCost = child.DijkstraCost;
                }

            }

            IComparer<INodeSearchable> nullback = new SortNullToBackByTotalCost();
            searchSet.Sort(nullback);

        }

        return null;
    }

    //@Param startNode: The INodeSearchable object the search should start from.
    //@Param targetNode: The INodeSearchable object the search should be attempting to reach.
    //@Param searchSet: A list containing all nodes to be searched/in the search range.
    INodeSearchable AStarBasic(INodeSearchable startNode, INodeSearchable targetNode, List<INodeSearchable> searchSet, ECostType costType, EHeuristic heuristic)
    {
        //A* selects the path that minimizes:
        //f(x) = g(x) + h(x)
        //Where g(x) is the cost of the node, and h(x) is the cost of heursitic

        INodeSearchable currentNode;
        currentNode = startNode;

        foreach (var node in searchSet)
        {
            node.DijkstraCost = null;
            node.HeuristicCost = null;
            node.TotalCost = null;
        }
        //Ensures we don't check our starting node twice, and that our starting node is at the front of our "queue"
        searchSet.Remove(currentNode);
        searchSet.Insert(0, currentNode);
        currentNode.DijkstraCost = 0;

        while (searchSet.Count > 0)
        {
            currentNode = searchSet[0];
            searchSet.RemoveAt(0);
            searchSet.TrimExcess();

            if (currentNode == targetNode)
            {
                return targetNode;
            }
            else if (currentNode.DijkstraCost == null)
            {
                //Remaining nodes are assumed unreachable, no path to target, return null as a fail state
                return null;
            }
            else
            {
                foreach (var child in currentNode.children)
                {
                    if(child == null)
                    {
                        continue;
                    }
                    //Calc the heuristic cost of chosen heuristic measure: h(x)
                    CalculateHeuristic(targetNode, child, heuristic);

                    //Calc the Dijkstra cost of chosen cost measure g(x)
                    CalculateDijkstra(currentNode, child, costType);

                    //Calc the total cost: f(x) = g(x) + h(x)
                    child.TotalCost = child.DijkstraCost + child.HeuristicCost;

                    if (!searchSet.Contains(child))
                    {
                        searchSet.Add(child);
                    }

                }

            }

            IComparer<INodeSearchable> nullback = new SortNullToBackByTotalCost();
            searchSet.Sort(nullback);

        }

        return null;
    }

    public float? CalculateMoveCostToReachChildTile(INodeSearchable parent, INodeSearchable child)
    {
        Tile tileChild = child as Tile;
        Tile tileParent = parent as Tile;
        EDirection directionToParent = EDirection.Error;
        EDirection directionToChild;
        EWallDirection wallDirectionToChild;
        EWallDirection wallDirectionToParent;
        int wallCostFromChild = 999;
        int wallCostFromParent = 999;
        int floorCostFromChild = 999;

        for (int i = 0; i < (int)EDirection.Error; i++)
        {
            if (tileChild.neighbors[i] == tileParent)
            {
                directionToParent = (EDirection)i;
                break;
            }
        }

        directionToChild = FlipDirection(directionToParent);

        //this switchcase feel wet, might want to refactor
        switch (directionToChild)
        {
            case EDirection.North:
                wallDirectionToChild = EDirectionToEWallDirection(directionToChild);
                wallDirectionToParent = EDirectionToEWallDirection(directionToParent);

                wallCostFromChild = tileChild.walls[(int)wallDirectionToParent].moveCost;
                wallCostFromParent = tileParent.walls[(int)wallDirectionToChild].moveCost;
                floorCostFromChild = tileChild.terrainType.moveCost;
                break;

            case EDirection.NorthEast:
                if ((tileChild.walls[(int)EWallDirection.South].moveCost != 0 || tileChild.walls[(int)EWallDirection.West].moveCost != 0) ||
                    (tileParent.walls[(int)EWallDirection.North].moveCost != 0 || tileParent.walls[(int)EWallDirection.East].moveCost != 0))
                {
                    wallCostFromChild = 999;
                    wallCostFromParent = 999;
                }
                else
                {
                    wallCostFromChild = 0;
                    wallCostFromParent = 0;
                }
                floorCostFromChild = tileChild.terrainType.moveCost;
                break;

            case EDirection.East:
                wallDirectionToChild = EDirectionToEWallDirection(directionToChild);
                wallDirectionToParent = EDirectionToEWallDirection(directionToParent);

                wallCostFromChild = tileChild.walls[(int)wallDirectionToParent].moveCost;
                wallCostFromParent = tileParent.walls[(int)wallDirectionToChild].moveCost;
                floorCostFromChild = tileChild.terrainType.moveCost;
                break;

            case EDirection.SouthEast:
                if ((tileChild.walls[(int)EWallDirection.North].moveCost != 0 || tileChild.walls[(int)EWallDirection.West].moveCost != 0) ||
                    (tileParent.walls[(int)EWallDirection.South].moveCost != 0 || tileParent.walls[(int)EWallDirection.East].moveCost != 0))
                {
                    wallCostFromChild = 999;
                    wallCostFromParent = 999;
                }
                else
                {
                    wallCostFromChild = 0;
                    wallCostFromParent = 0;
                }
                floorCostFromChild = tileChild.terrainType.moveCost;
                break;

            case EDirection.South:
                wallDirectionToChild = EDirectionToEWallDirection(directionToChild);
                wallDirectionToParent = EDirectionToEWallDirection(directionToParent);
                wallCostFromChild = tileChild.walls[(int)wallDirectionToParent].moveCost;
                wallCostFromParent = tileParent.walls[(int)wallDirectionToChild].moveCost;
                floorCostFromChild = tileChild.terrainType.moveCost;
                break;

            case EDirection.SouthWest:
                if ((tileChild.walls[(int)EWallDirection.North].moveCost != 0 || tileChild.walls[(int)EWallDirection.East].moveCost != 0) ||
                    (tileParent.walls[(int)EWallDirection.South].moveCost != 0 || tileParent.walls[(int)EWallDirection.West].moveCost != 0))
                {
                    wallCostFromChild = 999;
                    wallCostFromParent = 999;
                }
                else
                {
                    wallCostFromChild = 0;
                    wallCostFromParent = 0;
                }
                floorCostFromChild = tileChild.terrainType.moveCost;
                break;

            case EDirection.West:
                wallDirectionToChild = EDirectionToEWallDirection(directionToChild);
                wallDirectionToParent = EDirectionToEWallDirection(directionToParent);

                wallCostFromChild = tileChild.walls[(int)wallDirectionToParent].moveCost;
                wallCostFromParent = tileParent.walls[(int)wallDirectionToChild].moveCost;
                floorCostFromChild = tileChild.terrainType.moveCost;
                break;

            case EDirection.NorthWest:
                if ((tileChild.walls[(int)EWallDirection.South].moveCost != 0 || tileChild.walls[(int)EWallDirection.East].moveCost != 0) ||
                    (tileParent.walls[(int)EWallDirection.North].moveCost != 0 || tileParent.walls[(int)EWallDirection.West].moveCost != 0))
                {
                    wallCostFromChild = 999;
                    wallCostFromParent = 999;
                }
                else
                {
                    wallCostFromChild = 0;
                    wallCostFromParent = 0;
                }
                floorCostFromChild = tileChild.terrainType.moveCost;
                break;

            case EDirection.Error:
                break;
            default:

                break;
        }

        return wallCostFromChild + wallCostFromParent + floorCostFromChild;
    }

    private EWallDirection EDirectionToEWallDirection(EDirection direction)
    {
        switch (direction)
        {
            case EDirection.North:
                return EWallDirection.North;
            case EDirection.East:
                return EWallDirection.East;
            case EDirection.South:
                return EWallDirection.South;
            case EDirection.West:
                return EWallDirection.West;
            case EDirection.Error:
                return EWallDirection.Error;
            default:
                return EWallDirection.Error;
        }
    }

    public EDirection FlipDirection(EDirection direction)
    {
        EDirection newDirection;

        switch (direction)
        {
            case EDirection.North:
                newDirection = EDirection.South;
                break;
            case EDirection.NorthEast:
                newDirection = EDirection.SouthWest;
                break;
            case EDirection.East:
                newDirection = EDirection.West;
                break;
            case EDirection.SouthEast:
                newDirection = EDirection.NorthWest;
                break;
            case EDirection.South:
                newDirection = EDirection.North;
                break;
            case EDirection.SouthWest:
                newDirection = EDirection.NorthEast;
                break;
            case EDirection.West:
                newDirection = EDirection.East;
                break;
            case EDirection.NorthWest:
                newDirection = EDirection.SouthEast;
                break;
            default:
                newDirection = EDirection.Error;
                Debug.LogError("Error case reached for func FlipDirection in PathfindingAgent.");
                throw new System.ArgumentNullException("Invalid Direction provided at FlipDirection.");
                break;
        }

        return newDirection;

    }

    public void CalculateDijkstra(INodeSearchable currentNode, INodeSearchable child, ECostType costType)
    {

        float? calculatedCost = currentNode.DijkstraCost;

        switch (costType)
        {
            case ECostType.Movement:

                calculatedCost += CalculateMoveCostToReachChildTile(currentNode, child);

                break;
            default:
                break;
        }

        if (calculatedCost < child.DijkstraCost || child.DijkstraCost == null)
        {
            child.DijkstraCost = calculatedCost;
            child.parent = currentNode;
        }
    }

    public void CalculateHeuristic(INodeSearchable targetNode, INodeSearchable child, EHeuristic heuristic)
    {
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
                child.HeuristicCost = targetMagnitude - childMagnitude;

                //if(distance < bestDistance || bestDistance == 0)
                //{
                //    bestNode = child;
                //    bestDistance = distance;
                //}

                //nodeList.Add(childTile);
                //TileDistanceComparison compare = new TileDistanceComparison();
                //nodeList.Sort(compare);
                break;
            case EHeuristic.Manhattan:
                throw new System.NotImplementedException("Manhattan Style Distance Calculation not yet implemented.");
                break;
            default:
                //bestNode = null;
                Debug.LogError("No heuristic provided for Pathfinding Agent func BestFirst search with target: " + targetNode);
                break;
        }
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
