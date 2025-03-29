using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class ReachTargetComponentState : State
{
    private GameObject truckTarget = null;
    [SerializeField] private float _distanceTollerance = 2f;
    [SerializeField] private Components _components;

    private NavMeshAgent _navMeshAgent;

    public override UniTask OnEnter(GameObject target)
    {
        if (target.TryGetComponent(out BuildingComponents buildingComponents))
        {
            switch (_components)
            {
                case Components.Warehouse:
                    {
                        truckTarget = buildingComponents.warehouse;
                        break;
                    }
                case Components.DamagedBuilding:
                    {
                        truckTarget = buildingComponents.damagedBuilding;
                        break;
                    }
            }
        }
        if (truckTarget == null)
        {
            IsEnded = true;
            return UniTask.CompletedTask;
        }
        else if (target.TryGetComponent(out NavMeshAgent navMeshAgent))
        {
            _navMeshAgent = navMeshAgent;
            navMeshAgent.SetDestination(truckTarget.transform.position);
        }
        return UniTask.CompletedTask;
    }

    public override void OnUpdate(GameObject target)
    {
        if (truckTarget == null)
        {
            IsEnded = true;
            return;
        }
        if(Vector3.Distance(target.transform.position, truckTarget.transform.position) <= _distanceTollerance)
        {
            IsEnded = true;
            return;
        }
    }
    public override void OnClone(ref State newObject)
    {
        base.OnClone(ref newObject);
        ((ReachTargetComponentState)newObject)._components = _components;
        ((ReachTargetComponentState)newObject)._distanceTollerance = _distanceTollerance;
    }
}
