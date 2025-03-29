using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WorldInterface.SmartObject;

[Serializable]
public class RechargeWaterState : State
{
    public override async UniTask OnEnter(GameObject target)
    {
        if(target.TryGetComponent(out FireTruckSmartObject fireTruckSmartObject))
        {
            await fireTruckSmartObject.RechargeWater();
        }

        await UniTask.CompletedTask;
    }
}
