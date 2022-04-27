using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public float growthSpeed = 0.1f;
    public float fruitDelay = 5;
    public float minFruitingSize = 15;
    public float maxSize = 30;
    public GameObject fruitPrefab;

    float age = 0;
    float power = 0;

    IEnumerator ProduceFruitRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fruitDelay);
            ProduceFruit();
        }
    }

    void ProduceFruit()
    {
        if (transform.localScale.y > minFruitingSize)
        {
            Vector3 plantSize = GetComponent<Renderer>().bounds.size;
            print(plantSize);
            //Vector3 spawnTarget = new Vector3(transform.position.x + Random.Range(-plantSize.x / 2, plantSize.x / 2), transform.position.y, transform.position.z + Random.Range(-plantSize.z / 2, plantSize.z / 2));
            Vector3 spawnTarget = transform.position + Random.onUnitSphere * Random.Range(plantSize.x, plantSize.x * 3);

            //print(spawnTarget);

            //spawnTarget.y = world.transform

            GameObject fruit = Instantiate(fruitPrefab, spawnTarget, Quaternion.AngleAxis(Random.Range(0, 360), transform.forward));
            Fruit script = fruit.GetComponent<Fruit>();
            power -= script.health;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("ProduceFruitRoutine");
    }

    // Update is called once per frame
    void Update()
    {
        age += Time.deltaTime;

        power += Time.deltaTime * transform.localScale.y / age;

        if (transform.localScale.y < maxSize)
        {
            transform.localScale += new Vector3(growthSpeed * Time.deltaTime * 0.08f, growthSpeed * Time.deltaTime, growthSpeed * Time.deltaTime * 0.08f);
            transform.position += new Vector3(0, growthSpeed * Time.deltaTime / 2, 0);
        }
    }
}
