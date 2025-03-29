using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldInterface;
using WorldInterface.SmartObject;

public class HelicopterRefuelStationSmartObject : SmartObject
{
    private GameObject _currentAgent;
    [SerializeField] private HelicopterSmartObject _elicopter;

    [SerializeField]
    private int _refuelAmount;

    public override bool CanBeUsed(GameObject agent)
    {
        if (_currentAgent != null)
        {
            return false;
        }

        if (!agent.TryGetComponent(out HandController handController))
        {
            return false;
        }

        if (handController.GetItemAmount(HandItem.Fuel) <= 0)
        {
            return false;
        }

        return base.CanBeUsed(agent);
    }

    public override async UniTask Activate(GameObject agent)
    {
        if (!agent.TryGetComponent(out HandController handController))
        {
            return;
        }

        _currentAgent = agent;

        while (handController.GetItemAmount(HandItem.Fuel) > 0 && _elicopter.CanBeRefueled())
        {
            _elicopter.Refuel(_refuelAmount);
            handController.RemoveItem(HandItem.Fuel, 1);
            await UniTask.WaitForSeconds(.5f);
        }

        _currentAgent = null;
    }    
}
