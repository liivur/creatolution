using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/*
[CustomEditor (typeof (Character))]
public class FovEditor : Editor
{
    private void OnSceneGUI()
    {
        //Console.WriteLine("wtf");
        Character character = (Character)target;
        if (character)
        {
            Handles.color = Color.white;
            Handles.DrawWireArc(character.transform.position, Vector3.up, Vector3.forward, 360, character.viewRadius);

            Vector3 viewAngleA = character.ai.DirectionFromAngle(-character.viewAngle / 2, false);
            Vector3 viewAngleB = character.ai.DirectionFromAngle(character.viewAngle / 2, false);

            Handles.DrawLine(character.transform.position, character.transform.position + viewAngleA * character.viewRadius);
            Handles.DrawLine(character.transform.position, character.transform.position + viewAngleB * character.viewRadius);

            Handles.color = Color.red;
            foreach (KeyValuePair<float, Transform> visibleTarget in character.ai.visibleTargets)
            {
                Handles.DrawLine(character.transform.position, visibleTarget.Value.position);
            }

            Handles.color = Color.blue;
            Handles.DrawLine(character.transform.position, character.ai.targetPosition);
        }
        
        
    }
}
*/