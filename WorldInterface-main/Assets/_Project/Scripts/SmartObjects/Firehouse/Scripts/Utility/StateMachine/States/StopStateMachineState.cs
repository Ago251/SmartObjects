using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WorldInterface.SmartObject;

[Serializable]
public class StopStateMachineState : State
{
    public override UniTask OnEnter(GameObject target)
    {
        if(target.TryGetComponent(out FireTruckSmartObject fireTruckSmartObject))
        {
            fireTruckSmartObject.StopStateMachine();
        }

        IsEnded = true;
        return UniTask.CompletedTask;
    }
}
