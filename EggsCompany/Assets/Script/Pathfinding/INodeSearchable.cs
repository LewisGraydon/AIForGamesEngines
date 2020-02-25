using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interface which implements a node for certain algorithmic searches.
public interface INodeSearchable
{
    bool searched
    {
        get;
        set;
    }

    //Costs ref types are ? to make them nullable.
    //This is because costs of 0 or high numbers are seen as valid costs by the search systems.
    //Null allows us to check for unassigned/non-caculated costs in the searches.

    float? DijkstraCost
    {
        get;
        set;
    }

    float? HeuristicCost
    {
        get;
        set;
    }

    float? TotalCost
    {
        get;
        set;
    }

    INodeSearchable parent
    {
        get;
        set;
    }
    List<INodeSearchable> children
    {
        get;
        set;
    }

}
