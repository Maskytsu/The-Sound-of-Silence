using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorNote : MonoBehaviour
{
    [SerializeField, ResizableTextArea] private string _note;
}
