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
    [SerializeField] private Trigger _playerTutorialTrigger;
    [SerializeField] private GameObject _monsterOutside;

    private GameObject _spawnedCrouchTutorial;

    private void Start()
    {
        _hearingAid.OnInteract += () => StartCoroutine(BeginCheckDoorQuest());

        _playerTutorialTrigger.OnObjectTriggerEnter += DisplayCrouchTutorial;

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
        AudioManager.Instance.PlayOneShotOccludedRI(FmodEvents.Instance.OCC_Knocking, _doorSoundPoint);
        yield return new WaitForSeconds(AudioManager.Instance.EventLength(FmodEvents.Instance.OCC_Knocking) + 1f);

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
        _playerTutorialTrigger.gameObject.SetActive(false);
        _spawnedCrouchTutorial = Instantiate(_crouchTutorial);
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