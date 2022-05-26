using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreatureOverlay : MonoBehaviour
{
    TextMeshProUGUI textContainer;

    void SetCreateureCounts()
    {
        int herbivores = 0;
        int predators = 0;
        float speed = 0;
        float health = 0;
        Character[] characters = FindObjectsOfType<Character>();
        foreach (Character character in characters)
        {
            if (character.type == Character.FoodType.Herbivore)
            {
                herbivores++;
            } else
            {
                predators++;
            }
            speed += character.speed;
            health += character.health.current;
        }

        int sum = System.Math.Max(1, herbivores + predators);
        textContainer.SetText("Herbivores: {0}, Predators: {1}, Average speed: {2:2}, Average health: {3:0}", herbivores, predators, speed / sum, health / sum);
    }

    // Start is called before the first frame update
    void Start()
    {
        textContainer = GetComponent<TextMeshProUGUI>();
        SetCreateureCounts();
    }

    void FixedUpdate()
    {
        SetCreateureCounts();
    }
}
