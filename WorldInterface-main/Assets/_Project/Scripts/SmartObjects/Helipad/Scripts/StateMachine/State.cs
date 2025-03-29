using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public abstract class State
{
    public bool IsEnded { get; protected set; } = false;

    public virtual UniTask OnEnter(GameObject target)
    {
        IsEnded = false;
        return UniTask.CompletedTask;
    }
    public virtual void OnUpdate(GameObject target) { }

    public virtual UniTask OnExit(GameObject target)
    {
        return UniTask.CompletedTask;
    }

    public State Clone()
    {
        var result = Activator.CreateInstance(GetType()) as State;
        OnClone(ref result);
        return result;
    }

    public virtual void OnClone(ref State newObject)
    {
    }
}