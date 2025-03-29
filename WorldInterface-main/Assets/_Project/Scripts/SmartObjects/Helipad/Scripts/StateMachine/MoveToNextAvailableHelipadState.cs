using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class MoveToNextAvailableHelipadState : State
{
    public float AcceptanceRadius = .5f;

    public override async UniTask OnEnter(GameObject target)
    {
        await base.OnEnter(target);

        Vector3 nextPosition = target.transform.position;

        if (target.TryGetComponent(out HelicopterSmartObject helicopter))
        {
            var helipadManager = helicopter.HelipadManager;

            //Frees previous helipad 
            var previousHelipad = helipadManager.FreeHelipadPosition(helicopter);

            if (helipadManager.AnyHelipadsAvailable())
            {
                //Gets next available helipad (different from previous helipad)
                var helipad = helipadManager.GetAvailableHelipad(helicopter, previousHelipad);
                
                if (helipad != null) 
                {
                    nextPosition = helipad.transform.position;
                }               
            }            
        }        

        if (target.TryGetComponent(out NavMeshAgent navMeshAgent))
        {
            navMeshAgent.SetDestination(nextPosition);
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

        //Skips current tick if agent's path is still pending
        if (navMeshAgent.pathPending)
        {
            return;
        }

        if(navMeshAgent.remainingDistance <= AcceptanceRadius)
        {
            IsEnded = true;
            navMeshAgent.isStopped = true;
        }        
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
        ((MoveToNextAvailableHelipadState)newObject).AcceptanceRadius = AcceptanceRadius;
    }
}
