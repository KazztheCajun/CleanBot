using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{

    public int SeatsAvailable;
    
    public Transform Chair1, Chair2, Chair3, Chair4;


    public bool occupiedChair1, occupiedChair2, occupiedChair3, occupiedChair4;


    public bool OpenChair{
        get {
            if (!occupiedChair1 || !occupiedChair2 || !occupiedChair3 || !occupiedChair4){
                return true;
            } else {
                return false;
            }
        }
    }

    private void Start() {
        RandomSpawner.instance.AddThisChair(Chair1);
        RandomSpawner.instance.AddThisChair(Chair2);
        RandomSpawner.instance.AddThisChair(Chair3);
        RandomSpawner.instance.AddThisChair(Chair4);
        
    }



}
