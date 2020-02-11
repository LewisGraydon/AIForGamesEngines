using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGFlockingAgent : MonoBehaviour
{
    private Vector3 agentVelocity = new Vector3();
    private float maxAgentVelocity = 5.0f;
    private float maxAgentVelocitySquared;
    private float maxAgentDistanceFromCenter = 1.0f;
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
        // Create the behaviour vectors and initialise them to be empty.
        Vector3 alignmentVector = new Vector3();
        Vector3 cohesionVector = new Vector3();
        Vector3 separationVector = new Vector3();
        Vector3 stayInRadiusVector = new Vector3();

        int count = 0;

        foreach(LGFlockingAgent fa in AgentFlock.FlockAgents)
        {
            // Ensure we aren't checking against self
            if(name != fa.name)
            {

                float distance = (fa.transform.position - transform.position).sqrMagnitude;
                float neighbourRadius = 5f;
                float neighbourRadiusSquared = neighbourRadius * neighbourRadius;
                if(distance > 0 && distance < neighbourRadiusSquared)
                {
                    // We
                    alignmentVector += fa.transform.forward;
                    cohesionVector += fa.transform.position;
                    separationVector += fa.transform.position - transform.position;

                    // Increment the count variable, this is used to keep track of the number of 
                    count++;
                }
            }
        }

        // To avoid a nasty division by 0, if count is zero then there are no agents in the flock so no flocking behaviour will need to be done. Instead we return a zero vector.
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

        // Calculate the amount we need to move the agent by to get it back within the radius specified.
        stayInRadiusVector = StayInRadius(maxAgentDistanceFromCenter);

        // Calculate the average amount we need to move the agent by.
        stayInRadiusVector /= count;

        // Calculate the resultant vector of all the flocking behaviours multiplied by their respective weights.
        Vector3 resultantVector = ((alignmentVector * AgentFlock.alignmentWeight) + (cohesionVector * AgentFlock.cohesionWeight) + (separationVector * AgentFlock.separationWeight)
                                    + (stayInRadiusVector * AgentFlock.stayInRadiusWeight));

        return resultantVector;
    }

    Vector3 StayInRadius(float radius)
    {
        // If the object we are wanting to stay in a radius of has moved, we will set the centre to be its new position.
        if(centre != objectToFollow.transform.position)
        {
            centre = objectToFollow.transform.position;
        }

        // Calculate the distance we are from the centre.
        Vector3 centreOffset = centre - transform.position;

        // Calculate whether the agent is within the radius we want it to stay in.
        float t = centreOffset.magnitude / radius;

        // If the agent is within the radius then return no modification.
        if(t < 0.9)
        {    
            return Vector3.zero;
        }

        // Return the modification we want to make to the agent.
        return centreOffset * t * t;
    }

    void MoveAgent(Vector3 velocity)
    {
        // Sets the rotation of the agent to be the normalised vector of the direction it is moving in.
        transform.forward = velocity.normalized;

        // When we want to actually move the agent to the next positon, we will linearly interpolate between the current position,
        // and the future position (which will be the current position added to amount we are moving by) over the a time step of Time.deltaTime.
        transform.position = Vector3.Lerp(transform.position, transform.position + velocity, Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // On detecting a collision, we will calculate the vector from the position of the object we have collided with and the current position of this object.
        // We will then add that to the current velocity of this agent to find the resultant velocity of the collision.
        Vector3 vectorBetweenCollisionAndThis = transform.position - collision.gameObject.transform.position;
        agentVelocity += vectorBetweenCollisionAndThis;
    }

    private void OnCollisionStay(Collision collision)
    {
        OnCollisionEnter(collision);
    }

}
