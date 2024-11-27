using FMODUnity;
using System.Collections;
using UnityEngine;

public class MonsterDoorQuestHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _crouchTutorial;
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _checkDoorQuest;
    [SerializeField] private QuestScriptable _escapeQuest;
    [Header("Scene Objects")]
    [SerializeField] private HearingAid _hearingAid;
    [SerializeField] private Note _note;
    [SerializeField] private Transform _doorSoundPoint;
    [SerializeField] private PlayerTrigger _playerTutorialTrigger;
    [SerializeField] private GameObject _monsterOutside;
    [Header("Parameters")]
    [SerializeField] private EventReference _knockingEventRef;

    private GameObject _spawnedCrouchTutorial;

    private void Start()
    {
        _hearingAid.OnInteract += () => StartCoroutine(BeginCheckDoorQuest());

        _playerTutorialTrigger.OnPlayerTriggerEnter += DisplayCrouchTutorial;

        _note.OnInteract += EndQuest;

        _note.OnFirstReadingEnd += () => QuestManager.Instance.StartQuest(_escapeQuest);
    }

    private void Update()
    {
        ManageDestroyingTutorial();
    }

    private IEnumerator BeginCheckDoorQuest()
    {
        _note.gameObject.SetActive(true);
        _playerTutorialTrigger.gameObject.SetActive(true);

        _monsterOutside.SetActive(true);
        AudioManager.Instance.PlayOneShotOccluded(_knockingEventRef, _doorSoundPoint);
        yield return new WaitForSeconds(AudioManager.Instance.EventLength(_knockingEventRef) + 1f);

        QuestManager.Instance.StartQuest(_checkDoorQuest);
    }

    private void EndQuest()
    {
        _note.OnInteract -= EndQuest;
        _monsterOutside.SetActive(false);
        QuestManager.Instance.EndQuest(_checkDoorQuest);
    }

    private void DisplayCrouchTutorial()
    {
        _spawnedCrouchTutorial = Instantiate(_crouchTutorial);
        _playerTutorialTrigger.gameObject.SetActive(false);
    }

    private void ManageDestroyingTutorial()
    {
        if (InputProvider.Instance.PlayerMovementMap.Crouch.ReadValue<float>() > 0)
        {
            if (_spawnedCrouchTutorial != null)
            {
                Destroy(_spawnedCrouchTutorial);
            }
        }
    }
}