using System;
using UnityEngine;

[Serializable]
public class HasBuildingTargetTransition : Transition
{
    [SerializeField] private Components _components;
    [SerializeField] private bool _negate = false;
    public override bool ShouldTransition(StateMachine stateMachine, GameObject target)
    {
        if (target.TryGetComponent(out BuildingComponents buildingComponents))
        {
            if (_components == Components.Warehouse)
            {
                return _negate ^ buildingComponents.warehouse != null;
            }
            else if (_components == Components.DamagedBuilding)
            {
                return _negate ^ buildingComponents.damagedBuilding != null;
            }
        }
        return false;
    }
    public override void OnClone(ref Transition newObject)
    {
        base.OnClone(ref newObject);
        ((HasBuildingTargetTransition)newObject)._components = _components;
        ((HasBuildingTargetTransition)newObject)._negate = _negate;
    }
}
