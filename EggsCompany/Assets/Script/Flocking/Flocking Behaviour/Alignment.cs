using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Alignment")]
public class Alignment : FilteredFlockBehaviour
{
    public override Vector2 CalculateMovement(FlockAgent flockAgent, List<Transform> neighbours, Flock flock)
    {
        // If there is no neighbours, maintain current alignment.
        if (neighbours.Count == 0)
        {
            return flockAgent.transform.up;
        }

        // Add all points together and average
        Vector2 alignmentMove = Vector2.zero;
        List<Transform> filteredNeighbours = (filter == null) ? neighbours : filter.Filter(flockAgent, neighbours);

        foreach (Transform item in filteredNeighbours)
        {
            alignmentMove += (Vector2)item.transform.up;
        }
        alignmentMove /= neighbours.Count;

        return alignmentMove;
    }
}
