using TMPro;
using UnityEngine;

public class DialogueDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _textBox;
    [SerializeField] private TextMeshProUGUI _dialogueTMP;

    public void SetActiveTextBox(bool active)
    {
        _textBox.SetActive(active);
    }

    public void SetCurrentLine(DialogueSequenceScriptable.DialogueLine dialogueLine)
    {
        _dialogueTMP.text = dialogueLine.Text;
        _dialogueTMP.color = dialogueLine.TextColor;
    }
}