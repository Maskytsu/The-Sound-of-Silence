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
    [SerializeField, Scene] private string _scene3;

    private void Start ()
    {
        StartCoroutine(Dream());
    }

    private IEnumerator Dream()
    {
        yield return new WaitForSeconds(_timeBeforeStartingSound);

        RuntimeManager.PlayOneShot(FmodEvents.Instance.MonsterTeaserDream);

        yield return new WaitForSeconds(_timeAfterStartingSound);

        SceneManager.LoadScene(_scene3);
    }
}
