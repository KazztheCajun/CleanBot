using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicWindow
{
    /*  
     *  The dynamic window defines the local search space for the robot
     *  It is constructed of a set of translational (x, y) and rotational (Θ) velocities
     *  It is then filtered in three ways:
     *  -- Only the velocities where the robot can still safely stop | provided by the robot
     *  -- Only the velocities reachable by the next time step | provided by CreateDynamicWindow
     *  -- Only the velocities that avoid colliding with obstacles | use the list provided above to collision check
     *  
     *  A velocity is then chosen from the remaining set by maximizing a fitness function that accounts for:
     *  -- Progress toward the goal
     *  -- Forwad velocity of the robot
     *  -- The closest obstacle to the current trajectory
     *
     */

    public static List<Velocity> CreateDynamicWindow(WindowRobot robot, Velocity current)
    {
        List<Velocity> list = new List<Velocity>();
        
        // Robot spec window
        float[] spec = { -robot.maxVelocity, // maximum reverse linear velocity
                         robot.maxVelocity, // maximum forward linear velocity
                         -robot.turningRate,
                         robot.turningRate };
        //Debug.Log($"Robot Spec Window: Velocity Range: {spec[0]} -> {spec[1]} | Rotation Range {spec[2]} -> {spec[3]}");

        // current motion window
        float[] mot = { current.LinearVelocity - (current.LinearAcceleration * current.TimeStep),
                        current.LinearVelocity + (current.LinearAcceleration * current.TimeStep),
                        current.RotationalVelocity - (current.RotationalAcceleration * current.TimeStep),
                        current.RotationalVelocity + (current.RotationalAcceleration * current.TimeStep) };
       //Debug.Log($"Current Motion Window: Velocity Range: {mot[0]} -> {mot[1]} | Rotation Range {mot[2]} -> {mot[3]}");

        float[] dw = { Mathf.Max(spec[0], mot[0]), Mathf.Max(spec[1], mot[1]), Mathf.Max(spec[2], mot[2]), Mathf.Max(spec[3], mot[3]) };
        // if(dw[0] == 0 && dw[1] == 0 && dw[2] == 0 && dw[3] == 0)
        // {
        //     dw = spec;
        // }
        //Debug.Log($"Selected Velocity Range: {spec[0]} -> {spec[1]} | Rotation Range {spec[2]} -> {spec[3]}");
        //float lambda = .00000001f;
        for (float x = spec[0]; x <= spec[1]; x += .5f)
        {
            for (float y = spec[2]; y <= spec[3]; y += 1f)
            {
                Velocity temp = new Velocity(current.Location, current.Rotation, x, (x - current.LinearVelocity) / current.TimeStep , y, (y - current.RotationalVelocity) / current.TimeStep, current.TimeStep);
                //Velocity temp2 = new Velocity(current.Location, current.Rotation, -x, (x - current.LinearVelocity) ,-y, (y - current.RotationalVelocity) , current.TimeStep);
                //Velocity temp3 = new Velocity(current.Location, current.Rotation, x, (x - current.LinearVelocity) ,-y, (y - current.RotationalVelocity) , current.TimeStep);
                //Velocity temp4 = new Velocity(current.Location, current.Rotation, -x, (x - current.LinearVelocity) , y, (y - current.RotationalVelocity) , current.TimeStep);
                //Debug.Log($"Generated Velocity: ({temp.LinearVelocity}, {temp.RotationalVelocity})");
                temp.Destination = CalculatePosition(temp);
                //temp2.Destination = CalculatePosition(temp2);
                //temp3.Destination = CalculatePosition(temp3);
                //temp4.Destination = CalculatePosition(temp4);

                list.Add(temp);
                //list.Add(temp2);
                //list.Add(temp3);
                //list.Add(temp4);
            }
        }
        return list;
    }

    public static void SetDestination(ref Velocity v)
    {
        v.Destination = CalculatePosition(v);
    }

    private static Vector3 CalculatePosition(Velocity vel)
    {
        return new Vector3(ProjectXVelocity(vel), .55f, ProjectZVelocity(vel)); // return a vector3 with the X & Z component and zero'd Y
    }

    private static float ProjectXVelocity(Velocity v)
    {
        return v.Location.x + CircularArcX(v); // next X is a result of the the integral of V(tn) * COS(Θn) from the current time to the next timestep
    }

    private static float ProjectZVelocity(Velocity v)
    {
        return v.Location.z + CircularArcZ(v); // next Z is a result of the the integral of V(tn) * SIN(Θn) from the current time to the next timestep
    }

    private static float CircularArcX(Velocity v)
    {
        if(v.RotationalVelocity == 0)
        {
           return v.LinearVelocity * Mathf.Cos(v.NextRotation) * v.TimeStep;
        }
        else
        {
           return (v.LinearVelocity / v.RotationalVelocity) * (Mathf.Sin(v.Rotation) - Mathf.Sin(v.NextRotation));
        }
        
    }

    private static float CircularArcZ(Velocity v)
    {
        if(v.RotationalVelocity == 0)
        {
            return v.LinearVelocity * Mathf.Sin(v.NextRotation) * v.TimeStep;
        }
        else
        {
            return -(v.LinearVelocity / v.RotationalVelocity) * (Mathf.Cos(v.Rotation) - Mathf.Cos(v.NextRotation));
        }
    }
}


