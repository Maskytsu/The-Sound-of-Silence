using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

[CustomEditor(typeof(MonsterFieldOfView))]
public class MonsterFieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        MonsterFieldOfView monsterFOV = (MonsterFieldOfView)target;

        Handles.color = Color.red;
        Handles.DrawWireArc(monsterFOV.FOVStartingPoint.position, Vector3.up, Vector3.forward, 360, monsterFOV.Radius);

        Vector3 viewAngleLeft = DirectionFromAngle(monsterFOV.FOVStartingPoint.eulerAngles.y, -monsterFOV.Angle / 2);
        Vector3 viewAngleRight = DirectionFromAngle(monsterFOV.FOVStartingPoint.eulerAngles.y, monsterFOV.Angle / 2);

        if (monsterFOV.SeesPlayer) Handles.color = Color.green;
        else Handles.color = Color.red;

        Handles.DrawLine(monsterFOV.FOVStartingPoint.position, monsterFOV.FOVStartingPoint.position + (viewAngleLeft * monsterFOV.Radius));
        Handles.DrawLine(monsterFOV.FOVStartingPoint.position, monsterFOV.FOVStartingPoint.position + (viewAngleRight * monsterFOV.Radius));


        if (monsterFOV.SeesPlayer && monsterFOV.SeenPlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(monsterFOV.FOVStartingPoint.position, monsterFOV.SeenPlayer.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
