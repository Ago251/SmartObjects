using UnityEngine;
using WorldInterface.SmartObject;

[System.Serializable]
public class HasTargetTransition : Transition
{
    [SerializeField] private FiretruckTargetType _targetType;
    [SerializeField] private bool negate = false;

    public override bool ShouldTransition(StateMachine stateMachine, GameObject target)
    {
        if(target.TryGetComponent(out FiretruckTargetDatabase firetruckTargetDatabase))
        {
            if(_targetType == FiretruckTargetType.Hydrant)
            {
                return negate ^ firetruckTargetDatabase.hydrantTarget != null;
            }
            else if(_targetType == FiretruckTargetType.Fire)
            {
                return negate ^ firetruckTargetDatabase.fireplaceTarget != null;
            }
        }

        return false;
    }

    public override void OnClone(ref Transition newObject)
    {
        base.OnClone(ref newObject);
        ((HasTargetTransition)newObject)._targetType = _targetType;
        ((HasTargetTransition)newObject).negate = negate;
    }
}
