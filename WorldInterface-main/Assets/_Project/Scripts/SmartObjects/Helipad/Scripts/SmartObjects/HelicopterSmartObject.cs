using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using WorldInterface;
using WorldInterface.SmartObject;

public class HelicopterSmartObject : SmartObject
{
    [SerializeField]
    private Transform _agentExitPoint;

    [SerializeField]
    private HelicopterRefuelStationSmartObject _refuelStation;

    private HelicopterBrain _elicopterBrain;   

    private List<HelixMovement> _helixes;

    private StatTracker _statTracker;

    private FuelDepleter _fuelDepleter;

    private GameObject _currentAgent;

    private int _fuelAmount = 100;

    //Properties

    public HelipadManager HelipadManager { get; private set; }

    public bool CanDisembark { get; set; }

    public bool HasLanded { get; set; }


    private void Awake()
    {
        //Stat tracker and fuel amount setup
        _statTracker = GetComponent<StatTracker>();
        _fuelDepleter = GetComponent<FuelDepleter>();
        _fuelAmount = _statTracker.GetStatByType(StatType.Fuel).GetMaxLevel();
        _statTracker.GetStatByType(StatType.Fuel).SetCurrentLevel(_fuelAmount);
        _fuelDepleter.enabled = false;

        //Setting up references
        _elicopterBrain = GetComponent<HelicopterBrain>();

        _helixes = new List<HelixMovement>(GetComponentsInChildren<HelixMovement>());

        HelipadManager = FindObjectOfType<HelipadManager>();

        //Deactivates helix animation and refuel station
        _refuelStation.gameObject.SetActive(false);

        ActivateEngines(false);
    }

    private void Update()
    {
        if (_currentAgent == null) return;

        _fuelAmount = _statTracker.GetStatByType(StatType.Fuel).GetCurrentLevel();

        if (ShouldFinishUsage())
        {
            FinishUsage();
            return;
        }        
    }

    public override bool CanBeUsed(GameObject agent)
    {
        if (_currentAgent != null)
        {
            return false;
        }

        if (_fuelAmount <= 0)
        {
            return false;
        }

        if(HelipadManager == null)
        {
            HelipadManager = FindObjectOfType<HelipadManager>();
        }       

        if (!HelipadManager.AnyHelipadsAvailable())
        {
            return false;
        }

        return base.CanBeUsed(agent);
    }

    public override async UniTask Activate(GameObject agent)
    {
        //Updates current agent        
        _currentAgent = agent;
        _currentAgent.SetActive(false);        

        //Deactivates refuel station
        _refuelStation.gameObject.SetActive(false);

        ActivateEngines(true);

        //Disables fuel tracker
        _fuelDepleter.enabled = true;

        //Enables elicopter brain
        _elicopterBrain.enabled = false;
        await _elicopterBrain.InitBrain();
    }

    private bool ShouldFinishUsage()
    {
        if (HasLanded)
        {
            return true;
        }

        if (_fuelAmount <= 0)
        {
            return true;
        }

        return false;
    }

    private async void FinishUsage()
    {
        while (!CanDisembark)
        {
            await UniTask.NextFrame();
        }

        HasLanded = false;

        //Activates refuel station
        _refuelStation.gameObject.SetActive(true);

        //Updates current agent
        _currentAgent.transform.position = _agentExitPoint.position;
        _currentAgent.SetActive(true);        
        _currentAgent = null;

        //Disables fuel tracker
        _fuelDepleter.enabled = false;

        //Resets disembark flag
        CanDisembark = false;

        ActivateEngines(false);

        //Stops elicopter brain
        _elicopterBrain.enabled = false;        
    }   

    public void Refuel(int amount)
    {
        _fuelAmount += amount;
        _statTracker.GetStatByType(StatType.Fuel).SetCurrentLevel(_fuelAmount);
    }

    public bool CanBeRefueled()
    {
        if (_statTracker.GetStatByType(StatType.Fuel).GetCurrentLevel() <= _statTracker.GetStatByType(StatType.Fuel).GetMaxLevel())
        {
            return true;
        }
        else
        {
            return false;
        }        
    }

    private void ActivateEngines(bool activate)
    {
        foreach (var helix in _helixes)
        {
            helix.enabled = activate;
        }
    }
}
