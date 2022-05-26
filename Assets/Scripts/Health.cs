using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [HideInInspector]
    public float current;
    public float max = 100;

    AI ai;
    Character character;

    public void Defend(Transform attacker, float damage)
    {
        current -= damage;
        if (ai)
        {
            ai.Defend(attacker, damage);
        }
        if (character)
        {
            character.ShowMessage("*OUCH!*");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        current = max;
        ai = gameObject.GetComponent<AI>();
        character = gameObject.GetComponent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
