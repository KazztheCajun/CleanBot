using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{

    public GameObject linePrefab;
    public List<SensorLine> Lines => lines;
    [SerializeField]
    private List<SensorLine> lines;
    private List<GameObject> preload;

    // Start is called before the first frame update
    void Start()
    {
        lines = new List<SensorLine>();
        preload = new List<GameObject>();
        Vector3 loc = new Vector3(1000, 1000, 1000);
        Transform parent = GameObject.Find("Preload").transform;
        for (int i = 0; i < 100; i++)
        {
            GameObject o = Instantiate(linePrefab, loc, Quaternion.identity);
            o.transform.parent = parent;
            preload.Add(o);
            o.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //lines.Sort();
        foreach(SensorLine l in lines)
        {
            l.noise = GenerateNoise(Vector3.Distance(this.transform.position, l.obj.transform.position)); // apply some new noise
            SetColor(l); // check if it can still be seen
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject other_obj = other.gameObject; // cache collided object
        GameObject temp = GetNextLine();
        if(temp != null)
        {
            temp.transform.parent = this.transform; // set this sensor as it's parent
            SensorLine l = temp.GetComponent<SensorLine>();
            l.sensor = this.transform; // set this sensor as it's sensor location
            l.obj = other_obj.transform; // set the object as it's obstacle location
            l.noise = GenerateNoise(Vector3.Distance(this.transform.position, other_obj.transform.position));
            lines.Add(l); // add it to the list of lines
            temp.SetActive(true); // ensure it is active in the scene
        }
        else
        {
            Debug.Log("[Sensor] Tried to get a cached line but got null instead");
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject other_obj = other.gameObject; // cache collided object
        SensorLine l = lines.Find(x => x.obj.gameObject.Equals(other_obj)); // find the line that has this object as an obstacle
        l.gameObject.SetActive(false); // deactivate it
        lines.Remove(l); // remove it from the list of lines
    }

    private Vector3 GenerateNoise(float dist)
    {
        // return a noise value based on the given distance
        // if the ditstance is below 1m, +-10mm noise
        if(dist < 1)
        {
            return new Vector3(Random.Range(-.01f, .01f), Random.Range(-.01f, .01f), Random.Range(-.01f, .01f));
        }
        // if the distance is greater, +- 1% of distance
        float o = dist / 100;
        return new Vector3(Random.Range(-o, o), Random.Range(-o, o), Random.Range(-o, o));
    }

    private void SetColor(SensorLine l)
    {
        if(CheckVisibility(l)) // if the robot can see the obstacle make the line green
        {
            l.color = Color.green;
        }
        else // otherwise make the line red
        {
            l.color = Color.red;
        }
        
    }

    private bool CheckVisibility(SensorLine line)
    {
        /*
         * Checks if the obstacle of a SensorLine is visible to the sensor
         * Basically draws a ray to the detected object and if there is nothing blocking that ray
         * then the sensor can see the object.
         * Green lines for objects that can be seen
         * Red lines for objects that are hidden 
         */

        RaycastHit target; // hold a spot for the raycast result
        var dir = line.obj.transform.position - this.transform.position; // get the directional vector to the line obstacle
        Physics.Raycast(this.transform.position, dir.normalized, out target); // draw ray between sensor and obstacle
        if(target.transform == line.obj.transform) // if there is no object between them
        {
            //Debug.Log($"{this.gameObject} can see {target.transform.gameObject}");
            return true; // the sensor can see the object
        }
        else
        {
            //Debug.Log($"{this.gameObject} is unable to see {target.transform.gameObject}");
            return false; // the sensor cannot see the object
        }
    }

    private void CreateNewLine(GameObject other_obj)
    {
        GameObject temp = GetNextLine();
        if(temp != null)
        {
            temp.transform.parent = this.transform; // set this sensor as it's parent
            SensorLine l = temp.GetComponent<SensorLine>();
            l.sensor = this.transform; // set this sensor as it's sensor location
            l.obj = other_obj.transform; // set the object as it's obstacle location
            l.noise = GenerateNoise(Vector3.Distance(this.transform.position, other_obj.transform.position));
            lines.Add(l); // add it to the list of lines
            temp.SetActive(true); // ensure it is active in the scene
        }
    }

    private GameObject GetNextLine()
    {
        for (int i = 0; i < preload.Count; i++) // for each object in the preload list
        {
            if(!preload[i].activeInHierarchy) // if it is not active
            {
                return preload[i];
            }
        }

        return null; // otherwise return null
    }
    
}
