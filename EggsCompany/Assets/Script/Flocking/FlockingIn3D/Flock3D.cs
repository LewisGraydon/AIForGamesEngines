using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock3D : MonoBehaviour
{
    public FlockManager myFlockManager;
    float speed;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(myFlockManager.minimumSpeed, myFlockManager.maximumSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, 0, Time.deltaTime * speed);
        ApplyRules();
    }

    void ApplyRules()
    {
        GameObject[] gameObjectsInCurrentFlock;
        gameObjectsInCurrentFlock = myFlockManager.allFlockAgents;

        Vector3 averageCentre = Vector3.zero;
        Vector3 averageAvoidance = Vector3.zero;
        float globalSpeed = 0.01f;
        float distanceToGroup;
        int groupSize = 0;

        foreach(GameObject go in gameObjectsInCurrentFlock)
        {
            if(go != gameObject)
            {
                distanceToGroup = Vector3.Distance(go.transform.position, transform.position);
                if(distanceToGroup <= myFlockManager.neighbourDistance)
                {
                    averageCentre += go.transform.position;
                    groupSize++;

                    if (distanceToGroup < 1.0f)
                    {
                        averageAvoidance += (transform.position - go.transform.position);
                    }

                    Flock3D anotherFlock = go.GetComponent<Flock3D>();
                    globalSpeed += anotherFlock.speed;
                }
            }
        }

        if(groupSize > 0)
        {
            averageCentre /= groupSize;
            speed = globalSpeed / groupSize;

            Vector3 direction = (averageCentre + averageAvoidance) - transform.position;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), myFlockManager.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
