using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Contact", menuName = "ScriptableObjects/Contact")]
public class ContactScriptable : ScriptableObject
{
    public Texture Picture;
    public string Name;
    public bool isCallable;
    public bool isMessageable;
    [TextArea(2, 4)]
    public string MessageToSend = "";
    public List<Message> Messages;

    public Action OnCall;
    public Action OnSendMessage;

    public void Call()
    {
        Debug.Log("Called to " + Name);
        OnCall?.Invoke();
    }

    public void SendMessage()
    {
        Debug.Log("Message sent to " + Name);
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
