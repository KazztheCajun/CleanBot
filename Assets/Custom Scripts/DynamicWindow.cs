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


    public Sensor sensor; // ref to the robot's sensor
    public Rigidbody robot; // ref to the robot's dynamic rigidbody for physics stuff
    public GameObject p_obstacle; // obstacle dummy prefab
    public float timeStep;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void fitness() // evaluates the fitness of
    {

    }
}
