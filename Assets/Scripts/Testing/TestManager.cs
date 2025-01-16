using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviour
{
    [SerializeField, Scene] private string _chosenScene;

    [Button]
    private void ChangeSceneWithoutSaving()
    {
        SceneManager.LoadScene(_chosenScene);
    }

    [Button]
    private void ChangeSceneToWithSaving()
    {
        GameManager.Instance.LoadSceneAndSaveGameState(_chosenScene);
    }
}
