using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/StayInRadius")]
public class StayInRadius : FlockBehaviour
{
    public Vector2 center;
    public float radius = 15.0f;

    public override Vector2 CalculateMovement(FlockAgent flockAgent, List<Transform> neighbours, Flock flock)
    {
        Vector2 centerOffset = center - (Vector2)flockAgent.transform.position;
        float t = centerOffset.magnitude / radius;

        if (t < 0.9f)
        {
            return Vector2.zero;
        }

        return centerOffset * t * t;
    }

}
