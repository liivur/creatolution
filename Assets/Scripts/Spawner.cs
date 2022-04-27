using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float spawnDelay = 5;
    public GameObject world;

    IEnumerator SpawnObjectRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);
            SpawnObject();
        }
    }

    void SpawnObject()
    {
        Vector3 worldSize = world.GetComponent<Renderer>().bounds.size;
        Vector3 spawnTarget = new Vector3(world.transform.position.x + Random.Range(-worldSize.x/2, worldSize.x/2), transform.position.y, world.transform.position.z + Random.Range(-worldSize.z / 2, worldSize.z / 2));
        //print(spawnTarget);

        //spawnTarget.y = world.transform

        GameObject baby = Instantiate(objectToSpawn, spawnTarget, Quaternion.AngleAxis(Random.Range(0, 360), transform.up));
        Character script = baby.GetComponent<Character>();
        script.speed = Random.Range(4.0f, 6.0f);
        script.viewAngle = Random.Range(30, 180);
        script.viewRadius = Random.Range(10, 30);
        script.changeRange = Random.Range(30, 80);
        script.changeChance = Random.Range(0.2f, 0.8f);
        script.staminaRegen = Random.Range(2f, 8f);
    }

    // Start is called before the first frame update
    void Start()
    {
        print(world.transform.position);
        print(world.GetComponent<Renderer>().bounds.size);

        StartCoroutine("SpawnObjectRoutine");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
