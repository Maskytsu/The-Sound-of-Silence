using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageTextBox : MonoBehaviour
{
    public ContactScriptable.Message Message;

    [SerializeField] private TextMeshProUGUI _messageTMP;
    [SerializeField] private Image _backgroundImage;

    [SerializeField] private Color _playersMessageBackgroundColor;
    [SerializeField] private Color _contactMessageBackgroundColor;
    [SerializeField] private Color _playersMessageTextColor;
    [SerializeField] private Color _contactMessageTextColor;

    private void Start()
    {
        _messageTMP.text = Message.Text;

        if (Message.IsPlayers)
        {
            _backgroundImage.color = _playersMessageBackgroundColor;
            _messageTMP.color = _playersMessageTextColor;
        }
        else
        {
            _backgroundImage.color = _contactMessageBackgroundColor;
            _messageTMP.color = _contactMessageTextColor;
        }
    }
}
