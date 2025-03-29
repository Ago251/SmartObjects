using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using WorldInterface.SmartObject;

[Serializable]
public class RechargeBuildingComponentsState : State
{
    public override async UniTask OnEnter(GameObject target)
    {
        if(target.TryGetComponent(out BuildingTruckSmartObject buildingTruckSmartObject))
        {
            await buildingTruckSmartObject.RechargeBuildingComponents();
        }
        await UniTask.CompletedTask;
    }
}
