using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Baker : MonoBehaviour
{
    
    // public NavMeshSurface surface;

    public List<NavMeshSurface> surfaces = new List<NavMeshSurface>();

    void Start(){

        foreach( NavMeshSurface m in surfaces ){ 
            m.BuildNavMesh();
        }

        // surface.BuildNavMesh();


    }



}
