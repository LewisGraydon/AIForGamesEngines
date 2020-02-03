using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/SteeredCohesion")]
public class SteeredCohesion : FilteredFlockBehaviour
{
    Vector2 currentVelocity;
    public float flockAgentSmoothTime = 0.5f;

    public override Vector2 CalculateMovement(FlockAgent flockAgent, List<Transform> neighbours, Flock flock, GameObject objToFollow = null)
    {
        // If there is no neighbours, return no adjustment.
        if (neighbours.Count == 0)
        {
            return Vector2.zero;
        }

        // Add all points together and average
        Vector2 cohesionMove = Vector2.zero;

        List<Transform> filteredNeighbours = (filter == null) ? neighbours : filter.Filter(flockAgent, neighbours);

        foreach (Transform item in filteredNeighbours)
        {
            cohesionMove += (Vector2)item.position;
        }
        cohesionMove /= neighbours.Count;

        // Create offset from flock agent position
        cohesionMove -= (Vector2)flockAgent.transform.position;
        cohesionMove = Vector2.SmoothDamp(flockAgent.transform.up, cohesionMove, ref currentVelocity, flockAgentSmoothTime);

        return cohesionMove;
    }
}
