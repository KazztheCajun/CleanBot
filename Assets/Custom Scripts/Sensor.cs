using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{

    public GameObject line_pre;
    private List<GameObject> lines;

    // Start is called before the first frame update
    void Start()
    {
        lines = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject g in lines)
        {
            SetColor(g);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject other_obj = other.gameObject; // cache collided object
        bool isNew = true; // tracks newly detected objects
        GameObject line = null; // reference to already created lines

        foreach(GameObject g in lines) // check the list of lines to see if this is a new obstacle to track
        {
            if (g.GetComponent<SensorLine>().obj == other_obj.transform) // if a line already has the detected object, it cannot be a new line
            {
                isNew = false; // it is not a new line
                line = g; // store the reference to the line
                break;
            }
        }

        if(isNew) // if a new line needs to be created
        {
            GameObject temp = Instantiate(line_pre, this.transform.position, Quaternion.identity); // create new line object
            temp.transform.parent = this.transform; // set this sensor as it's parent
            SensorLine l = temp.GetComponent<SensorLine>();
            l.sensor = this.transform; // set this sensor as it's sensor location
            l.obj = other_obj.transform; // set the object as it's obstacle location
            lines.Add(temp); // add it to the list of lines
            temp.SetActive(true); // ensure it is active in the scene
        }
        else // otherwise reavtivate the object we found
        {
            if(line != null)
            {
                line.SetActive(true); // make the line visible
            }   
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject other_obj = other.gameObject; // cache collided object

        foreach(GameObject g in lines) // check the list of lines to see if this is a new obstacle to track
        {
            if (g.GetComponent<SensorLine>().obj == other_obj.transform) // if a line already has the detected object, it cannot be a new line
            {
                g.SetActive(false);
            }
        }
    }

    private void SetColor(GameObject other)
    {
        SensorLine l = other.GetComponent<SensorLine>();
        if(CheckVisibility(other)) // if the robot can see the obstacle make the line green
        {
            l.color = Color.green;
        }
        else // otherwise make the line red
        {
            l.color = Color.red;
        }
        
    }

    private bool CheckVisibility(GameObject other)
    {
        /*
         * Checks if the given GameObject is visible to the sensor
         * Basically draws a ray to the detected object and if there is nothing blocking that ray
         * then the sensor can see the object.
         * Green lines for objects that can be seen
         * Red lines for objects that are hidden 
         */

        RaycastHit target; // hold a spot for the raycast result
        var dir = other.transform.position - this.transform.position; // get the directional vector to the other object
        Physics.Raycast(this.transform.position, dir, out target); // draw ray between sensor and object
        if(target.transform == other.transform) // if there is no object between them
        {
            return true; // the sensor can see the object
        }
        else
        {
            return false; // the sensor cannot see the object
        }
    }

    
}
