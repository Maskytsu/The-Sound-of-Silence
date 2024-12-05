using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonsterFieldOfView))]
public class MonsterFieldOfViewEditor : Editor
{
    private MonsterFieldOfView _monsterFOV;

    private void OnSceneGUI()
    {
        _monsterFOV = (MonsterFieldOfView)target;

        if (_monsterFOV.SeesPlayer) Handles.color = Color.red;
        else Handles.color = Color.yellow;

        DrawFOVRange();
        DrawFOVAngle();
        DrawDistanceToPlayer();
    }

    private void DrawFOVRange()
    {
        Vector3 viewAngleLeftPoint = DirectionFromAngle(_monsterFOV.FOVStartingPoint.eulerAngles.y, -_monsterFOV.Angle / 2);
        Handles.DrawWireArc(_monsterFOV.FOVStartingPoint.position, Vector3.up, viewAngleLeftPoint, _monsterFOV.Angle, _monsterFOV.Radius);
    }

    private void DrawFOVAngle()
    {
        Vector3 viewAngleLeftPoint = DirectionFromAngle(_monsterFOV.FOVStartingPoint.eulerAngles.y, -_monsterFOV.Angle / 2);
        Vector3 viewAngleRightPoint = DirectionFromAngle(_monsterFOV.FOVStartingPoint.eulerAngles.y, _monsterFOV.Angle / 2);

        Vector3 viewCirclePointLeft = _monsterFOV.FOVStartingPoint.position + (viewAngleLeftPoint * _monsterFOV.Radius);
        Vector3 viewCirclePointRight = _monsterFOV.FOVStartingPoint.position + (viewAngleRightPoint * _monsterFOV.Radius);

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

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
