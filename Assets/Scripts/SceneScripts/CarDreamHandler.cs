using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CarDreamHandler : MonoBehaviour
{
    public Transform playerCar;
    public Transform playerCarPositionAtDream1End;
    public GameObject carRadioPosition;
    [Space]
    public Transform playerCarPositionAtDream2End;
    public Transform crushingCar;
    public Transform crushingCarPositionAtDream2End;

    private Transform Player => PlayerObjects.Instance.Player.transform;

    [HideInInspector] public float blackoutTime = 1f;
    [HideInInspector] public float musicEndingTime = 1f;

    [HideInInspector] public float carMovementDurationDream1;
    [HideInInspector] public float carMovementDurationDream2;

    private void Start()
    {
        carMovementDurationDream1 = AudioManager.EventLength(FmodEvents.Instance.SPT_CarRadioSong) - blackoutTime - musicEndingTime;
        carMovementDurationDream2 =  Vector3.Distance(playerCar.position, crushingCarPositionAtDream2End.position) / CalculatePlayerCarSpeed();
    }

    private void LateUpdate()
    {
        //restrain between -90 and 90 on Y axis
        if (Player.localEulerAngles.y < 270 && Player.localEulerAngles.y > 180) Player.localRotation = Quaternion.Euler(0, 270, 0);
        else if (Player.localEulerAngles.y > 90 && Player.localEulerAngles.y < 180) Player.localRotation = Quaternion.Euler(0, 90, 0);
    }

    public IEnumerator OpenEyes(BlinkEffect blink)
    {
        yield return new WaitForSeconds(blackoutTime);

        blink.PlayOpenEyes(1.0f);
        while (blink.IsPlaying) yield return null;

        InputProvider.Instance.TurnOnGameplayOverlayMap();
    }

    public void TweenPlayerCar(bool isEnding1)
    {
        var pos = isEnding1 ? playerCarPositionAtDream1End : playerCarPositionAtDream2End;
        playerCar.DOMove(pos.position, CalculatePlayerCarSpeed()).SetEase(Ease.Linear).SetSpeedBased();
    }

    public void TweenCrashCar()
    {
        crushingCar.DOMove(crushingCarPositionAtDream2End.position, carMovementDurationDream2).SetEase(Ease.Linear);
    }

    private float CalculatePlayerCarSpeed()
    {
        float distance = Vector3.Distance(playerCar.position, playerCarPositionAtDream1End.position);
        float duration = carMovementDurationDream1;
        return distance / duration;
    }
}