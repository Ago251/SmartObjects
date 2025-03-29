using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class WaitState : State
{
    public float WaitTime = 1;

    private float _currentAwaitedTime;

    public override async UniTask OnEnter(GameObject target)
    {
        await base.OnEnter(target);
        _currentAwaitedTime = 0;
    }

    public override void OnUpdate(GameObject target)
    {
        if (IsEnded)
        {
            return;
        }

        _currentAwaitedTime += Time.deltaTime;
        if (_currentAwaitedTime >= WaitTime)
        {
            IsEnded = true;
        }
    }

    public override void OnClone(ref State newObject)
    {
        base.OnClone(ref newObject);
        ((WaitState)newObject).WaitTime = WaitTime;
    }
}