using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ContactScriptable", menuName = "ScriptableObjects/ContactScriptable")]
public class ContactScriptable : ScriptableObject
{
    public Texture Picture;
    public string Name;
    public bool isCallable;
    public List<Message> Messages;

    public Action OnCall;

    public void Call()
    {
        Debug.Log("Called to " + Name);
        OnCall?.Invoke();
    }

    [Serializable]
    public class Message
    {
        public bool IsPlayers;
        public string Text;
    }
}
