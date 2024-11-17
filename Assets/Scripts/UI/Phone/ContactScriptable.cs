using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Contact", menuName = "ScriptableObjects/Contact")]
public class ContactScriptable : ScriptableObject
{
    public Texture Picture;
    public string Name;
    public bool IsNew;
    public bool IsCallable;
    public List<Message> Messages;
    public bool IsMessageable;
    [ShowIf(nameof(IsMessageable))] public Message MessageToSend;

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

    public void ClearSubscribers()
    {
        OnCheckNew = null;
        OnCall = null;
        OnSendMessage = null;
    }

    public bool IsOnCallNull()
    {
        return OnCall == null;
    }

    [Serializable]
    public class Message
    {
        public bool IsPlayers;
        [TextArea(2, 4)]
        public string Text;
    }
}
