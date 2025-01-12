using Cinemachine;
using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class EscapeEndingQuestHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private NeighbourCar _carPrefab;
    [SerializeField] private EscapeEndingDialogueChoices _dialogueChoicesPrefab;
    [SerializeField] private Blackout _blackoutPrefab;
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _escapeQuest;
    [SerializeField] private DialogueSequenceScriptable _neighbourDialogue;
    [SerializeField] private DialogueSequenceScriptable _neighbourWayDialogue;
    [SerializeField] private DialogueSequenceScriptable _neighbourExplainDialogue;
    [SerializeField] private DialogueSequenceScriptable _neighbourNoHearingDialogue;
    [Header("Scene Objects")]
    [SerializeField] private DoorLock _doorLock;
    [SerializeField] private PickableItem _keys;
    [SerializeField] private CinemachineVirtualCamera _playerCameraTarget;
    [Header("Parameters")]
    [SerializeField] private NoiseSettings _noiseSettings;
    [SerializeField, Scene] private string _escapeChrisEndingScene;
    [SerializeField, Scene] private string _escapeClaireEndingScene;
    [SerializeField, Scene] private string _escapePoliceEndingScene;
    [HorizontalLine]
    [Header("Right Animation")]
    [SerializeField] private Trigger _rightCarTrigger;
    [SerializeField] private Transform _rightPlayerTargetPos;
    [SerializeField] private Transform _rightCarStartingPos;
    [SerializeField] private Transform _rightCarTargetPos;
    [Header("Left Animation")]
    [SerializeField] private Trigger _leftCarTrigger;
    [SerializeField] private Transform _leftPlayerTargetPos;
    [SerializeField] private Transform _leftCarStartingPos;
    [SerializeField] private Transform _leftCarTargetPos;

    private NeighbourCar _spawnedCar;

    private Transform Player => PlayerObjects.Instance.Player.transform;
    private PlayerMovement PlayerMovement => PlayerObjects.Instance.PlayerMovement;

    private void Start()
    {
        _escapeQuest.OnQuestStart += StartEscapeQuest;

        _keys.OnInteract += () => GameState.Instance.TookKeys = true;

        _rightCarTrigger.OnObjectTriggerEnter += AnimationRight;
        _leftCarTrigger.OnObjectTriggerEnter += AnimationLeft;
    }

    private void StartEscapeQuest()
    {
        //this is for the scene 5, but i use this script on scene 7 where it is not needed
        if (_doorLock != null) _doorLock.UnlockableHitbox.gameObject.SetActive(true);
        else Debug.LogWarning("_doorLock is null! Is it intentional?");
        //this is also not needed on scene 7 but _keys are already needed here
        _keys.InteractionHitbox.gameObject.SetActive(true);
    }

    private void AnimationRight()
    {
        StartCoroutine(CarAnimation(_rightPlayerTargetPos, _rightCarStartingPos, _rightCarTargetPos));
    }

    private void AnimationLeft()
    {
        StartCoroutine(CarAnimation(_leftPlayerTargetPos, _leftCarStartingPos, _leftCarTargetPos));
    }

    private IEnumerator CarAnimation(Transform playerTargetPos, Transform carStartingPos, Transform carTargetPos)
    {
        InputProvider.Instance.TurnOffPlayerMaps();

        //spawn car and move it
        _spawnedCar = Instantiate(_carPrefab);
        Transform carTransform = _spawnedCar.transform;
        carTransform.position = carStartingPos.position;
        carTransform.rotation = carStartingPos.rotation;

        float carTweensTime = 20f;
        Tween moveCarTween = carTransform.DOMove(carTargetPos.position, carTweensTime).SetEase(Ease.OutSine);
        Tween rotateCarTween = carTransform.DORotateQuaternion(carTargetPos.rotation, carTweensTime);

        yield return new WaitForSeconds(0.75f);
        //look at incoming car
        PlayerMovement.SetCharacterController(false);

        _playerCameraTarget.LookAt = _spawnedCar.DriverPointToLookAt;
        _playerCameraTarget.enabled = true;
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = false;
        yield return null;
        while (CameraManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }

        //turn off flashlight
        PlayerObjects.Instance.PlayerEquipment.ChangeItem(ItemType.NONE);

        //add camerashake
        var noise = _playerCameraTarget.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_NoiseProfile = _noiseSettings;
        noise.m_AmplitudeGain = 0.7f;
        noise.m_FrequencyGain = 0.5f;

        //come to side of road and destroy camera shake after that
        Tween movePlayerTween = Player.DOMove(playerTargetPos.position, 2f).SetSpeedBased().SetEase(Ease.InOutSine);
        while (movePlayerTween.IsPlaying())
        {
            yield return null;
        }
        _playerCameraTarget.DestroyCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        //wait for car tweens to end
        while (moveCarTween.IsPlaying() || rotateCarTween.IsPlaying())
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);

        //display dialogue and choices after it
        if (AudioManager.Instance.IsAbleToHear)
        {
            UIManager.Instance.DisplayDialogueSequence(_neighbourDialogue);
            _neighbourDialogue.OnDialogueEnd += SpawnChoices;
        }
        else
        {
            UIManager.Instance.DisplayDialogueSequence(_neighbourNoHearingDialogue);
            _neighbourNoHearingDialogue.OnDialogueEnd += () => StartCoroutine(NeighbourNod());
        }
    }

    private IEnumerator NeighbourNod()
    {
        yield return new WaitForSeconds(0.75f);
        Vector3 baseLocalRot = _spawnedCar.NeighbourHead.localEulerAngles;
        Vector3 targetLocalRot = _spawnedCar.NeighbourHead.localEulerAngles;
        targetLocalRot.x = 30f;

        Tween nodDownTween = _spawnedCar.NeighbourHead.DOLocalRotate(targetLocalRot, 0.75f).SetEase(Ease.InOutSine);
        while (nodDownTween.IsPlaying()) yield return null;

        yield return new WaitForSeconds(0.25f);

        Tween nodUpTween = _spawnedCar.NeighbourHead.DOLocalRotate(baseLocalRot, 0.75f).SetEase(Ease.InOutSine);
        while (nodDownTween.IsPlaying()) yield return null;

        yield return new WaitForSeconds(0.75f);
        SpawnChoices();
    }

    private void SpawnChoices()
    {
        InputProvider.Instance.TurnOffGameplayOverlayMap();
        EscapeEndingDialogueChoices dialogueChoices = Instantiate(_dialogueChoicesPrefab);

        dialogueChoices.QuestHandler = this;
        dialogueChoices.OnChoiceMade += (EndingChoice endingChoice) => StartCoroutine(ManageChoice(endingChoice));
    }

    private IEnumerator ManageChoice(EndingChoice endingChoice)
    {
        //setup ending
        string nextScene = "";
        DialogueSequenceScriptable afterChoiceDialogue = null;

        if (endingChoice == EndingChoice.Chris) 
        {
            nextScene = _escapeChrisEndingScene;
            afterChoiceDialogue = _neighbourWayDialogue;
        }
        else if (endingChoice == EndingChoice.Claire)
        {
            nextScene = _escapeClaireEndingScene;
            afterChoiceDialogue = _neighbourWayDialogue;
        }
        else if (endingChoice == EndingChoice.Police)
        {
            nextScene = _escapePoliceEndingScene;
            afterChoiceDialogue = _neighbourExplainDialogue;
        }

        Debug.Log(endingChoice);

        if (AudioManager.Instance.IsAbleToHear)
        {
            //display dialogue
            UIManager.Instance.DisplayDialogueSequence(afterChoiceDialogue);
            yield return new WaitForSeconds(0.75f * afterChoiceDialogue.DialogueDuration());
        }
        else
        {
            yield return new WaitForSeconds(0.75f);
        }

        //blackout and then change scene
        Blackout blackoutBackground = Instantiate(_blackoutPrefab);
        blackoutBackground.SetAlphaToZero();

        Tween fadeTween = blackoutBackground.Image.DOFade(1f, 2f);
        while (fadeTween.IsActive())
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        GameManager.Instance.LoadSceneAndSaveGameState(nextScene);
    }

    //---------------------------------------------------------
    [HorizontalLine]
    [SerializeField] private PlayerTargetTransform _newTransform;
    [SerializeField] private bool _canHearForTesting;
    [Button]
    private void TestQuest()
    {
        QuestManager.Instance.StartQuest(_escapeQuest);
        AudioManager.Instance.ChangeIsAbleToHear(_canHearForTesting);
        PlayerObjects.Instance.Player.transform.position = _newTransform.Position;
        PlayerObjects.Instance.PlayerMovement.RotateCharacter(_newTransform.Rotation);
        ItemManager.Instance.ItemsPerType[ItemType.KEYS].PlayerHasIt = true;
        GameState.Instance.TookKeys = true;
        ItemManager.Instance.ItemsPerType[ItemType.FLASHLIGHT].PlayerHasIt = true;
    }
}

public enum EndingChoice
{
    Chris,
    Claire,
    Police
}
