using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            gameObject.transform.position += new Vector3(1.0f, 0.0f, 0.0f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            gameObject.transform.position += new Vector3(-1.0f, 0.0f, 0.0f);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            gameObject.transform.position += new Vector3(0.0f, 1.0f, 0.0f);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            gameObject.transform.position += new Vector3(0.0f, -1.0f, 0.0f);
        }
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            gameObject.transform.position += new Vector3(0.0f, 0.0f, 1.0f);
        }
        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            gameObject.transform.position += new Vector3(0.0f, 0.0f, -1.0f);
        }
    }
}
