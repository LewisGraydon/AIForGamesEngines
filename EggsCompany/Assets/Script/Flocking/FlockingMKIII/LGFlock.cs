using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LGFlock : MonoBehaviour
{
    public LGFlockingAgent flockAgentPrefab;
    private List<LGFlockingAgent> flockAgents = new List<LGFlockingAgent>();

    [Range(5, 50)]
    public int flockStartingSize = 25;
    const float flockAgentDensity = 0.08f;

    [Range(0.0f, 10.0f)]
    public float alignmentWeight = 0.5f;

    [Range(0.0f, 10.0f)]
    public float cohesionWeight = 0.7f;

    [Range(0.0f, 10.0f)]
    public float separationWeight = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < flockStartingSize; i++)
        {
            LGFlockingAgent newFlockAgent = Instantiate(flockAgentPrefab, Random.insideUnitSphere * flockStartingSize * flockAgentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0.0f, 360.0f)), transform);

            newFlockAgent.name = "Agent " + i;
            newFlockAgent.Initialize(this);
            flockAgents.Add(newFlockAgent);
        }
    }

    public List<LGFlockingAgent> FlockAgents
    {
        get { return flockAgents; }
    }
}
