using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Ensure that anything that uses this FlockAgent script has the Collider2D component.
[RequireComponent(typeof(Collider2D))]
public class FlockAgent : MonoBehaviour
{
    Flock agentFlock;
    public Flock GetAgentFlock
    {
        get { return agentFlock; }
    }

    Collider2D flockAgentCollider;
    public Collider2D GetAgentCollider
    {
        get { return flockAgentCollider; }
    }

    // Start is called before the first frame update
    void Start()
    {
        flockAgentCollider = GetComponent<Collider2D>();
    }

    public void Initialize(Flock flock)
    {
        agentFlock = flock;
    }

    public void Move(Vector2 offsetPosition)
    {
        // We will assign to transform.up in 2D, transform.forward in 3D.
        transform.up = offsetPosition;

        transform.position += (Vector3)offsetPosition * Time.deltaTime;
    }
}
