using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Composite")]
public class CompositeBehaviour : FlockBehaviour
{
    public FlockBehaviour[] flockBehaviours;
    public float[] weights;

    public override Vector2 CalculateMovement(FlockAgent flockAgent, List<Transform> neighbours, Flock flock, GameObject objToFollow = null)
    {
        // Handle the two arrays not being the same size.
        if (weights.Length != flockBehaviours.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector2.zero;
        }

        Vector2 movement = Vector2.zero;

        for(int i = 0; i < flockBehaviours.Length; i++)
        {
            Vector2 partialMovement = flockBehaviours[i].CalculateMovement(flockAgent, neighbours, flock, objToFollow) * weights[i];

            if(partialMovement != Vector2.zero)
            {
                if(partialMovement.sqrMagnitude > weights[i] * weights[i])
                {
                    partialMovement.Normalize();
                    partialMovement *= weights[i];
                }

                movement += partialMovement;
            }
        }

        return movement;
    }
}
