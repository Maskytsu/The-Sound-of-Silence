using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "PhoneSetup", menuName = "ScriptableObjects/PhoneSetup")]
public class PhoneSetupScriptable : ScriptableObject
{
    public List<ContactScriptable> Contacts;
}
