using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    
    public static HumanController instance;

    public List<Human> humans = new List<Human>();
    public List<Human> deactiveHumans = new List<Human>();

    public Transform StartingLocale;


    private void Awake() {
        instance = this;
    }

    public void AddHuman(Human x){
        humans.Add( x );
        deactiveHumans.Add(x);
    }

    public void BeginAllHumans(){
        foreach( Human n in deactiveHumans.ToArray() ){ 
            BeginNextAvailableHuman();
        }
    }


    public void BeginNextAvailableHuman(){
        // reset a deactivated human
        // start them on their journey
        Human oneToActivate = deactiveHumans[0];
        // oneToActivate.ResetEverything();
        ActivateThisHuman(oneToActivate);



    }

    public void ActivateThisHuman(Human m){


        deactiveHumans.Remove(m);
        ShuffleDeactiveHumanList();
        m.BeginHuman();

    }

    
    void ShuffleDeactiveHumanList(){
        for (int i = 0; i < deactiveHumans.Count; i++) {
            Human temp = deactiveHumans[i];
            int randomIndex = Random.Range(i, deactiveHumans.Count);
            deactiveHumans[i] = deactiveHumans[randomIndex];
            deactiveHumans[randomIndex] = temp;
        }
    }

    public void ThisHumanIsDone(Human m){
        m.ResetEverything();
        deactiveHumans.Add(m);

    }

    public void MoveThisHumanToCorrectStartingLocation(GameObject m){
        m.transform.position = new Vector3(StartingLocale.position.x, StartingLocale.position.y, StartingLocale.position.z );
    }



}
