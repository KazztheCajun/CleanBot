using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpillManager : MonoBehaviour
{
    public GameObject spill;
    public int width;
    public int height;
    [Range(0.0f,100.0f)]
    public float spawnRate; // spawn a spill every x seconds
    public Robot bot;
    public WindowRobot bot2;
    private bool canSpawn;
    private float cooldown;
    private List<GameObject> preload;
    private Vector3 pre_loc = new Vector3(1000, 1000, 1000);
    private int nextSpill;

    // Start is called before the first frame update
    void Start()
    {
        canSpawn = true;
        cooldown = 0;
        nextSpill = 0;
        preload = new List<GameObject>();
        for (int i = 0; i < 200; i++)
        {
            GameObject t = Instantiate(spill, pre_loc, Quaternion.identity);
            t.SetActive(false);
            preload.Add(t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(canSpawn && nextSpill < 200)
        {
            Vector3 loc = new Vector3(Random.Range(-width, width), .1f, Random.Range(-height, height));
            GameObject t = preload[nextSpill];
            if(bot != null)
            {
                bot.spills.Add(t.transform);
            }
            if(bot2 != null)
            {
                bot2.spills.Add(t.transform);
            }
            
            t.transform.position = loc;
            t.SetActive(true);
            canSpawn = false;
            nextSpill++;
        }
        else
        {
            cooldown += Time.deltaTime;
            if(cooldown >= spawnRate)
            {
                canSpawn = true;
                cooldown = 0;
            }
        }
    }

    [ContextMenu("Spawn Spill")]
    void SpawnSpill(){
        Vector3 loc = new Vector3(Random.Range(-width, width), .1f, Random.Range(-height, height));
        GameObject t = preload[nextSpill];
        bot.spills.Add(t.transform);
        t.transform.position = loc;
        t.SetActive(true);
        canSpawn = false;
        nextSpill++;
        if(FixedCamera.instance != null)
            FixedCamera.instance.SeeSpill(t.transform);
    }
}
