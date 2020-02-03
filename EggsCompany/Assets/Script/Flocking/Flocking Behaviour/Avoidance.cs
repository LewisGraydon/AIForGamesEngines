using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Avoidance")]
public class Avoidance : FilteredFlockBehaviour
{
    public override Vector2 CalculateMovement(FlockAgent flockAgent, List<Transform> neighbours, Flock flock, GameObject objToFollow = null)
    {
        // If there is no neighbours, return no adjustment.
        if (neighbours.Count == 0)
        {
            return Vector2.zero;
        }

        // Add all points together and average
        Vector2 avoidanceMove = Vector2.zero;
        int numberToAvoid = 0;

        List<Transform> filteredNeighbours = (filter == null) ? neighbours : filter.Filter(flockAgent, neighbours);

        foreach (Transform item in filteredNeighbours)
        {
            if(Vector2.SqrMagnitude(item.position - flockAgent.transform.position) < flock.getAvoidanceRadiusSquared)
            {
                numberToAvoid++;
                avoidanceMove = (flockAgent.transform.position - item.position);
            }        
        }
        
        if(numberToAvoid > 0)
        {
            avoidanceMove /= numberToAvoid;
        }

        return avoidanceMove;
    }
}
