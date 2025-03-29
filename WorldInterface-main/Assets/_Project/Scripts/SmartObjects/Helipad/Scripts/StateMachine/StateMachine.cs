using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Serializable]
public class StateMachine
{
    [SerializeReference, SubclassSelector]
    public State[] States;
    [SerializeReference, SubclassSelector]
    public Transition[] Transitions;

    public int DefaultState = 0;

    private int _currentState = 0;

    private CancellationToken StateMachineCancellationToken;

    public StateMachine Clone()
    {
        var states = new State[States.Length];

        for (var i = 0; i < States.Length; i++)
        {
            states[i] = States[i].Clone();
        }

        var transitions = new Transition[Transitions.Length];
        for (var i = 0; i < Transitions.Length; i++)
        {
            transitions[i] = Transitions[i].Clone();
        }

        return new StateMachine { States = states, Transitions = transitions };
    }

    public async UniTask Init(GameObject target, CancellationToken cancellationToken)
    {
        StateMachineCancellationToken = cancellationToken;
        StateMachineCancellationToken.ThrowIfCancellationRequested();

        _currentState = DefaultState;
        await States[_currentState].OnEnter(target);
        await Update(target);
    }

    public async UniTask Init(GameObject target)
    {        
        _currentState = DefaultState;
        await States[_currentState].OnEnter(target);
        await Update(target);
    }

    public async UniTask Update(GameObject target)
    {
        while (true)
        {
            StateMachineCancellationToken.ThrowIfCancellationRequested();

            Debug.Log("State: " + _currentState + " - " + States[_currentState].GetType());
            if (ShouldExecuteTransition(target, out var transition))
            {
                await PerformTransition(transition, target);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            Debug.Log("Transition: " + transition + " - " + Transitions[transition].GetType());

            States[_currentState].OnUpdate(target);
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }

    private bool ShouldExecuteTransition(GameObject target, out int selectedTransition)
    {
        StateMachineCancellationToken.ThrowIfCancellationRequested();

        selectedTransition = 0;
        var possibleTransitions = Transitions.Where(x => x.Start == _currentState && x.ShouldTransition(this, target)).ToList();

        if (!possibleTransitions.Any())
        {
            return false;
        }

        possibleTransitions.Sort((x, y) => x.Priority.CompareTo(y.Priority));
        selectedTransition = Array.IndexOf(Transitions, possibleTransitions.First());
        return true;
    }

    private async UniTask PerformTransition(int transition, GameObject target)
    {
        StateMachineCancellationToken.ThrowIfCancellationRequested();

        await States[_currentState].OnExit(target);
        await Transitions[transition].OnTransition();
        _currentState = Transitions[transition].End;
        await States[_currentState].OnEnter(target);
    }
}