using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/StayInRadius")]
public class StayInRadius : FlockBehaviour
{
    Vector2 center = new Vector2(0,0);
    public float radius = 15.0f;

    public override Vector2 CalculateMovement(FlockAgent flockAgent, List<Transform> neighbours, Flock flock, GameObject objToFollow = null)
    {
        if (center != (Vector2)objToFollow.transform.position)
        {
            center = objToFollow.transform.position;
            Debug.Log(center);
        }
        
        Vector2 centerOffset = center - (Vector2)flockAgent.transform.position;
        float t = centerOffset.magnitude / radius;

        if (t < 0.9f)
        {
            return Vector2.zero;
        }

        return centerOffset * t * t;
    }

}
