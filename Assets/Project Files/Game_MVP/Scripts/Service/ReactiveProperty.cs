using System;
using UnityEngine;

[Serializable]
public class ReactiveProperty<T>
{
    [SerializeField] private T _value;
    private Action<T> _onChanged;

    public ReactiveProperty(T initialValue) 
    { 
        _value = initialValue; 
    }

    public T Value
    {
        get => _value;
        set
        {
            if ((_value == null && value != null) || (_value != null && !_value.Equals(value)))
            {
                _value = value;
                _onChanged?.Invoke(_value);
            }
        }
    }

    public void Subscribe(Action<T> callback)
    {
        _onChanged += callback;
        callback?.Invoke(_value); 
    }
        
    public void Unsubscribe(Action<T> callback) 
    { 
        _onChanged -= callback; 
    }
}