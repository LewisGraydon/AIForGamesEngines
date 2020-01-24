using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Abstract class as we will never instanciate a flock behaviour.
public abstract class FlockBehaviour : ScriptableObject
{
    public abstract Vector2 CalculateMovement(FlockAgent flockAgent, List<Transform> neighbours, Flock flock);
}
