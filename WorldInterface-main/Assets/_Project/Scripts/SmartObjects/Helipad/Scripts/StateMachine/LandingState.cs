using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class LandingState : State
{
    public float LandingSpeed;

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

        if (IsEnded)
        {
            return;
        }

        if (_model.position.y <= 0)
        {
            if (target.TryGetComponent(out HelicopterSmartObject helicopterSmartObject))
            {
                helicopterSmartObject.HasLanded = true;
                helicopterSmartObject.CanDisembark = true;
            }

            IsEnded = true;
            return;
        }

        _model.position -= Vector3.up * LandingSpeed * Time.deltaTime;
    }

    public override void OnClone(ref State newObject)
    {
        base.OnClone(ref newObject);
        ((LandingState)newObject).LandingSpeed = LandingSpeed;
    }
}
