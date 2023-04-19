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
    public float speed;
    [Range(0.0f, 15.0f)]
    public float maxPower;
    [Range(0.0f, 6000.0f)]
    public float rpm;
    [Range(0.0f, 1.0f)]
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
        this.timer = 0;
        this.preload = new List<GameObject>();
        this.lines = new List<GameObject>();
        Vector3 loc = new Vector3(1000, 1000, 1000);
        for (int i = 0; i < 100; i++)
        {
            GameObject o = Instantiate(linePrefab, loc, Quaternion.identity);
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
                List<Velocity> list = GenerateVelocities();
                foreach(Velocity v in list)
                {
                    Debug.Log($"Translational: {v.LinearVelocity} m/s | Rotational: {v.RotationalVelocity} Θ/s | Next T: {v.NextLinearVelocity} m/s | Next R {v.NextRotationalVelocity}");
                }
                
                List<Vector3> window = DynamicWindow.CreateDynamicWindow(list);
                foreach(Vector3 v in window)
                {
                    GameObject temp = GetNextLine();
                    if(temp != null)
                    {
                        temp.transform.position = v;
                        temp.transform.parent = this.transform;
                        SensorLine l = temp.GetComponent<SensorLine>();
                        l.sensor = this.transform;
                        l.noise = Vector3.zero;
                        lines.Add(temp);
                    }
                }
                timer = 0;
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

    public List<Velocity> GenerateVelocities()
    {
        List<Velocity> list = new List<Velocity>();
        // get the current physics info
        float v = physics.velocity.magnitude;
        float theta = physics.angularVelocity.magnitude;
        float linAcc = (v - lastVelocity) / this.timeStep;
        float rotAcc = (theta - lastRotation) / this.timeStep;

        for (int i = 0; i < 10; i++)
        {
            // generate a velocity object that incriments the linear and rotational acceleration by 10%
            Velocity inc = new Velocity(this.transform.position, v, linAcc + (i * (linAcc * .1f)), theta, rotAcc + (i * .1f), this.timeStep);
            // check if the velocity will exceed the maximum acceleration
            if(inc.NextLinearVelocity >= CalculateAcceleration(maxPower))
            {
                list.Add(inc); // if not add it to the list
            }
            // generate a velocity object that decriments the linear and rotational acceleration by 10%
            Velocity dec = new Velocity(this.transform.position, v, linAcc - (i * (linAcc * .1f)), theta, rotAcc - (i * .1f), this.timeStep);
            // check if the velocity will exceed the maximum acceleration
            if(dec.NextLinearVelocity >= CalculateAcceleration(maxPower))
            {
                list.Add(dec); // if not add it to the list
            }
        }
        return list;
    }

    public float CalculateAcceleration(float torque)
    {
        return torque / physics.inertiaTensor.magnitude;
    }

    public float CalculateTorque(float power)
    {
        return (float) (60 * (power / (2 * Math.PI * rpm))); // given a power, calculate the given torque generated
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
        this.GetComponent<Rigidbody>().MovePosition(this.transform.position + v.normalized * Time.deltaTime * speed);
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
