using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class DefaultTransition : Transition
{
    public override bool ShouldTransition(StateMachine stateMachine, GameObject target)
    {
        return stateMachine.States[Start].IsEnded;
    }
}