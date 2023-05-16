using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSceneScript : MonoBehaviour
{


    private bool paused;

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            toggleTime();
        }
    }

    void toggleTime(){

        if(paused){
             Time.timeScale = 1f;
             paused = false;
        } else {
             Time.timeScale = 0f;
             paused = true;
        }

    }



}
