using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[Serializable]
public class MoveToRandomPositionState : State
{
    public float MinRange = 2f;
    public float MaxRange = 10f;
    public float AcceptanceRadius = .5f;

    public override async UniTask OnEnter(GameObject target)
    {
        await base.OnEnter(target);
        var randomDirection = Random.insideUnitCircle;
        var randomPosition = target.transform.position + new Vector3(randomDirection.x, 0, randomDirection.y) * Random.Range(MinRange, MaxRange);

        if (target.TryGetComponent(out NavMeshAgent navMeshAgent))
        {
            navMeshAgent.SetDestination(randomPosition);
            navMeshAgent.isStopped = false;
        }
    }

    public override void OnUpdate(GameObject target)
    {
        base.OnUpdate(target);

        if (IsEnded)
        {
            return;
        }

        if (!target.TryGetComponent(out NavMeshAgent navMeshAgent))
        {
            return;
        }

        if (!(navMeshAgent.remainingDistance <= AcceptanceRadius))
        {
            return;
        }

        IsEnded = true;
        navMeshAgent.isStopped = true;
    }

    public override async UniTask OnExit(GameObject target)
    {
        await base.OnExit(target);
        if (target.TryGetComponent(out NavMeshAgent navMeshAgent))
        {
            navMeshAgent.isStopped = true;
        }
    }

    public override void OnClone(ref State newObject)
    {
        base.OnClone(ref newObject);
        ((MoveToRandomPositionState)newObject).MinRange = MinRange;
        ((MoveToRandomPositionState)newObject).MaxRange = MaxRange;
        ((MoveToRandomPositionState)newObject).AcceptanceRadius = AcceptanceRadius;
    }
}