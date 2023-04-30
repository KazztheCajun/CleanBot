using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityLine : MonoBehaviour 
{

    public Vector3 target;
    public Transform origin;
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
        if(target != null && origin != null)
        {
            this.line.SetPosition(0, origin.position);
            this.line.SetPosition(1, target);
            this.line.startColor = color;
            this.line.endColor = color;
        }
    }
}
