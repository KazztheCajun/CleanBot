using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpillManager : MonoBehaviour
{
    public GameObject spill;
    public FixedCamera FixedCamera;
    public int width;
    public int height;
    [Range(0.0f,100.0f)]
    public float spawnRate; // spawn a spill every x seconds
    public Robot bot;
    public WindowRobot bot2;
    
    public SpillSpawnEvent SpillSpawned;


    private bool canSpawn;
    private float cooldown;
    private List<GameObject> preload;
    private Vector3 pre_loc = new Vector3(1000, 1000, 1000);


    // Start is called before the first frame update
    void Start()
    {
        canSpawn = true;
        cooldown = 0;
        preload = new List<GameObject>();
        Transform parent = GameObject.Find("Preload").transform;
        for (int i = 0; i < 200; i++)
        {
            GameObject t = Instantiate(spill, pre_loc, Quaternion.identity);
            t.transform.parent = parent;
            t.SetActive(false);
            preload.Add(t);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(canSpawn)
        {
            SpawnSpill();
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
        Vector3 loc = new Vector3(Random.Range(-width, width), .1f, Random.Range(-height, height)); // create new spill
        GameObject t = NextPreload();
        if(t == null) return; // escape if you can't get a preload at this time
        GiveToBot(t.transform);
        t.transform.position = loc;
        t.SetActive(true);
        canSpawn = false;
        //nextSpill++;
        // if(FixedCamera != null)
        //     FixedCamera.SeeSpill(t.transform);
        SpillSpawned.Invoke(t.transform);
    }

    private void GiveToBot(Transform spill) // add a spill to any type of bot in the scene
    {
        if(bot != null)
        {
            bot.spills.Add(spill);
        }
        if(bot2 != null)
        {
            bot2.spills.Add(spill);
        }
    }

    private GameObject NextPreload()
    {
        for (int i = 0; i < this.preload.Count; i++)
        {
            if (!preload[i].activeInHierarchy)
            {
                return preload[i];
            }
        }

        return null; // if there are no preload objects left | should not happen
    }
}
[System.Serializable]
public class SpillSpawnEvent : UnityEvent<Transform> {}

