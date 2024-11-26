using Cinemachine;
using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class EscapeEndingQuestHandler : MonoBehaviour
{
    [HideInInspector] public EndingChoice EndingChoice;

    [Header("Prefabs")]
    [SerializeField] private NeighbourCar _carPrefab;
    [SerializeField] private EscapeEndingDialogueChoices _dialogueChoicesPrefab;
    [SerializeField] private Blackout _blackoutPrefab;
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _escapeQuest;
    [SerializeField] private DialogueSequenceScriptable _neighbourDialogue;
    [SerializeField] private DialogueSequenceScriptable _neighbourWayDialogue;
    [SerializeField] private DialogueSequenceScriptable _neighbourExplainDialogue;
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
    [SerializeField] private PlayerTrigger _rightCarTrigger;
    [SerializeField] private Transform _rightPlayerTargetPos;
    [SerializeField] private Transform _rightCarStartingPos;
    [SerializeField] private Transform _rightCarTargetPos;
    [Header("Left Animation")]
    [SerializeField] private PlayerTrigger _leftCarTrigger;
    [SerializeField] private Transform _leftPlayerTargetPos;
    [SerializeField] private Transform _leftCarStartingPos;
    [SerializeField] private Transform _leftCarTargetPos;

    Transform Player => PlayerObjectsHolder.Instance.Player.transform;
    PlayerMovement PlayerMovement => PlayerObjectsHolder.Instance.PlayerMovement;

    private void Start()
    {
        _escapeQuest.OnQuestStart += StartEscapeQuest;

        _keys.OnInteract += () => GameState.Instance.TookKeys = true;

        _rightCarTrigger.OnPlayerTriggerEnter += AnimationRight;
        _leftCarTrigger.OnPlayerTriggerEnter += AnimationLeft;
    }

    private void StartEscapeQuest()
    {
        _doorLock.UnlockableHitbox.gameObject.SetActive(true);
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
        NeighbourCar car = Instantiate(_carPrefab);
        Transform carTransform = car.transform;
        carTransform.position = carStartingPos.position;
        carTransform.rotation = carStartingPos.rotation;

        float carTweensTime = 20f;
        Tween moveCarTween = carTransform.DOMove(carTargetPos.position, carTweensTime).SetEase(Ease.OutSine);
        Tween rotateCarTween = carTransform.DORotateQuaternion(carTargetPos.rotation, carTweensTime);

        //look at incoming car
        PlayerObjectsHolder.Instance.PlayerCharacterController.enabled = false;
        _playerCameraTarget.LookAt = car.DriverPointToLookAt;
        _playerCameraTarget.enabled = true;
        PlayerObjectsHolder.Instance.PlayerVirtualCamera.enabled = false;
        yield return null;
        while (CameraManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }

        //turn off flashlight and come to side of road looking forward
        PlayerObjectsHolder.Instance.PlayerEquipment.ChangeItem(ItemType.NONE);
        Tween movePlayerTween = Player.DOMove(playerTargetPos.position, 3f).SetSpeedBased().SetEase(Ease.InOutSine);

        //while walking add camerashake
        var noise = _playerCameraTarget.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_NoiseProfile = _noiseSettings;
        noise.m_AmplitudeGain = 0.7f;
        noise.m_FrequencyGain = 0.5f;

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
        UIManager.Instance.DisplayDialogueSequence(_neighbourDialogue);
        _neighbourDialogue.OnDialogueEnd += SpawnChoices;
    }

    private void SpawnChoices()
    {
        InputProvider.Instance.TurnOffGameplayOverlayMap();
        EscapeEndingDialogueChoices dialogueChoices = Instantiate(_dialogueChoicesPrefab);

        dialogueChoices.QuestHandler = this;
        dialogueChoices.OnChoiceMade += () => StartCoroutine(ManageChoice());
    }

    private IEnumerator ManageChoice()
    {
        //setup ending
        string nextScene = "";
        DialogueSequenceScriptable afterChoiceDialogue = null;

        if (EndingChoice == EndingChoice.Chris) 
        {
            nextScene = _escapeChrisEndingScene;
            afterChoiceDialogue = _neighbourWayDialogue;
        }
        else if (EndingChoice == EndingChoice.Claire)
        {
            nextScene = _escapeClaireEndingScene;
            afterChoiceDialogue = _neighbourWayDialogue;
        }
        else if (EndingChoice == EndingChoice.Police)
        {
            nextScene = _escapePoliceEndingScene;
            afterChoiceDialogue = _neighbourExplainDialogue;
        }

        //display dialogue
        UIManager.Instance.DisplayDialogueSequence(afterChoiceDialogue);
        yield return new WaitForSeconds(0.5f * afterChoiceDialogue.DialogueDuration());

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
    [SerializeField] private Transform _newPos; 
    [Button]
    private void TestQuest()
    {
        QuestManager.Instance.StartQuest(_escapeQuest);
        PlayerObjectsHolder.Instance.Player.transform.position = _newPos.position;
        PlayerObjectsHolder.Instance.PlayerMovement.RotateCharacter(_newPos.rotation.eulerAngles, 0f);
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
