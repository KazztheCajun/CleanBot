using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceRefiller : MonoBehaviour
{
    
    
    // register robot entering
    // once entering, start refilling it in update
    // once full, stop and let it leave

    public bool RobotInside;

    public float TimeFactorForRefilling;
    public ResourceManager rm;

    private void Update() {
        if(RobotInside){
            TryToFillResourceGuage();
        }
    }

    void TryToFillResourceGuage(){
        try{
            rm.AddSpillCount( 1 / TimeFactorForRefilling );
        } catch {

        }
    }

    
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.name == "Robot"){
            print(" robot entered ");
            RobotInside = true;
            rm = other.gameObject.GetComponent<ResourceManager>();
        }           
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.name == "Robot"){
            print(" robot exited ");
            RobotInside = false;
            rm = null;
        }                  
    }


}
