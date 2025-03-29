using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using WorldInterface.SmartObject;

[Serializable]
public class StopStateMachine : State
{
    public override UniTask OnEnter(GameObject target)
    {
        if (target.TryGetComponent(out BuildingTruckSmartObject buildingTruckSmartObject))
        {
            buildingTruckSmartObject.StopStateMachine();
        }
        IsEnded = true;
        return UniTask.CompletedTask;
    }
}
