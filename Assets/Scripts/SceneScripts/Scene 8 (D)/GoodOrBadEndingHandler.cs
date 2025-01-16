using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodOrBadEndingHandler : MonoBehaviour
{
    public string NextScene { get; private set; }

    [Header("Good Ending")]
    [SerializeField] private GameObject _lightGreen;
    [Scene, SerializeField] private string _goodEndingScene;
    [Space]
    [Header("Bad Ending")]
    [SerializeField] private GameObject _lightRed;
    [Scene, SerializeField] private string _badEndingScene;

    private void Start()
    {
        ManageEndings();
    }

    private void ManageEndings()
    {
        bool claireContactedBothWays = GameState.Instance.ClaireCalled && GameState.Instance.ClaireMessaged;
        bool policeContacted = GameState.Instance.PoliceCalled;
        bool tookPills = GameState.Instance.TookPills;
        bool readNewspaper = GameState.Instance.ReadNewspaper;

        if (tookPills && (policeContacted || claireContactedBothWays) && readNewspaper)
        {
            Debug.Log("Good Ending!");

            NextScene = _goodEndingScene;
            _lightGreen.SetActive(true);
            _lightRed.SetActive(false);
        }
        else
        {
            Debug.Log("Bad Ending!");

            NextScene = _badEndingScene;
            _lightGreen.SetActive(false);
            _lightRed.SetActive(true);
        }
    }
}
