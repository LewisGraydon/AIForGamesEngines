using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorChecker : MonoBehaviour
{

    public GameObject otherFuckingCube;
    public float coverThreshhold = 0.44f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 normalDirection = (otherFuckingCube.transform.position - this.transform.position).normalized;
        
        if(normalDirection.x >= coverThreshhold)
        {
            print("observing cover from positive x");
        }
        else if (normalDirection.x <= -coverThreshhold)
        {
            print("observing cover from negative x");
        }
        else
        {
            print("normalDirection.x = " + normalDirection.x);
        }
        if (normalDirection.z >= coverThreshhold)
        {
            print("observing cover from positive z");
        }
        else if (normalDirection.z <= -coverThreshhold)
        {
            print("observing cover from negative z");
        }
        else
        {
            print("normalDirection.z = " + normalDirection.z);
        }
    }
}
