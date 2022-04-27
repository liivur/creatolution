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
        Character[] characters = FindObjectsOfType<Character>();
        foreach (Character character in characters)
        {
            if (character.type == "Herbivore")
            {
                herbivores++;
            } else
            {
                predators++;
            }
        }

        textContainer.SetText("Herbivores: {0}, Predators: {1}", herbivores, predators);
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
