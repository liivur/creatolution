using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatingSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;

    public Character SpawnObject(Character parentA, Character parentB)
    {
        Vector3 spawnTarget = (parentA.transform.position + parentB.transform.position) / 2;

        GameObject baby = Instantiate(objectToSpawn, spawnTarget, Quaternion.AngleAxis(Random.Range(0, 360), transform.up));
        Character script = baby.GetComponent<Character>();
        float speed = Random.Range(parentA.speed, parentB.speed);
        script.speed = speed + Random.Range(speed * -0.1f, speed * 0.1f);
        float viewAngle = Random.Range(parentA.viewAngle, parentB.viewAngle);
        script.viewAngle = viewAngle + Random.Range(viewAngle * -0.1f, viewAngle * 0.1f);
        float viewRadius = Random.Range(parentA.viewRadius, parentB.viewRadius);
        script.viewRadius = viewRadius + Random.Range(viewRadius * -0.1f, viewRadius * 0.1f);
        float changeRange = Random.Range(parentA.changeRange, parentB.changeRange);
        script.changeRange = changeRange + Random.Range(changeRange * -0.1f, changeRange * 0.1f);
        float changeChance = Random.Range(parentA.changeChance, parentB.changeChance);
        script.changeChance = changeChance + Random.Range(changeChance * -0.1f, changeChance * 0.1f);
        float staminaRegen = Random.Range(parentA.staminaRegen, parentB.staminaRegen);
        script.staminaRegen = staminaRegen + Random.Range(staminaRegen * -0.1f, staminaRegen * 0.1f);
        float maxHealth = Random.Range(parentA.health.max, parentB.health.max);
        script.staminaRegen = maxHealth + Random.Range(maxHealth * -0.1f, maxHealth * 0.1f);
        float aggression = Random.Range(parentA.aggression, parentB.aggression);
        script.aggression = aggression + Random.Range(aggression * -0.1f, aggression * 0.1f);
        float persistence = Random.Range(parentA.persistence, parentB.persistence);
        script.persistence = persistence + Random.Range(persistence * -0.1f, persistence * 0.1f);
        float matingCallFrequency = Random.Range(parentA.matingCallFrequency, parentB.matingCallFrequency);
        script.matingCallFrequency = matingCallFrequency + Random.Range(matingCallFrequency * -0.1f, matingCallFrequency * 0.1f);
        float offspringContribution = Random.Range(parentA.offspringContribution, parentB.offspringContribution);
        script.offspringContribution = offspringContribution + Random.Range(offspringContribution * -0.1f, offspringContribution * 0.1f);
        float matingCallRadius = Random.Range(parentA.matingCallRadius, parentB.matingCallRadius);
        script.matingCallRadius = matingCallRadius + Random.Range(matingCallRadius * -0.1f, matingCallRadius * 0.1f);

        return script;
    }
}
