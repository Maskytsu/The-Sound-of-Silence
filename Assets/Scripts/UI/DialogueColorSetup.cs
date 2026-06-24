using UnityEngine;

[CreateAssetMenu(fileName = "DialogueColorSetup", menuName = "ScriptableObjects/DialogueColorSetup")]
public class DialogueColorSetup : ScriptableObject
{
    [SerializeField] private Color Sharon;
    [SerializeField] private Color TalkShowTV;
    [SerializeField] private Color Neighbour;
    [SerializeField] private Color Phone;
    [SerializeField] private Color Police;
    [SerializeField] private Color Claire;
    [SerializeField] private Color Harry;
    [SerializeField] private Color Chris;

    public Color GetColor(DialogueColorType colorType)
    {
        switch (colorType)
        {
            case DialogueColorType.Sharon:
                return Sharon;
            case DialogueColorType.TalkShowTV:
                return TalkShowTV;
            case DialogueColorType.Neighbour:
                return Neighbour;
            case DialogueColorType.Phone:
                return Phone;
            case DialogueColorType.Police:
                return Police;
            case DialogueColorType.Claire:
                return Claire;
            case DialogueColorType.Harry:
                return Harry;
            case DialogueColorType.Chris:
                return Chris;
            default:
                return Color.red;
        }
    }
}

public enum DialogueColorType
{
    Sharon = 1,
    TalkShowTV = 2,
    Neighbour = 3,
    Phone = 4,
    Police = 5,
    Claire = 6,
    Harry = 7,
    Chris = 8
}