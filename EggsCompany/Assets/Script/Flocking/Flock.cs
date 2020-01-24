using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockAgent flockAgentPrefab;
    public FlockBehaviour flockBehaviour;
    List<FlockAgent> flockAgents = new List<FlockAgent>();  

    [Range(10, 500)]
    public int flockStartingSize = 250;
    const float agentDensity = 0.08f;

    [Range(1.0f, 100.0f)]
    public float driveFactor = 10.0f;

    [Range(1.0f, 100.0f)]
    public float maximumSpeed = 5.0f;

    [Range(1.0f, 10.0f)]
    public float neighbourRadius = 1.5f;

    [Range(0.0f, 1.0f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    float maximumSpeedSquared;
    float neighbourRadiusSquared;
    float avoidanceRadiusSquared;
    public float getAvoidanceRadiusSquared
    {
        get
        {
            return avoidanceRadiusSquared;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        maximumSpeedSquared = maximumSpeed * maximumSpeed;
        neighbourRadiusSquared = neighbourRadius * neighbourRadius;
        avoidanceRadiusSquared = neighbourRadiusSquared * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        for( int i = 0; i < flockStartingSize; i++)
        {
            FlockAgent newFlockAgent = Instantiate(
                flockAgentPrefab,
                Random.insideUnitCircle * flockStartingSize * agentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0.0f, 360.0f)),
                transform
                );

            newFlockAgent.name = "Agent " + i;
            newFlockAgent.Initialize(this);
            flockAgents.Add(newFlockAgent);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(FlockAgent flockAgent in flockAgents)
        {
            List<Transform> neighbours = GetNearbyObjects(flockAgent);

            // For demonstration purposes.
            //flockAgent.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, neighbours.Count / 6.0f);

            Vector2 movement = flockBehaviour.CalculateMovement(flockAgent, neighbours, this);
            movement *= driveFactor;

            if (movement.sqrMagnitude > maximumSpeedSquared)
            {
                movement = movement.normalized * maximumSpeed;
            }
            flockAgent.Move(movement);
        }
    }

    List<Transform> GetNearbyObjects(FlockAgent flockAgent)
    {
        List<Transform> neighbours = new List<Transform>();
        // In 3D use collider and physics overlap sphere.
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(flockAgent.transform.position, neighbourRadius);

        foreach(Collider2D c in nearbyColliders)
        {
            if(c != flockAgent.GetAgentCollider)
            {
                neighbours.Add(c.transform);
            }
        }

        return neighbours;
    }
}
