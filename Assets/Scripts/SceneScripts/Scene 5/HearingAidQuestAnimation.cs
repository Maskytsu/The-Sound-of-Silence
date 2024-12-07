using System.Collections;
using UnityEngine;

public class HearingAidQuestAnimation : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _flashlightQuest;
    [SerializeField] private QuestScriptable _hearingAidQuest;
    [Header("Scene Objects")]
    [SerializeField] private Trigger _animationTrigger;
    [SerializeField] private HearingAid _hearingAid;
    [SerializeField] private GameObject _bloodyWallText;
    [SerializeField] private GameObject _harryGhost;
    [SerializeField] private GameObject _harryGhostDarkness;
    [SerializeField] private GameObject _harryGhostBrightness;

    private CameraManager CameraManager => CameraManager.Instance;
    private Item ItemInHand => PlayerObjects.Instance.PlayerEquipment.SpawnedItemInHand;

    private void Start()
    {
        _flashlightQuest.OnQuestEnd += () => QuestManager.Instance.StartQuest(_hearingAidQuest);
        _flashlightQuest.OnQuestEnd += ActivateCreepyRoomObjects;

        _animationTrigger.OnObjectTriggerEnter += () => StartCoroutine(CreepyAnimation());

        _hearingAid.OnInteract += () => QuestManager.Instance.EndQuest(_hearingAidQuest);
    }

    private void ActivateCreepyRoomObjects()
    {
        _animationTrigger.gameObject.SetActive(true);
        _hearingAid.gameObject.SetActive(true);
        _bloodyWallText.SetActive(true);
        _harryGhostBrightness.SetActive(true);
    }

    private IEnumerator CreepyAnimation()
    {
        _animationTrigger.gameObject.SetActive(false);

        GameObject lightCone = null;

        if (ItemInHand != null && ItemInHand.GetComponent<ItemFlashlight>() != null)
        {
            ItemFlashlight flashlight = ItemInHand.GetComponent<ItemFlashlight>();
            
            if (flashlight.LightCone.activeSelf)
            {
                lightCone = flashlight.LightCone;
            }
        }

        float rotationTime = 1.5f;
        float lookingAtTargetTime = 4.5f;

        StartCoroutine(CameraManager.LookAtTargetAnimation(
            _harryGhost.transform, rotationTime, lookingAtTargetTime));

        yield return new WaitForSeconds(rotationTime);

        float segmentsNumber = 3f;
        float oneSegmentTime = lookingAtTargetTime / segmentsNumber;
        Vector3 ghostStartingPos = _harryGhost.transform.position;
        Vector3 playerPos = PlayerObjects.Instance.Player.transform.position;

        for (int i = 0; i < segmentsNumber; i++)
        {
            yield return new WaitForSeconds(oneSegmentTime / 2);
            if (lightCone != null) lightCone.SetActive(false);
            _harryGhostBrightness.SetActive(false);
            _harryGhostDarkness.SetActive(true);
            _harryGhost.transform.position = Vector3.Lerp(ghostStartingPos, playerPos, (i + 1) / (segmentsNumber + 1));

            yield return new WaitForSeconds(oneSegmentTime / 2);
            if (lightCone != null) lightCone.SetActive(true);
            _harryGhostDarkness.SetActive(false);
        }

        yield return new WaitForSeconds(oneSegmentTime / 2);
        InputProvider.Instance.TurnOnPlayerMaps();
    }
}