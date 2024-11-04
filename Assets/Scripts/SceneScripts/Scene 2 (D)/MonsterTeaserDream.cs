using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterTeaserDream : MonoBehaviour
{
    [SerializeField] private float _timeBeforeStartingSound;
    [SerializeField] private float _timeAfterStartingSound;
    [SerializeField] private EventReference _monsterSoundEventRef;
    [SerializeField, Scene] private string _scene3;

    private EventInstance _monsterSoundInstance;

    private void Start ()
    {
        StartCoroutine(Dream());
    }

    private IEnumerator Dream()
    {
        yield return new WaitForSeconds(_timeBeforeStartingSound);

        _monsterSoundInstance = AudioManager.Instance.PlayOneShotReturnInstance(_monsterSoundEventRef);

        yield return new WaitForSeconds(_timeAfterStartingSound);

        SceneManager.LoadScene(_scene3);
    }
}
