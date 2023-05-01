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
    public Transform goal;
    public Velocity velocity;
    [Range(0.0f, 100.0f)]
    public float recharge;
    [Range(0.0f, 100.0f)]
    public float discharge;
    [Range(0.0f, 100.0f)]
    public float maxVelocity;
    [Range(0.0f, 5f)]
    public float alpha;
    [Range(0.0f, 100.0f)]
    public float turningRate;
    [Range(0.0f, 4.0f)]
    public float headingParam;
    [Range(0.0f, 1.0f)]
    public float velocityParam;
    [Range(0.0f, 1.0f)]
    public float clearanceParam;
    public float timeStep;
    public bool isPaused;
    public UnityEvent PickUpSpill;

    // private variables
    private float battery;
    private Rigidbody physics;
    private Sensor sensor;
    private Velocity lastVelocity;
    private float timer;
    private List<GameObject> lines;
    private List<GameObject> preload;
    private Transform lineParent;
    private Transform preloadParent;
    private GameObject targetSphere;
    private LineRenderer goalLine;

    // Start is called before the first frame update
    void Start()
    {
        this.state = RState.DOCKED;
        this.goal = homeBase;
        this.transform.position = homeBase.position; //new Vector3(homeBase.position.x, 0.55f, homeBase.position.z);
        this.physics = this.gameObject.GetComponent<Rigidbody>();
        this.sensor = GameObject.Find("AreaSensor").GetComponent<Sensor>();
        this.timer = 0f;
        this.preload = new List<GameObject>();
        this.lines = new List<GameObject>();
        this.lastVelocity = new Velocity(this.transform.position, this.transform.rotation.y, 0, 0, 0, 0, timeStep);
        this.velocity = GetCurrentVelocity();
        DynamicWindow.SetDestination(ref velocity);
        this.targetSphere = GameObject.Find("Target");
        this.targetSphere.transform.position = velocity.Destination;
        this.goalLine = this.GetComponent<LineRenderer>();
        this.goalLine.widthMultiplier = .1f;
        Vector3 loc = new Vector3(1000, 1000, 1000);
        lineParent = GameObject.Find("LineParent").transform;
        preloadParent = GameObject.Find("Preload").transform;
        for (int i = 0; i < 1000; i++)
        {
            GameObject o = Instantiate(linePrefab, loc, Quaternion.identity);
            o.transform.parent = preloadParent;
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
                Velocity best = GetCurrentVelocity(); // assume the current velocity is the best
                DynamicWindow.SetDestination(ref best); // set it's destination value
                float fitness = CalculateFitness(best);
                List<Velocity> window = DynamicWindow.CreateDynamicWindow(this, best); // generate window based on current
                Debug.Log($"Finding best Velocity from {window.Count} options");
                foreach (Velocity v in window) // find the best velocity
                {
                    //Debug.Log($"Current location: ({this.transform.position.x}, {this.transform.position.z}) | Next Location: ({v.x}, {v.z})");
                    //DrawLine(v); // draw a trajectory line
                    float f = CalculateFitness(v);
                    if(f > fitness) // if this velocity is more fit, set it as best
                    {
                        best = v;
                        fitness = f;
                    }
                }
                Debug.Log($"Best Velocity: {best.LinearAcceleration} m/s | {best.RotationalVelocity} rad/s");
                lastVelocity = velocity;
                velocity = best;
                timer = 0.1f;
            }
            else
            {
                Debug.Log("Awating a new spill");
                timer += Time.deltaTime;
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
                    //MoveToCurrent();
                    CleanSpill();
                    break;
                case RState.RESUPPLY:
                    this.battery -= discharge * Time.deltaTime;
                    IsHome();
                break;
            }

            goalLine.SetPositions(new Vector3[]{this.transform.position, goal.transform.position});
            MoveToTarget();
        }
        
    }

    private void MoveToTarget() // apply a translational velocity to the robot
    {
        // (velocity.Destination - this.transform.position).normalized
        // this.transform.forward
        targetSphere.transform.position = velocity.Destination;
        this.transform.Rotate( new Vector3(0, velocity.RotationalAcceleration * Time.fixedDeltaTime, 0));
        physics.MovePosition(this.transform.position + -this.transform.forward * Time.fixedDeltaTime * velocity.LinearVelocity);
        
    }

    private float CalculateFitness(Velocity v)
    {
        float heading = headingParam * CalculateHeading(v);
        float clear = clearanceParam * CalculateClearance(v.Destination);
        float vel = velocityParam * (ProjectVelocity(v) / maxVelocity);
        return alpha * (heading + clear + vel);
    }

    private float CalculateClearance(Vector3 t)
    {
        float min = 1; // set min to large value

        foreach(SensorLine l in sensor.Lines)
        {
            if(l.color == Color.green) // if the robot can see the obstacle
            {
                RaycastHit target; // hold a spot for the raycast result
                Physics.Raycast(this.transform.position, t.normalized, out target, t.magnitude); // raycast along the trajectory t
                if(target.collider != null) // if the raycast hits something
                {
                    if(target.distance / 5 < min) // if this normalized distance is shorter
                    {
                        min = target.distance / 5; // set it's distance as closest
                    }
                }
            }
        }

        return min; // return the distance to the closest obstacle along the trajectory
    }

    private float ProjectVelocity(Velocity v)
    {
        return maxVelocity - v.LinearVelocity; // return the normalized velocity projection
    }

    private float CalculateHeading(Velocity v)
    {
        Vector3 targetVector = goal.position - v.Destination;
        return Vector3.Angle(v.Destination - v.Location, targetVector) / 180; // return the normalized angle between the target vector and the projected rotation
    }

    public Velocity GetCurrentVelocity()
    {
        // get the current physics info
        float v = physics.velocity.magnitude;
        float theta = physics.angularVelocity.magnitude;
        float rot = this.transform.rotation.y;
        float linAcc = (v - lastVelocity.LinearVelocity) / this.timeStep;
        float rotAcc = (theta - lastVelocity.Rotation) / this.timeStep;
        Velocity vel = new Velocity(this.transform.position, rot, v, linAcc, theta, rotAcc, this.timeStep); // current velocity object
        return vel;
    }

    private void DrawLine(Velocity v)
    {
        GameObject temp = GetNextLine();
        //Debug.Log(temp);
        if (temp != null)
        {
            VelocityLine l = temp.GetComponent<VelocityLine>();
            l.origin = this.transform;
            l.target = v.Destination;
            l.gameObject.SetActive(true);
            l.gameObject.transform.parent = lineParent;
            lines.Add(temp);
        }
    }

    public void CleanSpill()
    {
        if(Vector3.Distance(this.transform.position, goal.position) < 1)
        {
            Debug.Log("Cleaning up spill!");
            goal.gameObject.SetActive(false); // clean up spills
            spills.Remove(goal.gameObject.transform); // remove it from the list
            NextSpill();
            PickUpSpill.Invoke();
        }
    }

    private void IsHome()
    {
        //MoveToCurrent();
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
            goal = spills[0];
            this.state = RState.CLEANING;
        }
        else
        {
            goal = homeBase;
        }
    }

    // private void MoveToCurrent()
    // {
    //     Vector3 v = new Vector3(current.position.x - this.transform.position.x, 0f, current.position.z - this.transform.position.z);
    //     this.GetComponent<Rigidbody>().MovePosition(this.transform.position + v.normalized * Time.deltaTime * maxVelocity);
    //     //this.transform.LookAt(current);
    // }

    private void NextSpill()
    {
        if(spills.Count > 0) // check if more spills are in the list
        {
            goal = spills[UnityEngine.Random.Range(0, spills.Count)]; // get next if so
        }
        else // otherwise return to home
        {
            goal = homeBase;
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
            o.transform.parent = preloadParent;
        }
        lines.Clear();
    }
}
