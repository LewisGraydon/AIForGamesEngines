using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public GameObject flockAgentPrefab;
    public GameObject[] allFlockAgents;
    public Vector3 movementLimits = new Vector3(5, 5, 5);

    [Range(10, 30)]
    public int flockSize = 20;

    [Header("Flock Settings")]
    [Range(0.0f, 5.0f)]
    public float minimumSpeed;
    [Range(0.0f, 5.0f)]
    public float maximumSpeed;

    [Range(1.0f, 10.0f)]
    public float neighbourDistance;
    [Range(0.0f, 5.0f)]
    public float rotationSpeed;


    // Start is called before the first frame update
    void Start()
    {     
        allFlockAgents = new GameObject[flockSize];
        for(int i = 0; i < flockSize; i++)
        {
            Vector3 flockPosition = transform.position + new Vector3(
                Random.Range(-movementLimits.x, movementLimits.x),
                Random.Range(-movementLimits.y, movementLimits.y),
                Random.Range(-movementLimits.z, movementLimits.z));

            allFlockAgents[i] = (GameObject)Instantiate(flockAgentPrefab, flockPosition, Quaternion.identity);
            allFlockAgents[i].GetComponent<Flock3D>().myFlockManager = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
