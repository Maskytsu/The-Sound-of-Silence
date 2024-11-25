using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class EscapeEndingQuestHandler : MonoBehaviour
{
    [HideInInspector] public string NextScene;

    [Header("Prefabs")]
    [SerializeField] private GameObject _car;
    [SerializeField] private EscapeEndingDialogueChoice _dialogueChoices;
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _escapeQuest;
    [SerializeField] private DialogueSequenceScriptable _neighbourDialogue;
    [Header("Scene Objects")]
    [SerializeField] private DoorLock _doorLock;
    [SerializeField] private PickableItem _keys;
    [Space]
    [Header("Right Animation")]
    [SerializeField] private PlayerTrigger _rightCarTrigger;
    [SerializeField] private Vector3 _rightPlayerTargetPos;
    [SerializeField] private Vector3 _rightPlayerTargerRot;

    [SerializeField] private Vector3 _rightCarStartingPos;
    [SerializeField] private Vector3 _rightCarTargetPos;

    [SerializeField] private Vector3 _rightCarStartingRot;
    [SerializeField] private Vector3 _rightCarTargetRot;
    [Header("Left Animation")]
    [SerializeField] private PlayerTrigger _leftCarTrigger;
    [SerializeField] private Vector3 _leftPlayerTargetPos;
    [SerializeField] private Vector3 _leftPlayerTargerRot;

    [SerializeField] private Vector3 _leftCarStartingPos;
    [SerializeField] private Vector3 _leftCarTargetPos;

    [SerializeField] private Vector3 _leftCarStartingRot;
    [SerializeField] private Vector3 _leftCarTargetRot;


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
        StartCoroutine(CarAnimation(
            _rightPlayerTargetPos, _rightPlayerTargerRot,
            _rightCarStartingPos, _rightCarTargetPos,
            _rightCarTargetRot, _rightCarTargetRot));
    }

    private void AnimationLeft()
    {
        StartCoroutine(CarAnimation(
            _leftPlayerTargetPos, _leftPlayerTargerRot,
            _leftCarStartingPos, _leftCarTargetPos,
            _leftCarTargetRot, _leftCarTargetRot));
    }

    private IEnumerator CarAnimation(
        Vector3 playerTargetPos, Vector3 playerTargetRot,
        Vector3 carStartignPos, Vector3 carTargetPos, 
        Vector3 carStartingRot, Vector3 carTargetRot)
    {
        InputProvider.Instance.TurnOffPlayerMaps();
        //spawnujemy tutaj auto i odpalamy mu tweena
        yield return StartCoroutine(CameraManager.Instance.LookAtTargetAnimation(carStartignPos, 2f, 2f));

        //wy��czamy latark� i przesuwamy gracza na bok drogi (obraca si� w stron�, w kt�r� idzie)
        //potem gracz obraca si� w stron� carTargetPos
        //auto zatrzymuje si� i wida� posta� w �rodku
        //odpala si� dialog
        //po dialogu odpala si� choices (trzeba mu przekaza� this pod zmienn� QuestHandler) (turnOff gameplayoverlaymap)
        //potem prze��cza si� scena na t� przypisan� przez choices do NextScene

        yield return null;
    }


    //---------------------------------------------------------
    [Button]
    private void TestQuest()
    {
        QuestManager.Instance.StartQuest(_escapeQuest);
    }
}
