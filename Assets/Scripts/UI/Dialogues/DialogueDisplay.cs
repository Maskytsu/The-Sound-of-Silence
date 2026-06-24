using TMPro;
using UnityEngine;

public class DialogueDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _textBox;
    [SerializeField] private TextMeshProUGUI _dialogueTMP;
    [SerializeField] private DialogueColorSetup _colorSetup;

    public void SetActiveTextBox(bool active)
    {
        _textBox.SetActive(active);
    }

    public void SetCurrentLine(DialogueSequenceScriptable.DialogueLine dialogueLine)
    {
        _dialogueTMP.text = dialogueLine.Text;
        _dialogueTMP.color = _colorSetup.GetColor(dialogueLine.ColorType);
    }
}