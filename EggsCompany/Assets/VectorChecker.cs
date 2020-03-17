using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VectorChecker : MonoBehaviour
{

    public GameObject otherFuckingCube;
    public float coverThreshhold = 0.44f;

    public Dictionary<ActionID, int> testBob = new Dictionary<ActionID, int>();
    // Start is called before the first frame update
    void Start()
    {
        for(ActionID i = ActionID.Move; i <= ActionID.Reload; i++)
        {
            testBob.Add(i, (int)i * UnityEngine.Random.Range(0, 10));
        }
        Debug.Log("Unordered");
        foreach (KeyValuePair<ActionID, int> kp in testBob)
        {
            Debug.Log("Key: " + kp.Key.ToString() + ", Value: " + kp.Value);
        }
        testBob = (Dictionary<ActionID, int>)testBob.OrderBy(key => key.Value);
        Debug.Log("Ordered");
        foreach (KeyValuePair<ActionID, int> kp in testBob)
        {
            Debug.Log("Key: " + kp.Key.ToString() + ", Value: " + kp.Value);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 normalDirection = (otherFuckingCube.transform.position - this.transform.position).normalized;
        
        //if(normalDirection.x >= coverThreshhold)
        //{
        //    print("observing cover from positive x");
        //}
        //else if (normalDirection.x <= -coverThreshhold)
        //{
        //    print("observing cover from negative x");
        //}
        //else
        //{
        //    print("normalDirection.x = " + normalDirection.x);
        //}
        //if (normalDirection.z >= coverThreshhold)
        //{
        //    print("observing cover from positive z");
        //}
        //else if (normalDirection.z <= -coverThreshhold)
        //{
        //    print("observing cover from negative z");
        //}
        //else
        //{
        //    print("normalDirection.z = " + normalDirection.z);
        //}
    }
}
