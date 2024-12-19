using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterSwappingStairsAnimation : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Blackout _blackoutPrefab;
    [Header("Scene Objects")]
    [SerializeField] private Trigger _playerAnimationTrigger;
    [SerializeField] private StormEffect _storm;
    [SerializeField] private GameObject _monster;
    [SerializeField] private Door _shedDoor;
    [SerializeField] private GameObject _goingUpBlockade;
    [SerializeField] private List<GameObject> _objectsForFakeStairs;
    [SerializeField] private List<GameObject> _objectsForWalkableStairs;

    private bool _shouldCheckPlayerRotTowardsDown = false;
    private bool _shouldCheckPlayerRotTowardsUp = false;

    private void Start()
    {
        _playerAnimationTrigger.OnObjectTriggerEnter += StartCheckingForAnimation;
    }

    private void Update()
    {
        if (_shouldCheckPlayerRotTowardsDown) CheckPlayerRotTowardsDown();
        if (_shouldCheckPlayerRotTowardsUp) CheckPlayerRotTowardsUp();
    }

    public void SkipAnimation()
    {
        _playerAnimationTrigger.gameObject.SetActive(false);

        SetActiveObjects(_objectsForFakeStairs, false);
        SetActiveObjects(_objectsForWalkableStairs, true);
    }

    private void StartCheckingForAnimation()
    {
        _playerAnimationTrigger.gameObject.SetActive(false);

        _goingUpBlockade.SetActive(true);

        _shouldCheckPlayerRotTowardsDown = true;
    }

    private void CheckPlayerRotTowardsDown()
    {
        Transform playerTransform = PlayerObjects.Instance.Player.transform;

        if (playerTransform.eulerAngles.y > 180)
        {
            _shouldCheckPlayerRotTowardsDown = false;

            _monster.SetActive(true);
            _shouldCheckPlayerRotTowardsUp = true;
        }
    }

    private void CheckPlayerRotTowardsUp()
    {
        Transform playerTransform = PlayerObjects.Instance.Player.transform;

        if (playerTransform.eulerAngles.y < 180)
        {
            _shouldCheckPlayerRotTowardsUp = false;

            StartCoroutine(MonsterDoorLightningAnimation());
        }
    }

    private IEnumerator MonsterDoorLightningAnimation()
    {
        yield return new WaitForSeconds(2.5f);

        yield return StartCoroutine(_storm.LightningEffect(0.3f));

        Blackout blackout = Instantiate(_blackoutPrefab);
        blackout.GetComponent<Canvas>().sortingOrder = -1;
        blackout.SetAlphaToZero();
        Tween fadeInTween = blackout.Image.DOFade(1f, 0.2f);
        while (fadeInTween.IsPlaying()) yield return null;

        yield return new WaitForSeconds(0.1f);

        _monster.SetActive(false);
        _goingUpBlockade.SetActive(false);

        SetActiveObjects(_objectsForFakeStairs, false);
        SetActiveObjects(_objectsForWalkableStairs, true);

        _shedDoor.InteractionHitbox.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        Tween fadeOutTween = blackout.Image.DOFade(0f, 0.2f);
        while (fadeOutTween.IsPlaying()) yield return null;
        Destroy(blackout.gameObject);

        yield return new WaitForSeconds(1f);

        if (_shedDoor.IsOpened) _shedDoor.SwitchDoorAnimated();
    }


    private void SetActiveObjects(List<GameObject> gObjects, bool activeStateToSet)
    {
        foreach (GameObject gObject in gObjects)
        {
            gObject.SetActive(activeStateToSet);
        }
    }


    //TESTING--------------------------------------------------------------------

    [HorizontalLine]
    [Header("--TESTING--\n" +
        "For the button to work properly set this on Scene7ResetHandler:" +
        "\n- AutoReset = true, " +
        "\n- SceneWasReseted = true, " +
        "\n- SafeRoomReached = true")]
    [SerializeField] private PlayerTargetTransform _testingPTT;
    [SerializeField] private MonsterStateMachine _monsterSM;
    [SerializeField] private PerishingMonsterState _perishingState;

    [Button]
    private void TestingSequence()
    {
        if (_monsterSM == null) Debug.LogWarning("Monster is null. Was it killed?");
        else _monsterSM.ChangeState(_perishingState);

        _storm.gameObject.SetActive(true);

        PlayerObjects.Instance.Player.transform.position = _testingPTT.Position;
        PlayerObjects.Instance.PlayerMovement.RotateCharacter(_testingPTT.Rotation);

        ItemManager.Instance.ItemsPerType[ItemType.KEYS].PlayerHasIt = true;
        GameState.Instance.TookKeys = true;
    }
}
