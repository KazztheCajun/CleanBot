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
    private bool canSpawn;
    private float cooldown;

    // Start is called before the first frame update
    void Start()
    {
        canSpawn = true;
        cooldown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(canSpawn)
        {
            Vector3 loc = new Vector3(Random.Range(-width, width), .1f, Random.Range(-height, height));
            GameObject temp = Instantiate(spill, loc, Quaternion.identity);
            bot.spills.Add(temp.transform);
            canSpawn = false;
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

    public void RemoveSpill()
    {
        Destroy(bot.current.gameObject);
    }
}
