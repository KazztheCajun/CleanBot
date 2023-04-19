using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Velocity
{
    private Vector3 location; // the origin of this velocity
    private float velocity; // the current translational velocity
    private float nextVelocity; // the next trans velocity given an acceleration
    private float theta; // the current roational velocity
    private float nextTheta; // the next rot velocity given an acc
    private float lineAcc; // translationsal acceleration
    private float rotAcc; // rotational acceleration
    private float time; // the amount of time this velocity occurs over

    public Vector3 Location => location;
    public float LinearVelocity => velocity;
    public float NextLinearVelocity => nextVelocity;
    public float RotationalVelocity => theta;
    public float NextRotationalVelocity => nextTheta;
    public float TimeStep => time;


    public Velocity(Vector3 l, float v, float la, float th, float ra, float ti)
    {
        this.location = l;
        this.velocity = v;
        this.theta = th;
        this.lineAcc = la;
        this.rotAcc = ra;
        this.time = ti;
        this.nextVelocity = v + (lineAcc * time);
        this.nextTheta = th + (rotAcc * time);
    }
}