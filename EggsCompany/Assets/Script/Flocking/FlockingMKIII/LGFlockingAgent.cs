using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGFlockingAgent : MonoBehaviour
{
    private Vector3 agentVelocity = new Vector3();
    private float maxAgentVelocity = 5.0f;
    private float maxAgentVelocitySquared;
    private float maxAgentDistanceFromCenter = 3.0f;
    private GameObject objectToFollow;
    Vector3 centre = new Vector3();

    LGFlock agentFlock;
    SphereCollider agentCollider;

    // Start is called before the first frame update
    void Start()
    {
        maxAgentVelocitySquared = maxAgentVelocity * maxAgentVelocity;
        agentCollider = GetComponent<SphereCollider>();
        objectToFollow = AgentFlock.objToFollow;
        SetVelocity(transform.forward);
    }

    // Update is called once per frame
    void Update()
    {
        SetVelocity(agentVelocity + FlockingBehaviour());

        if (agentVelocity.sqrMagnitude > maxAgentVelocitySquared)
        {
            SetVelocity(agentVelocity.normalized * maxAgentVelocity);
        }

        MoveAgent(agentVelocity);

        // Reposition agent
        //This may not be entirely necessary if I can get the stayInRadius stuff working, though the Mathf.Lerp first statement and Time.delta time may not be correct for this.
        //Vector3 pos = transform.position;
        //if (pos.x >= objectToFollow.transform.position.x + maxAgentDistanceFromCenter)
        //{
        //    pos.x = Mathf.Lerp(transform.position.x, objectToFollow.transform.position.x + maxAgentDistanceFromCenter - 0.5f, Time.deltaTime);
        //    SetVelocity(agentVelocity * -1);
        //}
        //else if (pos.x <= objectToFollow.transform.position.x - maxAgentDistanceFromCenter)
        //{
        //    pos.x = Mathf.Lerp(transform.position.x, objectToFollow.transform.position.x - maxAgentDistanceFromCenter + 0.5f, Time.deltaTime);
        //    SetVelocity(agentVelocity * -1);
        //}

        //if (pos.y >= objectToFollow.transform.position.y + maxAgentDistanceFromCenter)
        //{
        //    pos.y = Mathf.Lerp(transform.position.y, objectToFollow.transform.position.y + maxAgentDistanceFromCenter - 0.5f, Time.deltaTime);
        //    SetVelocity(agentVelocity * -1);
        //}
        //else if (pos.y <= objectToFollow.transform.position.y - maxAgentDistanceFromCenter)
        //{
        //    pos.y = Mathf.Lerp(transform.position.y, objectToFollow.transform.position.y - maxAgentDistanceFromCenter + 0.5f, Time.deltaTime);
        //    SetVelocity(agentVelocity * -1);
        //}

        //if (pos.z >= objectToFollow.transform.position.z + maxAgentDistanceFromCenter)
        //{
        //    pos.z = Mathf.Lerp(transform.position.z, objectToFollow.transform.position.z + maxAgentDistanceFromCenter - 0.5f, Time.deltaTime);
        //    SetVelocity(agentVelocity * -1);
        //}
        //else if (pos.z <= objectToFollow.transform.position.z - maxAgentDistanceFromCenter)
        //{
        //    pos.z = Mathf.Lerp(transform.position.z, objectToFollow.transform.position.z - maxAgentDistanceFromCenter + 0.5f, Time.deltaTime);
        //    SetVelocity(agentVelocity * -1);
        //}

        //transform.forward = agentVelocity.normalized;
        //transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime);
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
        Vector3 stayInRadiusVector = new Vector3();

        int count = 0;

        foreach(LGFlockingAgent fa in AgentFlock.FlockAgents)
        {
            if(name != fa.name)
            {
                float distance = (fa.transform.position - transform.position).sqrMagnitude;
                float neighbourRadius = 5f;
                float neighbourRadiusSquared = neighbourRadius * neighbourRadius;
                if(distance > 0 && distance < neighbourRadiusSquared)
                {
                    alignmentVector += fa.transform.forward;
                    cohesionVector += fa.transform.position;
                    separationVector += fa.transform.position - transform.position;

                    count++;
                }
            }
        }

        if(count == 0)
        {
            return Vector3.zero;
        }

        // Calculate the average for the vectors from all the points added together.
        alignmentVector /= count;
        cohesionVector /= count;
        separationVector /= count;

        // Create offset from the transform of the flock agent.
        cohesionVector -= transform.position;

        separationVector *= -1;

        //Maybe?
        stayInRadiusVector = StayInRadius(5);
        stayInRadiusVector /= count;

        // Calculate the resultant vector of all the flocking behaviours multiplied by their respective weights.
        Vector3 resultantVector = ((alignmentVector * AgentFlock.alignmentWeight) + (cohesionVector * AgentFlock.cohesionWeight) + (separationVector * AgentFlock.separationWeight)
                                    + (stayInRadiusVector * AgentFlock.stayInRadiusWeight));

        return resultantVector;
    }

    Vector3 StayInRadius(float radius)
    {
       if(centre != objectToFollow.transform.position)
       {
            centre = objectToFollow.transform.position;
       }

        Vector3 centreOffset = centre - transform.position;
        float t = centreOffset.magnitude / radius;

        if(t < 0.9)
        {
            return Vector3.zero;
        }

        return centreOffset * t * t;
    }

    void MoveAgent(Vector3 velocity)
    {
        transform.forward = velocity.normalized;
        transform.position = Vector3.Lerp(transform.position, transform.position + velocity, Time.deltaTime);
    }
}
