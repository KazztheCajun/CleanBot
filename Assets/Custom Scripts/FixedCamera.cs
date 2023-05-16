using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCamera : MonoBehaviour
{
    
    
    // have a called method that is alerted to a new spill beng spawned
    // orient eye orientation to point at 

    public static FixedCamera instance;
    public Camera eyeCam;

    

    public Color color;
    public bool DrawLineToLastSpill;
    private LineRenderer line;

    public Transform currentSpill, lineStart;
     public float speed = 2.5f;

    private void Awake() {
        instance = this;
    }

    void Update(){
        if(currentSpill == null){
            return;
        }
        Vector3 dir = currentSpill.position - transform.position;
        // dir.y = 0; // keep the direction strictly horizontal
        Quaternion rot = Quaternion.LookRotation(dir);
        // slerp to the desired rotation over time
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, speed * Time.deltaTime);
    }   
    
    public void SeeSpill(Transform spill){
        // print(" Looking at new spill ");
        currentSpill = spill;
        // transform.LookAt(spill);
        if(DrawLineToLastSpill)
            DrawLine(spill);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        this.line = GetComponent<LineRenderer>();
        this.line.positionCount = 2;
        this.line.widthMultiplier = .1f;
    }

    void DrawLine(Transform spill){
        this.line.SetPosition(0, lineStart.position);
        this.line.SetPosition(1, spill.position);
        this.line.startColor = color;
        this.line.endColor = color;
    }




    
}
