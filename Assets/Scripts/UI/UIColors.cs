using System;
using System.Collections.Generic;
using UnityEngine;

public class UIColors : SingletonMonobehaviour<UIColors>
{
    public Color InteractableOutline = Color.white;
    public Color UnlockableOutline = Color.white;
    public Color HiddenQuestText = Color.white;
    [Space]
    [Header("Dialogue")]
    [SerializeField] private Color Placeholder = Color.white;
    [SerializeField] private Color Sharon = Color.white;
    [SerializeField] private Color TalkShowTV = Color.white;
    [SerializeField] private Color Neighbour = Color.white;
    [SerializeField] private Color Phone = Color.white;
    [SerializeField] private Color Police = Color.white;
    [SerializeField] private Color Claire = Color.white;
    [SerializeField] private Color Harry = Color.white;
    [SerializeField] private Color Chris = Color.white;
    [Space]
    [Header("Holder")]
    [SerializeField] private List<ColorHolder> Colors = new();

    public Color GetDialogueColor(DialogueColorType colorType)
    {
        switch (colorType)
        {
            case DialogueColorType.Placeholder:
                return Placeholder;
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

    [Serializable]
    private class ColorHolder
    {
        public string Name;
        public Color Color;
    }
}


public enum DialogueColorType
{
    Placeholder = 0,
    Sharon = 1,
    TalkShowTV = 2,
    Neighbour = 3,
    Phone = 4,
    Police = 5,
    Claire = 6,
    Harry = 7,
    Chris = 8
}