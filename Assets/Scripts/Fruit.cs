using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    [HideInInspector]
    public Health health;
    [HideInInspector]
    public Plant parent;

    // Start is called before the first frame update
    void Start()
    {
        health = gameObject.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health.current <= 0)
        {
            Destroy(gameObject);
        }
    }
}
