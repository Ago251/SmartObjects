using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public abstract class Transition
{
    public int Start;
    public int End;
    public int Priority = 0;

    public abstract bool ShouldTransition(StateMachine stateMachine, GameObject target);

    public virtual UniTask OnTransition()
    {
        return UniTask.CompletedTask;
    }

    public Transition Clone()
    {
        var result = Activator.CreateInstance(GetType()) as Transition;
        OnClone(ref result);
        return result;
    }

    public virtual void OnClone(ref Transition newObject)
    {
        newObject.Start = Start;
        newObject.End = End;
        newObject.Priority = Priority;
    }
}