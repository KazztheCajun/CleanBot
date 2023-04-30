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

    public static List<Vector3> CreateDynamicWindow(WindowRobot robot, Velocity current) // given a list of safe velocities
    {
        List<Vector3> list = new List<Vector3>();
        
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

        float[] dw = { Mathf.Max(spec[0], mot[0]), Mathf.Min(spec[1], mot[1]), Mathf.Max(spec[2], mot[2]), Mathf.Min(spec[3], mot[3]) };
        //Debug.Log($"Selected Velocity Range: {dw[0]} -> {dw[1]} | Rotation Range {dw[2]} -> {dw[3]}");

        //float lambda = .00000001f;
        for (float x = dw[0]; x <= dw[1]; x += .1f)
        {
            for (float y = dw[2]; y <= dw[3]; y += .1f)
            {
                Velocity temp = new Velocity(current.Location, current.Rotation, x, y, current.TimeStep);
                Debug.Log($"Generated Velocity: ({x}, {y})");
                list.Add(CalculatePosition(temp));
            }
        }
        return list;
    }

    private static Vector3 CalculatePosition(Velocity vel)
    {
        return new Vector3(ProjectXVelocity(vel), .55f, ProjectZVelocity(vel)); // return a vector3 with the X & Z component and zero'd Y
    }

    private static float ProjectXVelocity(Velocity v)
    {
        return v.Location.x + CircularArcX(v.LinearVelocity, v.NextRotation); // next X is a result of the the integral of V(tn) * COS(Θn) from the current time to the next timestep
    }

    private static float ProjectZVelocity(Velocity v)
    {
        return v.Location.z + CircularArcZ(v.LinearVelocity, v.NextRotation); // next X is a result of the the integral of V(tn) * COS(Θn) from the current time to the next timestep
    }

    private static float CircularArcX(float v, float r)
    {
        return v * Mathf.Cos(r);
    }

    private static float CircularArcZ(float v, float r)
    {
        return v * Mathf.Sin(r);
    }
}


