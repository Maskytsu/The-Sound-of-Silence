using TMPro;
using UnityEngine;

public class EndingHintButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _buttonText;

    HintSetup _hintSetup;
    EndingHintsMenu _parentMenu;

    public void Setup(EndingHintsMenu parentMenu, HintSetup hintSetup)
    {
        _parentMenu = parentMenu;
        _hintSetup = hintSetup;
        _buttonText.text = hintSetup.buttonText;
    }

    public void ShowHint()
    {
        StartCoroutine(_parentMenu.ShowHintRoutine(_hintSetup.hintText));
    }
}