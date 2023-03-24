using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoBehaviour
{
    public Robot robot;
    private float cd = .2f;
    private float timer = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && timer >= cd)
        {
            robot.isPaused = !robot.isPaused; // flip the toggle
            timer = 0f;
        }

        timer += Time.deltaTime;
    }
}
