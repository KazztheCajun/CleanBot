using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorLine : MonoBehaviour 
{

    public Transform obj;
    public Transform sensor;
    public Color color;
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
            this.line.SetPosition(1, obj.position);
            this.line.startColor = color;
            this.line.endColor = color;
        }
    }
}
