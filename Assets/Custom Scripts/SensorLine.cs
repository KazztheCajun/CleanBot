using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorLine : MonoBehaviour, IComparable<SensorLine>, IEquatable<SensorLine>
{

    public Transform obj;
    public Vector3 noise;
    public Transform sensor;
    public Color color;
    public bool isBlocked;
    public float Distance => Vector3.Distance(obj.position, sensor.position);
    public Vector3 ObstacleVector => obj.position - sensor.position;
    private LineRenderer line;
    
    // Start is called before the first frame update
    void Start()
    {
        this.line = GetComponent<LineRenderer>();
        this.line.positionCount = 2;
        this.line.widthMultiplier = .1f;
    }

    // Update is called once per frame
    void Update()
    {
        if(obj != null && sensor != null)
        {
            this.line.SetPosition(0, sensor.position);
            this.line.SetPosition(1, obj.position + noise);
            this.line.startColor = color;
            this.line.endColor = color;
        }
    }

    public int CompareTo(SensorLine other)
    {
        if(other == null) return 1;

        return this.Distance.CompareTo(other.Distance);
    }

    public bool Equals(SensorLine other)
    {
        return this.gameObject.Equals(other.gameObject);
    }
}
