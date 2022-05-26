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
    public GameObject plantPrefab;
    public GameObject floatingTextPrefab;
    public LayerMask obstacleMask;

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

    public Plant Mate(Plant partner, Vector3 location)
    {
        ShowMessage("*POOF!*");

        GameObject baby = Instantiate(plantPrefab, location, Quaternion.identity);
        Plant script = baby.GetComponent<Plant>();
        float newGrowthSpeed = Random.Range(growthSpeed, partner.growthSpeed);
        script.growthSpeed = newGrowthSpeed + Random.Range(newGrowthSpeed * -0.1f, newGrowthSpeed * 0.1f);
        float newFruitDelay = Random.Range(fruitDelay, partner.fruitDelay);
        script.fruitDelay = newFruitDelay + Random.Range(newFruitDelay * -0.1f, newFruitDelay * 0.1f);
        float newMinFruitingSize = Random.Range(minFruitingSize, partner.minFruitingSize);
        script.minFruitingSize = newMinFruitingSize + Random.Range(newMinFruitingSize * -0.1f, newMinFruitingSize * 0.1f);
        float newMaxSize = Random.Range(maxSize, partner.maxSize);
        script.maxSize = newMaxSize + Random.Range(newMaxSize * -0.1f, newMaxSize * 0.1f);

        return script;
    }

    void ProduceFruit()
    {
        if (transform.localScale.y > minFruitingSize && power > 0)
        {
            Vector3 plantSize = GetComponent<Renderer>().bounds.size;
            //print(plantSize);
            //Vector3 spawnTarget = new Vector3(transform.position.x + Random.Range(-plantSize.x / 2, plantSize.x / 2), transform.position.y, transform.position.z + Random.Range(-plantSize.z / 2, plantSize.z / 2));
            Vector3 spawnTarget = transform.position + Random.onUnitSphere * Random.Range(plantSize.x, plantSize.x * 3);

            //print(spawnTarget);

            //spawnTarget.y = world.transform

            ShowMessage("*SPROINK*");

            GameObject item = Instantiate(fruitPrefab, spawnTarget, Quaternion.AngleAxis(Random.Range(0, 360), transform.forward));
            Health health = item.GetComponent<Health>();
            power -= health.max;

            Fruit fruit = item.GetComponent<Fruit>();
            if (fruit)
            {
                fruit.parent = this;
            }
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

        power += Time.deltaTime * transform.localScale.y;

        Collider[] targetsInShadowRadius = Physics.OverlapSphere(transform.position, transform.localScale.y * 0.4f, obstacleMask);
        for (int i = 0; i < targetsInShadowRadius.Length; i++)
        {
            Transform target = targetsInShadowRadius[i].transform;
            if (target.transform.root == transform || (target.parent && target.parent.transform.root == transform))
            {
                continue;
            }

            if (target.transform.localScale.y > transform.localScale.y)
            {
                power -= (target.transform.localScale.y - transform.localScale.y) * Time.deltaTime * 0.2f;
            }
        }

        float growthCost = growthSpeed * Time.deltaTime * 0.3f;
        if (transform.localScale.y < maxSize && power > growthCost)
        {
            power -= growthCost;

            transform.localScale += new Vector3(growthCost * 0.08f, growthCost, growthCost * 0.08f);
            transform.position += new Vector3(0, growthSpeed * Time.deltaTime / 3, 0);
            if (transform.position.y > (transform.localScale.y / 2))
            {
                transform.position = new Vector3(transform.position.x, transform.localScale.y / 2, transform.position.z);
            }
        }

        if (power < -1000)
        {
            ShowMessage("*CROAK*");
            Destroy(gameObject);
        }
    }

    public void ShowMessage(string text)
    {
        if (floatingTextPrefab)
        {
            GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Camera.main.transform.rotation);
            TextMesh textMesh = prefab.GetComponentInChildren<TextMesh>();
            if (textMesh)
            {
                textMesh.text = text;
            }
        }
    }
}
