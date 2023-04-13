using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicWindow : MonoBehaviour
{
    /*  
     *  The dynamic window defines the local search space for the robot
     *  It is constructed of a set of translational (x, y) and rotational (Θ) velocities
     *  It is then filtered in three ways:
     *  -- Only the velocities where the robot can still safely stop
     *  -- Only the velocities reachable by the next time step
     *  -- Only the velocities that avoid colliding with obstacles
     *  
     *  A velocity is then chosen from the remaining set by maximizing a fitness function that accounts for:
     *  -- Progress toward the goal
     *  -- Forwad velocity of the robot
     *  -- The closest obstacle to the current trajectory
     *
     */


    //public Sensor sensor; // ref to the robot's sensor
    public Rigidbody robot; // ref to the robot's dynamic rigidbody for physics stuff
    public GameObject p_obstacle; // obstacle dummy prefab
    public float timeStep;
    public float samples; // number of subdivisions of the area around the robot to sample velocities

    private float lastVelocity;
    private float lastRotation;
    private float timer;
    private List<Velocity> window;

    // Start is called before the first frame update
    void Start()
    {
        lastVelocity = robot.velocity.magnitude;
        lastRotation = robot.angularVelocity.y;
        timer = 0;
        window = new List<Velocity>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(timer >= timeStep) // every timestep
        {
            // clear the window
            window.Clear();
            // create a set of translational and rotational velocites
            float a = robot.velocity.magnitude - lastVelocity / Time.fixedDeltaTime; // calculate the current acceleration
            float theta = robot.angularVelocity.y; // get the rotational velocity around the Y axis

            for (int i = 0; i < samples; i++)
            {
                float velX = robot.velocity.magnitude + ((a) * timeStep); // calculate the next translational velocity X component
                float theta_a = theta - lastRotation / Time.fixedDeltaTime;
                float theta_next = theta + (theta_a * timeStep); // calculate the next rotational velocity by adding the acceleration over a single time step
                //window.Add(new Velocity())
            }


                timer = 0;
        }
        else
        {
            timer += Time.fixedDeltaTime;
        }
    }

    private Vector3 CalculatePosition(Velocity vel)
    {
        return new Vector3(ProjectXVelocity(this.transform.position.x, vel.nextVelocity, vel.nextTheta, vel.theta), 0f, ProjectZVelocity(this.transform.position.z, vel.nextVelocity, vel.nextTheta, vel.theta)); // return a vector3 with the X & Z component and zero'd Y
    }

    private float ProjectXVelocity(float c, float nv, float nt, float t)
    {
        return c + (CircularArcX(nv, nt) - CircularArcX(robot.velocity.magnitude, t)); // next X is a result of the the integral of V(tn) * COS(Θn) from the current time to the next timestep
    }

    private float ProjectZVelocity(float c, float nv, float nt, float t)
    {
        return c + (CircularArcZ(nv, nt) - CircularArcZ(robot.velocity.magnitude, t)); // next X is a result of the the integral of V(tn) * COS(Θn) from the current time to the next timestep
    }

    private float CircularArcX(float v, float r)
    {
        return (float) (Math.Pow((double)v, 2.0) * Math.Sin((double) r)) / 2;
    }

    private float CircularArcZ(float v, float r)
    {
        return (float) (-Math.Pow((double)v, 2.0) * Math.Cos((double) r)) / 2;
    }
}

class Velocity
{
    public float nextVelocity;
    public float theta;
    public float nextTheta;

    public Velocity(float t, float nv, float nt)
    {
        this.theta = t;
        this.nextVelocity = nv;
        this.nextTheta = nt;
    }
}
