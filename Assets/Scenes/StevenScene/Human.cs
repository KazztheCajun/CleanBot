using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Human : MonoBehaviour
{
    
    public Camera cam;

    public NavMeshAgent agent;

    public Transform GoalPoint;
    public Vector3 exitGoalPoint;

    bool started = false;

    public bool sitting = false;

    public CapsuleCollider collider;

    public Rigidbody rb;

    public NavMeshAgent nmAgent;

    public float timer;
    public float timerLimit;
    public bool sittingTimer;

    public bool finishedSitting;

    public Vector3 oldPos;

    public GameObject spill;

    public float MaxRangeOdds, OddsCutoff;

    public Animator anim;

    private void Awake() {
        collider = this.GetComponent<CapsuleCollider>();   
        rb = this.GetComponent<Rigidbody>();    
        nmAgent = this.GetComponent<NavMeshAgent>();   
    }

    private void Start() {
        HumanController.instance.AddHuman(this);
    }

    public void BeginHuman() {
        if(!this.gameObject.activeSelf){
            HumanController.instance.MoveThisHumanToCorrectStartingLocation(this.gameObject);
            this.gameObject.SetActive(true);
        }
        if(anim != null){
            anim.SetBool( "isWalking", true);
            anim.SetBool( "isSitting", false);
        }
        started = true;
        GetNewGoalPoint();
        GetNewExitGoalPoint();
        agent.SetDestination( GoalPoint.position );
    }

    private void Update() {
        
        CheckToSpawnSpill();

        if(sittingTimer){
            timer += Time.deltaTime;
            if(timer >= timerLimit){
                // break out of sitting 
                sittingTimer = false;
                sitting = false;
                finishedSitting = true;
                StopSitting();
                agent.SetDestination( exitGoalPoint );
            }
        }

        if(!finishedSitting){

        


            if(!started)
                return;

            //             if( GoalPoint.x <= 0.1f && GoalPoint.y <= 0.1f && GoalPoint.z <= 0.1f){

            if( GoalPoint.position == null){
                GetNewGoalPoint();
                GetNewExitGoalPoint();
                
                // print(" running 5 ");
                agent.SetDestination( GoalPoint.position );


            } else if( Vector3.Distance (this.transform.position, GoalPoint.position) < 1.8f ){
                if(!sitting)
                    SitAtChair();
                return;

            }

        } else {
            // print(" running 4 ");
            // if(exitGoalPoint != exitGoalPoint)
    
            // if( exitGoalPoint == null){
            //     print(" running 7 ");
            //     agent.SetDestination( exitGoalPoint );
            //     print(" running 8 ");


            // } else 
            if( Vector3.Distance (this.transform.position, exitGoalPoint) < 1.8f ){
                Deactivate();
                return;

            }

            print(agent.destination);


        }



    }

    void GetNewGoalPoint(){
        // print(" running ");
        GoalPoint = RandomSpawner.instance.ReturnRandomPointOnMap();
        // print(" running 3");
    }
    void GetNewExitGoalPoint(){
        // print(" running 2 ");
        exitGoalPoint = RandomSpawner.instance.ReturnExitPointOnMap();
    }

    void SitAtChair(){
        // disable collider
        // move to exact trasnform location

        if(anim != null){
            anim.SetBool( "isWalking", false);
            anim.SetBool( "isSitting", true);
        }


        
        nmAgent.enabled = false;
        collider.enabled = false;
        rb.detectCollisions = false;

        oldPos = this.transform.position;

        this.transform.position = new Vector3( GoalPoint.position.x, 2f,  GoalPoint.position.z) ;
        this.transform.rotation = GoalPoint.rotation;

        this.transform.parent = GoalPoint;
        this.transform.localPosition = new Vector3( 0, 1.12f, 0.62f ) ;




        sitting = true;

        timerLimit = Random.Range(10f, 20f);
        sittingTimer = true;
        //print( rb.detectCollisions );





    }

    void StopSitting(){
        
        // RandomSpawner.instance.ChairOpensUp(GoalPoint);

        print(" stopping sitting ");
        this.transform.parent = null;
        this.transform.position = new Vector3( oldPos.x, oldPos.y,  oldPos.z) ;

        
        if(anim != null){
            anim.SetBool( "isWalking", true);
            anim.SetBool( "isSitting", false);
        }

        collider.enabled = true;
        rb.detectCollisions = true;
        nmAgent.enabled = true;

        
        RandomSpawner.instance.ChairOpensUp(GoalPoint);



    }



    public void ResetEverything(){
        GoalPoint = null;
        exitGoalPoint = new Vector3(0f,0f,0f);
        started = false;
        sitting = false;
        timer = 0f;
        timerLimit = 0f;
        sittingTimer = false;
        finishedSitting = false;






    }

    void Deactivate(){
        // reset everything
        // add yourself back to appropriate human controller list
        HumanController.instance.ThisHumanIsDone(this);
        this.gameObject.SetActive(false);

    }

    
    public void SpawnSpill(){
        Instantiate(spill, new Vector3(this.transform.position.x,this.transform.position.y, this.transform.position.z ), Quaternion.identity);   
    }

    void CheckToSpawnSpill(){
        // run odds, if pass spawn a spill
        if(!started)
            return;

        if(Random.Range(0f,MaxRangeOdds) <= OddsCutoff){
            SpawnSpill();
        }


    }





}
