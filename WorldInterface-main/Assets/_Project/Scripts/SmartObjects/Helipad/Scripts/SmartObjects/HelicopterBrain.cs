using Cysharp.Threading.Tasks;
using UnityEngine;

public class HelicopterBrain : MonoBehaviour
{
    public StateMachineSchema Schema;

    private StateMachine _current;

    private void Awake()
    {
        _current = Schema.StateMachine.Clone();
    }

    public UniTask InitBrain()
    {
        return _current.Init(gameObject);
    }
}
