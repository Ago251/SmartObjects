using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class TakeOffState : State
{
    public float TakeOffSpeed;

    private Transform _model;

    public override async UniTask OnEnter(GameObject target)
    {
        await base.OnEnter(target);

        //Gets helicopter's model transform
        _model = target.transform.GetChild(0);
    }

    public override void OnUpdate(GameObject target)
    {
        base.OnUpdate(target);

        _model.position += Vector3.up * TakeOffSpeed * Time.deltaTime;
    }

    public override void OnClone(ref State newObject)
    {
        base.OnClone(ref newObject);
        ((TakeOffState)newObject).TakeOffSpeed = TakeOffSpeed;
    }
}
