using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WorldInterface.SmartObject;

[Serializable]
public class ExtinguishFireState : State
{
    public override async UniTask OnEnter(GameObject target)
    {
        if(target.TryGetComponent(out FireTruckSmartObject fireTruckSmartObject))
        {
            await fireTruckSmartObject.ShootWater();

            if(target.TryGetComponent(out FiretruckTargetDatabase firetruckTargetDatabase))
            {
                firetruckTargetDatabase.fireplaceTarget = null;
            }

            IsEnded = true;
        }

        await UniTask.CompletedTask;
    }
}
