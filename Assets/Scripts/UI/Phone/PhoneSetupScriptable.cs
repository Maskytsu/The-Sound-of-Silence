using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "PhoneSetupScriptable", menuName = "ScriptableObjects/PhoneSetupScriptable")]
public class PhoneSetupScriptable : ScriptableObject
{
    public List<ContactScriptable> Contacts;
}
