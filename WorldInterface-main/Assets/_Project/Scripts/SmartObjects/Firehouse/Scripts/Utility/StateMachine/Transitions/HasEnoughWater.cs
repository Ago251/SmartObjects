using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldInterface.SmartObject;
using WorldInterface.SmartObjects;

[System.Serializable]
public class HasEnoughWater : Transition
{
    [SerializeField] private bool negate = false;
    public override bool ShouldTransition(StateMachine stateMachine, GameObject target)
    {
        if(target.TryGetComponent(out FireTruckSmartObject fireTruckSmartObject))
        {
            return negate ^ fireTruckSmartObject.IsWaterLevelOk();
        }

        return false;
    }

    public override void OnClone(ref Transition newObject)
    {
        base.OnClone(ref newObject);
        ((HasEnoughWater)newObject).negate = negate;
    }
}
