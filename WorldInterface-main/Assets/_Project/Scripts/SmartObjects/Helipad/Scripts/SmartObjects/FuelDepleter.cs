using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StatTracker))]
public class FuelDepleter : MonoBehaviour
{
    private Stat _fuel;

    public float DecreaseStepTime = 1;

    private float _currentAwaitedTime;

    private void Start()
    {
        _fuel = GetComponent<StatTracker>().GetStatByType(StatType.Fuel);
        _currentAwaitedTime = 0;
    }

    private void Update()
    {
        _currentAwaitedTime += Time.deltaTime;
        if (_currentAwaitedTime >= DecreaseStepTime)
        {
            _fuel.Decrease(_fuel.Decay);
            _currentAwaitedTime = 0;
        }
    }
}
