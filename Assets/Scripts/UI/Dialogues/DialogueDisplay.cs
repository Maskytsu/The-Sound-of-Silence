using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueDisplay : MonoBehaviour
{
    public DialogueSequenceScriptable DialogueSequence;

    [SerializeField] private TextMeshProUGUI _dialogueTMP;

    private IEnumerator Start()
    {
        foreach (var dialogueLine in DialogueSequence.DialogueLines)
        {
            _dialogueTMP.text = dialogueLine.Text;
            _dialogueTMP.color = dialogueLine.TextColor;
            yield return new WaitForSeconds(dialogueLine.DisplayTime);
        }

        Destroy(gameObject);
    }
}
