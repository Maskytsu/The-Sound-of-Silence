using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadNewspaperQuest : MonoBehaviour
{
    [SerializeField] private Note _newspaper;

    private void Start()
    {
        _newspaper.OnInteract += () => GameState.Instance.ReadNewspaper = true;
    }
}
