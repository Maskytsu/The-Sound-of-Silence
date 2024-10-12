using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Contact", menuName = "ScriptableObjects/Contact")]
public class ContactScriptable : ScriptableObject
{
    public Texture Picture;
    public string Name;
    public bool isNew;
    [HorizontalLine(color: EColor.Gray)]
    public bool isCallable;
    [HorizontalLine(color: EColor.Gray)]
    public List<Message> Messages;
    [HorizontalLine(color: EColor.Gray)]
    public bool isMessageable;
    public Message MessageToSend;

    public event Action OnCheckNew;
    public event Action OnCall;
    public event Action OnSendMessage;

    public void CheckNew()
    {
        Debug.Log(Name + " checked.");
        OnCheckNew?.Invoke();
    }

    public void Call()
    {
        Debug.Log(Name + " called.");
        OnCall?.Invoke();
    }

    public void SendMessage()
    {
        Debug.Log(Name + " messaged.");
        OnSendMessage?.Invoke();
    }

    [Serializable]
    public class Message
    {
        public bool IsPlayers;
        [TextArea(2, 4)]
        public string Text;
    }
}
