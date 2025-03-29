using System;
using UnityEngine;

[Serializable]
public class HeightReachedTransition : Transition
{
    public float MaxHeight;

    public override bool ShouldTransition(StateMachine stateMachine, GameObject target)
    {
        //Gets helicopter's model current height 
        float currentHeight = target.transform.GetChild(0).position.y;

        return currentHeight >= MaxHeight;
    }

    public override void OnClone(ref Transition newObject)
    {
        base.OnClone(ref newObject);
        ((HeightReachedTransition)newObject).MaxHeight = MaxHeight;
    }
}
