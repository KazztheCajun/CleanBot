using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertFixedCamera : MonoBehaviour
{
    

    // on spawn, alert the fixed camera to this spill being spawned

    Camera camera;
    MeshRenderer renderer;
    Plane[] cameraFrustum;
    Collider collider;
    bool seen = false;

    private void Start() {
        camera = FixedCamera.instance.eyeCam;
        renderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
    }

    private void Update() {
        if(seen)
            return;
        // var bounds = collider.bounds;
        // cameraFrustum = GeometryUtility.CalculateFrustumPlanes(camera);
        // if(GeometryUtility.TestPlanesAABB(cameraFrustum, bounds)){
            FixedCamera.instance.SeeSpill(this.transform);
            seen = true;
        // } else {
        //     print(" turn red ");
        // }
    }



}
