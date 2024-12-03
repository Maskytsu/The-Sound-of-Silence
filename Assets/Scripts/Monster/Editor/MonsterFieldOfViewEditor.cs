using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonsterFieldOfView))]
public class MonsterFieldOfViewEditor : Editor
{
    private MonsterFieldOfView _monsterFOV;

    private void OnSceneGUI()
    {
        _monsterFOV = (MonsterFieldOfView)target;

        if (_monsterFOV.SeesPlayer) Handles.color = Color.green;
        else Handles.color = Color.red;

        DrawFOVRange();
        DrawFOVAngle();
        DrawDistanceToPlayer();
        DrawCatchRange();
    }

    private void DrawFOVRange()
    {
        Handles.DrawWireArc(_monsterFOV.FOVStartingPoint.position, Vector3.up, Vector3.forward, 360, _monsterFOV.Radius);
    }

    private void DrawFOVAngle()
    {
        Vector3 viewAngleLeft = DirectionFromAngle(_monsterFOV.FOVStartingPoint.eulerAngles.y, -_monsterFOV.Angle / 2);
        Vector3 viewAngleRight = DirectionFromAngle(_monsterFOV.FOVStartingPoint.eulerAngles.y, _monsterFOV.Angle / 2);

        Vector3 viewCirclePointLeft = _monsterFOV.FOVStartingPoint.position + (viewAngleLeft * _monsterFOV.Radius);
        Vector3 viewCirclePointRight = _monsterFOV.FOVStartingPoint.position + (viewAngleRight * _monsterFOV.Radius);

        Handles.DrawLine(_monsterFOV.FOVStartingPoint.position, viewCirclePointLeft);
        Handles.DrawLine(_monsterFOV.FOVStartingPoint.position, viewCirclePointRight);
    }

    private void DrawDistanceToPlayer()
    {
        if (_monsterFOV.SeesPlayer && _monsterFOV.SeenPlayerObj != null)
        {
            Handles.color = Color.magenta;
            Handles.DrawLine(_monsterFOV.FOVStartingPoint.position, _monsterFOV.SeenPlayerObj.transform.position);
        }
    }

    private void DrawCatchRange()
    {
        Handles.color = Color.red;
        Handles.DrawWireArc(_monsterFOV.FOVStartingPoint.position, Vector3.up, Vector3.forward, 360, _monsterFOV.CatchRange);
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
