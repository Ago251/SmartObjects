using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using WorldInterface.SmartObject;

[Serializable]
public class RepairBuildingState : State
{
    public override async UniTask OnEnter(GameObject target)
    {
        if (target.TryGetComponent(out BuildingTruckSmartObject buildingTruckSmartObject))
        {
            await buildingTruckSmartObject.BuildingRepair(target);

            if(target.TryGetComponent(out BuildingComponents buildingComponents))
            {
                buildingComponents.damagedBuilding = null;
            }
            IsEnded = true;
        }
        await UniTask.CompletedTask;
    }
}
