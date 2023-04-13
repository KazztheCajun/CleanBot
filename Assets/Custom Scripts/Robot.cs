using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Robot : MonoBehaviour
{
    // public variables
    public enum RState {DOCKED, CLEANING, RESUPPLY}
    [SerializeField]
    private RState state;
    public List<Transform> spills;
    public Transform homeBase;
    public Transform current;
    [Range(0.0f, 100.0f)]
    public float recharge;
    [Range(0.0f, 100.0f)]
    public float discharge;
    [Range(0.0f, 100.0f)]
    public float speed;
    //public UnityEvent PickUpSpill;
    public bool isPaused;

    // private variables
    private float battery;

    // Start is called before the first frame update
    void Start()
    {
        this.state = RState.DOCKED;
        this.current = homeBase;
        this.transform.position = homeBase.position; //new Vector3(homeBase.position.x, 0.55f, homeBase.position.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isPaused)
        {
            // do stuff based on current state
            switch (state)
            {
                case RState.DOCKED:
                    this.battery += recharge * Time.deltaTime;
                    WaitForSpill();
                break;
                case RState.CLEANING:
                    this.battery -= discharge * Time.deltaTime;
                    MoveToCurrent();
                    CleanSpill();
                    break;
                case RState.RESUPPLY:
                    this.battery -= discharge * Time.deltaTime;
                    NavigateHome();
                break;
            }
        }
        
    }

    public void CleanSpill()
    {
        if(Vector3.Distance(this.transform.position, current.position) < 1)
        {
            Debug.Log("Cleaning up spill!");
            Destroy(current.gameObject); // clean up spills
            spills.Remove(current.gameObject.transform); // remove it from the list
            NextSpill();
        }
    }

    private void NavigateHome()
    {
        MoveToCurrent();
        float dist = Vector3.Distance(this.transform.position, homeBase.position);
        if(dist < 1)
        {
            this.state = RState.DOCKED;
        }
    }


    private void WaitForSpill()
    {
        if(spills.Count > 0)
        {
            current = spills[0];
            this.state = RState.CLEANING;
        }
        else
        {
            current = homeBase;
        }
    }

    private void MoveToCurrent()
    {
        Vector3 v = new Vector3(current.position.x - this.transform.position.x, 0f, current.position.z - this.transform.position.z);
        this.GetComponent<Rigidbody>().MovePosition(this.transform.position + v.normalized * Time.deltaTime * speed);
        //this.transform.LookAt(current);
    }

    private void NextSpill()
    {
        if(spills.Count > 0) // check if more spills are in the list
        {
            current = spills[Random.Range(0, spills.Count)]; // get next if so
        }
        else // otherwise return to home
        {
            current = homeBase;
            this.state = RState.RESUPPLY;
        }
    }
}
