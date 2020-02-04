using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGFlockingAgent : MonoBehaviour
{
    private Vector3 agentVelocity = new Vector3();
    private float maxAgentVelocity = 1.0f;
    private float maxAgentDistanceFromCenter = 3.0f;
    public GameObject objectToFollow;

    LGFlock agentFlock;
    SphereCollider agentCollider;

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<SphereCollider>();
        SetVelocity(transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
        SetVelocity(agentVelocity + FlockingBehaviour());
        transform.position += agentVelocity * Time.deltaTime;
        transform.forward = agentVelocity.normalized;

        Vector3 pos = transform.position;

        if (pos.x >= objectToFollow.transform.position.x + maxAgentDistanceFromCenter)
        {
            pos.x = objectToFollow.transform.position.x + maxAgentDistanceFromCenter - 0.2f;
            agentVelocity *= -1;
        }
        else if (pos.x <= objectToFollow.transform.position.x - maxAgentDistanceFromCenter)
        {
            pos.x = objectToFollow.transform.position.x - maxAgentDistanceFromCenter + 0.2f;
            agentVelocity *= -1;
        }

        if (pos.y >= objectToFollow.transform.position.y + maxAgentDistanceFromCenter)
        {
            pos.y = objectToFollow.transform.position.y + maxAgentDistanceFromCenter - 0.2f;
            agentVelocity *= -1;
        }
        else if (pos.y <= objectToFollow.transform.position.y - maxAgentDistanceFromCenter)
        {
            pos.y = objectToFollow.transform.position.y - maxAgentDistanceFromCenter + 0.2f;
            agentVelocity *= -1;
        }

        if (pos.z >= objectToFollow.transform.position.z + maxAgentDistanceFromCenter)
        {
            pos.z = objectToFollow.transform.position.z + maxAgentDistanceFromCenter - 0.2f;
            agentVelocity *= -1;
        }
        else if (pos.z <= objectToFollow.transform.position.z - maxAgentDistanceFromCenter)
        {
            pos.z = objectToFollow.transform.position.z - maxAgentDistanceFromCenter + 0.2f;
            agentVelocity *= -1;
        }

        transform.forward = agentVelocity.normalized;
        transform.position = pos;
    }

    public void Initialize(LGFlock flock)
    {
        agentFlock = flock;
    }

    public LGFlock AgentFlock
    {
        get { return agentFlock; }
    }

    public SphereCollider AgentCollider
    {
        get { return agentCollider; }
    }

    private void SetVelocity(Vector3 newVelocity)
    {
        agentVelocity = Vector3.ClampMagnitude(newVelocity, maxAgentVelocity);
    }

    private Vector3 FlockingBehaviour()
    {
        Vector3 alignmentVector = new Vector3();
        Vector3 cohesionVector = new Vector3();
        Vector3 separationVector = new Vector3();

        int count = 0;

        foreach(LGFlockingAgent fa in AgentFlock.FlockAgents)
        {
            if(name != fa.name)
            {
                float distance = (transform.position - fa.transform.position).sqrMagnitude;
                float neighbourRadius = 1.5f;
                float neighbourRadiusSquared = neighbourRadius * neighbourRadius;
                if(distance > 0 && distance < neighbourRadiusSquared)
                {
                    alignmentVector += fa.transform.forward;
                    cohesionVector += fa.transform.position;
                    separationVector += fa.transform.forward - transform.position;

                    count++;
                }
            }
        }

        if(count == 0)
        {
            return Vector3.zero;
        }

        alignmentVector /= count;

        cohesionVector /= count;
        cohesionVector -= transform.position;

        separationVector /= count;
        separationVector *= -1;

        Vector3 resultantVector = ((alignmentVector * AgentFlock.alignmentWeight) + (cohesionVector * AgentFlock.cohesionWeight) + (separationVector * AgentFlock.separationWeight));

        return resultantVector;
    }
}
