using System;
using UnityEngine;

public abstract class SaveData<T>
{
    protected string _name;
    protected private Func<T> _getter;
    protected private Action<T> _setter;

    public SaveData(string name, Func<T> getter, Action<T> setter)
    {
        _name = name;
        _getter = getter;
        _setter = setter;
    }

    public abstract void SaveValue();
    public abstract void LoadValue();
    public abstract void ClearSavedValue();
}

public class BoolSaveData: SaveData<bool>
{
    public BoolSaveData(string name, Func<bool> getter, Action<bool> setter) : base(name, getter, setter) { }
    public override void SaveValue() => PlayerPrefs.SetInt(_name, _getter() ? 1 : 0);
    public override void LoadValue() => _setter(PlayerPrefs.GetInt(_name) == 1);
    public override void ClearSavedValue() => PlayerPrefs.SetInt(_name, 0);
}

public class StringSaveData : SaveData<string>
{
    public StringSaveData(string name, Func<string> getter, Action<string> setter) : base(name, getter, setter) { }
    public override void SaveValue() => PlayerPrefs.SetString(_name, _getter());
    public override void LoadValue() => _setter(PlayerPrefs.GetString(_name));
    public override void ClearSavedValue() => PlayerPrefs.SetString(_name, "");
}

public class FloatSaveData : SaveData<float>
{
    public FloatSaveData(string name, Func<float> getter, Action<float> setter) : base(name, getter, setter) { }
    public override void SaveValue() => PlayerPrefs.SetFloat(_name, _getter());
    public override void LoadValue() => _setter(PlayerPrefs.GetFloat(_name));
    public override void ClearSavedValue() => PlayerPrefs.SetFloat(_name, 0.0f);
}

public class IntSaveData : SaveData<int>
{
    public IntSaveData(string name, Func<int> getter, Action<int> setter) : base(name, getter, setter) { }
    public override void SaveValue() => PlayerPrefs.SetInt(_name, _getter());
    public override void LoadValue() => _setter(PlayerPrefs.GetInt(_name));
    public override void ClearSavedValue() => PlayerPrefs.SetInt(_name, 0);
}