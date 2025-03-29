using System;
using Unity.VisualScripting;
using UnityEngine;
using WorldInterface.SmartObject;

[Serializable]
public class HasEnoughMaterials : Transition
{
    [SerializeField] private bool negate = false;
    public override bool ShouldTransition(StateMachine stateMachine, GameObject target)
    {
        if (target.TryGetComponent(out BuildingTruckSmartObject buildingTruckSmartObject))
        {
            var transition = !buildingTruckSmartObject.HasEnoughMaterial();
            return transition;
        }
        return false;
    }
    public override void OnClone(ref Transition newObject)
    {
        base.OnClone(ref newObject);
        ((HasEnoughMaterials)newObject).negate = negate;
    }
}
