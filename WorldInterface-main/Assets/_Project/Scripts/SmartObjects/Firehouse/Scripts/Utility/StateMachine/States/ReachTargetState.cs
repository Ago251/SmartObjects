using System;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using WorldInterface.SmartObject;


[Serializable]
public class ReachTargetState : State
{
    private GameObject _targetFiretruck = null;
    [SerializeField] private FiretruckTargetType _targetType;
    [SerializeField] private float _stopDistance;
    private NavMeshAgent _navMeshAgent;

    public override UniTask OnEnter(GameObject target)
    {
        if(target.TryGetComponent(out FiretruckTargetDatabase firetruckTargetDatabase))
        {
            switch(_targetType)
            {
                case FiretruckTargetType.Hydrant:
                    _targetFiretruck = firetruckTargetDatabase.hydrantTarget;
                    break;
                case FiretruckTargetType.Fire:
                    _targetFiretruck = firetruckTargetDatabase.fireplaceTarget;
                    break;
                case FiretruckTargetType.FireHouse:
                    _targetFiretruck = firetruckTargetDatabase._fireHouse;
                    break;
            }
        }

        if(_targetFiretruck == null) 
        {
            IsEnded = true;
            return UniTask.CompletedTask;
        }
        else if(target.TryGetComponent(out NavMeshAgent navMeshAgent))
        {
            _navMeshAgent = navMeshAgent;
            navMeshAgent.SetDestination(_targetFiretruck.transform.position);
        }

        return UniTask.CompletedTask;
    }

    public override void OnUpdate(GameObject target)
    {
        if(_targetFiretruck == null)
        {
            IsEnded = true;
            return;
        }

        if(math.distance(target.transform.position, _targetFiretruck.transform.position) <=_stopDistance)
        {
            IsEnded = true;
            return;
        }
    }

    public override void OnClone(ref State newObject)
    {
        base.OnClone(ref newObject);
        ((ReachTargetState)newObject)._targetType = _targetType;
        ((ReachTargetState)newObject)._stopDistance = _stopDistance;
    }
}
