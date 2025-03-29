using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using WorldInterface.SmartObject;

[Serializable]
public class SearchTargetState : State
{
    private GameObject _targetFiretruck = null;
    [SerializeField] private float _detectionRange = 15f;
    [SerializeField] private FiretruckTargetType _targetType;
    [SerializeField] private LayerMask _detectionMask;

    public override void OnUpdate(GameObject target)
    {
        var colliders = Physics.OverlapSphere(target.transform.position, 
                                            _detectionRange, 
                                            _detectionMask);
                
        float minDistance = float.MaxValue;
        foreach (var collider in colliders)
        {
            var distance = 
                Vector3.Distance(collider.transform.position, 
                target.transform.position);
                
            if (distance < minDistance)
            {
                minDistance = distance;
                _targetFiretruck = collider.gameObject;
            }
        }

        if (_targetFiretruck != null)
        {
            Debug.Log("Target found: " + _targetFiretruck.name);
            IsEnded = true;
            return;
        }
    }

    public override UniTask OnExit(GameObject target)
    {
        if(target.TryGetComponent(out FiretruckTargetDatabase firetruckTargetDatabase))
        {
            if(_targetType == FiretruckTargetType.Hydrant)
            {
                firetruckTargetDatabase.hydrantTarget = _targetFiretruck;
            }
            else
            {
                firetruckTargetDatabase.fireplaceTarget = _targetFiretruck;
            }
        }
        
        return UniTask.CompletedTask;
    }

    public override void OnClone(ref State newObject)
    {
        base.OnClone(ref newObject);
        ((SearchTargetState)newObject)._targetType = _targetType;
        ((SearchTargetState)newObject)._detectionRange = _detectionRange;
        ((SearchTargetState)newObject)._detectionMask = _detectionMask;
    }
}
