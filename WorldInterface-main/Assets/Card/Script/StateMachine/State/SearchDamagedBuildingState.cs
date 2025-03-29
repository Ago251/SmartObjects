using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

[Serializable]
public class SearchDamagedBuildingState : State
{
    [SerializeField] private float _detectionRange = 20f;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Components _components;
    [SerializeField] private GameObject truckTarget = null;

    public override void OnUpdate(GameObject target)
    {
        base.OnUpdate(target);
        var colliders = Physics.OverlapSphere(
            target.transform.position, 
            _detectionRange,
            _layerMask);
        float minDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            var distance = Vector3.Distance(collider.transform.position, target.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                truckTarget = collider.gameObject;
            }
        }
        if (truckTarget != null)
        {
            IsEnded = true;
            return;
        }
    }
    public override UniTask OnExit(GameObject target)
    {
        if (target.TryGetComponent(out BuildingComponents buildingComponents))
        {
            if (_components == Components.Warehouse)
            {
                buildingComponents.warehouse = truckTarget;
            }
            else if (_components == Components.DamagedBuilding)
            {
                buildingComponents.damagedBuilding = truckTarget;
            }
        }
        return UniTask.CompletedTask;
    }

    public override void OnClone(ref State newObject)
    {
        base.OnClone(ref newObject);
        ((SearchDamagedBuildingState)newObject)._components = _components;
        ((SearchDamagedBuildingState)newObject)._layerMask = _layerMask;
        ((SearchDamagedBuildingState)newObject)._detectionRange = _detectionRange;
    }
}
