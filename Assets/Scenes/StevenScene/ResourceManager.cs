using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    

    public Slider guage;

    public float currentSpillCountRaw, maxSpillCountRaw;

    private void Update() {
        guage.value = (float)(currentSpillCountRaw / maxSpillCountRaw);
    }

    [ContextMenu("AddSpillCount")]
    public void AddSpillCount(){
        currentSpillCountRaw+=10;
    }
    public void AddSpillCount(float m){
        currentSpillCountRaw+=m;
    }

    public void SubtractSpillCount(float m){
        currentSpillCountRaw -= m;
    }



}
