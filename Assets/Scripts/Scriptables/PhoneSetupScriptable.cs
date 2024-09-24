using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "PhoneSetupScriptable", menuName = "ScriptableObjects/PhoneSetupScriptable")]
public class PhoneSetupScriptable : ScriptableObject
{
    public List<PhoneContact> Contacts;

    [System.Serializable]
    public class PhoneContact
    {
        public Texture Picture;
        public string Name;
        public List<Message> Messages;
    }

    [System.Serializable]
    public class Message
    {
        public bool IsPlayers;
        public string Text;
    }
}
