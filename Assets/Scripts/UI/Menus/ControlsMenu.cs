using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    [SerializeField] private GameObject _noLeapControlsMenu;
    [SerializeField] private GameObject _fullControlsMenu;

    private void OnEnable()
    {
        var isLeapUnlocked = GameState.Instance.LeapUnlocked;
        _noLeapControlsMenu.SetActive(!isLeapUnlocked);
        _fullControlsMenu.SetActive(isLeapUnlocked);
    }
}