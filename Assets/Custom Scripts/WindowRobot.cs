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
    public bool showLines;
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
                List<Velocity> window = DynamicWindow.CreateDynamicWindow(this, GetCurrentVelocity()); // generate window based on current
                Velocity best = null;
                float fitness = float.MinValue;
                Debug.Log($"Finding best Velocity from {window.Count} options");
                foreach (Velocity v in window) // find the best velocity
                {
                    
                    if(showLines)
                    {
                        DrawLine(v); // draw a trajectory line
                    }
                    float f = CalculateFitness(v);
                    Debug.Log($"Velocity: {v.LinearVelocity} | Rotation: {v.RotationalVelocity} | Fitness: {f}");
                    if(f > fitness) // if this velocity is more fit, set it as best
                    {
                        Debug.Log($"New Best: {(best == null ? 0 : best.LinearVelocity)} --> {v.LinearVelocity} m/s | {(best == null ? 0 : best.RotationalVelocity)} --> {v.RotationalVelocity} rad/s | Fitness: {fitness} --> {f}");
                        best = v;
                        fitness = f;
                        
                    }
                }
                Debug.Log($"Best Velocity: {best.LinearVelocity} m/s | {best.RotationalVelocity} rad/s | Finess: {fitness}");
                lastVelocity = velocity;
                velocity = best;
                timer = 0.1f;
                
            }
            else
            {
                //Debug.Log("Awating a new spill");
                timer += Time.fixedDeltaTime;
            }
            
            // do stuff based on current state
            switch (state)
            {
                case RState.DOCKED:
                    this.battery += recharge * Time.fixedDeltaTime;
                    WaitForSpill();
                break;
                case RState.CLEANING:
                    this.battery -= discharge * Time.fixedDeltaTime;
                    //MoveToCurrent();
                    CleanSpill();
                    break;
                case RState.RESUPPLY:
                    this.battery -= discharge * Time.fixedDeltaTime;
                    IsHome();
                break;
            }

            goalLine.SetPositions(new Vector3[]{this.transform.position, goal.transform.position});
            MoveToTarget();
        }
        
    }

    private void MoveToTarget() // apply a translational velocity to the robot
    {
        targetSphere.transform.position = velocity.Destination; // set target location
        physics.angularVelocity = new Vector3(0f, velocity.RotationalVelocity, 0f); // directly apply the rotational velocity
        physics.velocity = new Vector3(0f, 0f, velocity.LinearVelocity); // directly apply the translational velocity
    }

    private float CalculateFitness(Velocity v)
    {
        float heading = headingParam * CalculateHeading(v);
        float clear = clearanceParam * CalculateClearance(v.Destination);
        float vel = velocityParam * ProjectVelocity(v);
        return alpha * (heading + clear + vel);
    }

    private void NormalizeVelocities()
    {
        
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
                    if(target.distance < min) // if this distance is shorter
                    {
                        min = target.distance; // set it as min
                    }
                }
            }
        }

        return min; // return the distance to the closest obstacle along the trajectory
    }

    private float ProjectVelocity(Velocity v)
    {
        return Mathf.Abs(v.NextLinearVelocity - v.LinearVelocity);
    }

    private float CalculateHeading(Velocity v)
    {
        Vector3 goalLoc = new Vector3(goal.position.x, .55f, goal.position.y);
        Vector3 targetVector = goalLoc - v.Destination; // vector from goal to destinaton location
        Vector3 headingVector = this.transform.forward + new Vector3(0f, v.RotationalAcceleration, 0f);
        Quaternion target = Quaternion.LookRotation(targetVector);
        Quaternion heading = Quaternion.LookRotation(headingVector);

        float result = 180 - Quaternion.Angle(target, heading);
        Debug.Log(result);
        return result; // return the angle between the target vector and the projected rotation
    }

    public Velocity GetCurrentVelocity()
    {
        // get the current physics info
        float v = transform.InverseTransformDirection(physics.velocity).z; // get a signed forward velocity
        float theta = transform.InverseTransformDirection(physics.angularVelocity).y; // get the rotational velocity
        float rot = this.transform.eulerAngles.y * Mathf.Deg2Rad;
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
