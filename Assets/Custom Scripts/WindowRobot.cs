using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class WindowRobot : MonoBehaviour
{
    // public variables
    public enum RState {DOCKED, CLEANING, RESUPPLY}
    [SerializeField]
    private RState state;
    public List<Transform> spills;
    public Transform homeBase;
    public GameObject linePrefab;
    public Transform current;
    [Range(0.0f, 100.0f)]
    public float recharge;
    [Range(0.0f, 100.0f)]
    public float discharge;
    [Range(0.0f, 100.0f)]
    public float maxVelocity;
    [Range(0.0f, 100.0f)]
    public float turningRate;
    public float timeStep;
    public bool isPaused;
    public UnityEvent PickUpSpill;

    // private variables
    private float battery;
    private Rigidbody physics;
    private float lastVelocity;
    private float lastRotation;
    private float timer;
    private List<GameObject> lines;
    private List<GameObject> preload;

    // Start is called before the first frame update
    void Start()
    {
        this.state = RState.DOCKED;
        this.current = homeBase;
        this.transform.position = homeBase.position; //new Vector3(homeBase.position.x, 0.55f, homeBase.position.z);
        this.physics = this.gameObject.GetComponent<Rigidbody>();
        this.lastVelocity = physics.velocity.magnitude;
        this.lastRotation = physics.angularVelocity.magnitude;
        this.timer = 0f;
        this.preload = new List<GameObject>();
        this.lines = new List<GameObject>();
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
    void FixedUpdate()
    {
        if (!isPaused)
        {
            if(timer >= timeStep)
            {
                Debug.Log("Generating new dynamic window");
                ClearLines();
                List<Vector3> window = DynamicWindow.CreateDynamicWindow(this, GetCurrentVelocity());
                Debug.Log($"Window Size: {window.Count}");
                foreach (Vector3 v in window)
                {
                    Debug.Log($"Current location: ({this.transform.position.x}, {this.transform.position.z}) | Next Location: ({v.x}, {v.z})");
                    GameObject temp = GetNextLine();
                    Debug.Log(temp);
                    if (temp != null)
                    {
                        VelocityLine l = temp.GetComponent<VelocityLine>();
                        l.origin = this.transform;
                        l.target = v;
                        l.gameObject.SetActive(true);
                        lines.Add(temp);
                    }
                }
                timer = 0.1f;
            }
            else
            {
                timer += Time.fixedDeltaTime;
            }
            
            // do stuff based on current state
            switch (state)
            {
                case RState.DOCKED:
                    this.battery += recharge * Time.deltaTime;
                    WaitForSpill();
                break;
                case RState.CLEANING:
                    this.battery -= discharge * Time.deltaTime;
                    MoveToCurrent();
                    CleanSpill();
                    break;
                case RState.RESUPPLY:
                    this.battery -= discharge * Time.deltaTime;
                    NavigateHome();
                break;
            }
        }
        
    }

    public void ApplyVelocity() // apply a translational velocity to the robot
    {

    }

    public void ApplyTheta() // apply a rotational velocity to the robot
    {

    }

    public Velocity GetCurrentVelocity()
    {
        // get the current physics info
        float v = physics.velocity.magnitude;
        float theta = physics.angularVelocity.magnitude;
        float rot = this.transform.rotation.y;
        float linAcc = (v - lastVelocity) / this.timeStep;
        float rotAcc = (theta - lastRotation) / this.timeStep;
        Velocity vel = new Velocity(this.transform.position, rot, v, linAcc, theta, rotAcc, this.timeStep); // current velocity object
        return vel;
    }

    public void CleanSpill()
    {
        if(Vector3.Distance(this.transform.position, current.position) < 1)
        {
            Debug.Log("Cleaning up spill!");
            current.gameObject.SetActive(false); // clean up spills
            spills.Remove(current.gameObject.transform); // remove it from the list
            NextSpill();
            PickUpSpill.Invoke();
        }
    }

    private void NavigateHome()
    {
        MoveToCurrent();
        float dist = Vector3.Distance(this.transform.position, homeBase.position);
        if(dist < .1)
        {
            this.state = RState.DOCKED;
        }
    }


    private void WaitForSpill()
    {
        if(spills.Count > 0)
        {
            current = spills[0];
            this.state = RState.CLEANING;
        }
        else
        {
            current = homeBase;
        }
    }

    private void MoveToCurrent()
    {
        Vector3 v = new Vector3(current.position.x - this.transform.position.x, 0f, current.position.z - this.transform.position.z);
        this.GetComponent<Rigidbody>().MovePosition(this.transform.position + v.normalized * Time.deltaTime * maxVelocity);
        //this.transform.LookAt(current);
    }

    private void NextSpill()
    {
        if(spills.Count > 0) // check if more spills are in the list
        {
            current = spills[UnityEngine.Random.Range(0, spills.Count)]; // get next if so
        }
        else // otherwise return to home
        {
            current = homeBase;
            this.state = RState.RESUPPLY;
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

    private void ClearLines()
    {
        foreach(GameObject o in lines)
        {
            o.SetActive(false);
        }
        lines.Clear();
    }
}
