using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Velocity : IComparable<Velocity>
{
    private Vector3 location; // the origin of this velocity
    private Vector3 destination;
    private float rotation; // current rotation of origin
    private float velocity; // the current translational velocity
    private float theta; // the current roational velocity
    private float lineAcc; // translationsal acceleration
    private float rotAcc; // rotational acceleration
    private float time; // the amount of time this velocity occurs over
    private float fitness; // how good of a choice this velocity is

    public float Fitness { get {return fitness;}
                           set {fitness = value;} }
    public Vector3 Location { get {return location;}
                              set {location = value;} }
    public Vector3 Destination { get {return destination;}
                              set {destination = value;} }
    public float Rotation { get {return rotation;}
                            set {rotation = value;} }
    public float NextRotation => rotation + (theta * time);
    public float LinearVelocity => velocity;
    public float NextLinearVelocity => velocity + (lineAcc * time);
    public float RotationalVelocity => theta;
    public float NextRotationalVelocity => theta + (rotAcc * time);
    public float LinearAcceleration => lineAcc;
    public float RotationalAcceleration => rotAcc;
    public float TimeStep => time;


    public Velocity(Vector3 l, float r, float v, float la, float th, float ra, float ti)
    {
        this.location = l;
        this.rotation = r;
        this.velocity = v;
        this.theta = th;
        this.lineAcc = la;
        this.rotAcc = ra;
        this.time = ti;
    }

    public Velocity(Vector3 l, float r, float v, float t, float ti)
    {
        this.location = l;
        this.rotation = r;
        this.velocity = v;
        this.theta = t;
        this.time = ti;
    }

    public int CompareTo(Velocity other)
    {
        if(other == null)
        {
            return 1;
        }

        return -this.fitness.CompareTo(other.Fitness); // negate the default float CompareTo() sort largest to smallest
    }
}