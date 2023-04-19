using System;
using System.Collections;
using System.Collections.Generic;
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

    public static List<Vector3> CreateDynamicWindow(List<Velocity> vels) // given a list of safe velocities
    {
        List<Vector3> list = new List<Vector3>();
        foreach(Velocity v in vels)
        {
            list.Add(CalculatePosition(v)); // generate a list of positions
        }
        return list;
    }

    private static Vector3 CalculatePosition(Velocity vel)
    {
        return new Vector3(ProjectXVelocity(vel), 0f, ProjectZVelocity(vel)); // return a vector3 with the X & Z component and zero'd Y
    }

    private static float ProjectXVelocity(Velocity v)
    {
        return v.Location.x + (CircularArcX(v.NextLinearVelocity, v.NextRotationalVelocity) - CircularArcX(v.LinearVelocity, v.RotationalVelocity)); // next X is a result of the the integral of V(tn) * COS(Θn) from the current time to the next timestep
    }

    private static float ProjectZVelocity(Velocity v)
    {
        return v.Location.z + (CircularArcZ(v.NextLinearVelocity, v.NextRotationalVelocity) - CircularArcZ(v.LinearVelocity, v.RotationalVelocity)); // next X is a result of the the integral of V(tn) * COS(Θn) from the current time to the next timestep
    }

    private static float CircularArcX(float v, float r)
    {
        return (float) (Math.Pow((double)v, 2.0) * Math.Sin((double) r)) / 2;
    }

    private static float CircularArcZ(float v, float r)
    {
        return (float) (-Math.Pow((double)v, 2.0) * Math.Cos((double) r)) / 2;
    }
}


