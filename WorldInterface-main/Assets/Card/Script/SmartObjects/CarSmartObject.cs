using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WorldInterface.SmartObject
{
    [RequireComponent(typeof(Agent))]
    public class CarSmartObject : SmartObject
    {
        [SerializeField] private float _fuel = 100;
        [SerializeField] private float _fuelConsumption = 1;
        [SerializeField] private int _minimumFuelAmount = 2;
        [SerializeField] private int _cost = 2;
        [SerializeField] private int _refuelPerSecond = 1;
        [SerializeField] private float _parkingTolerance = 2f;
        [SerializeField] private float drivingTime;
        [SerializeField] private int stressIncreasing = 1;

        [SerializeField]
        private GameObject _currentAgent;
        private Agent _agent;
        public Transform target;
        private StatTracker _statsTracker;
        private MeshRenderer _meshRenderer;
        private readonly StatType stressType = StatType.Stress;

        public float _stressLevel;

        private void Update()
        {
            if (_currentAgent == null)
            {
                return;
            }
            if (_currentAgent != null)
            {
                _fuel -= _fuelConsumption * Time.deltaTime;
            }
            if (_statsTracker != null)
            {
                var stressLevel = _statsTracker.GetStatByType(stressType);
                stressLevel.Increase(stressIncreasing);
            }
        }
        private void Awake()
        {
            _agent = GetComponent<Agent>();
            _agent.enabled = false;
        }
        public override async UniTask Activate(GameObject agent)
        {
            _currentAgent = agent;
            _statsTracker = _currentAgent.GetComponent<StatTracker>();
            _meshRenderer = _currentAgent.GetComponentInChildren<MeshRenderer>();
            _agent.enabled = true;

            if (_currentAgent == null)
            {
                return;
            }
            if (!agent.TryGetComponent(out HandController handController))
            {
                return;
            }
            if (handController.GetItemAmount(HandItem.Money) < _cost)
            {
                return;
            }
            if (_fuel <= _minimumFuelAmount)
            {
                await RefuelCar(agent);
            }
            if (_fuel <= 0f)
            {
                Disembark();
            }

            foreach(var behaviour in _agent._steering)
            {
                if(behaviour is GraphMoveBehaviour graphMoveBehaviour)
                {
                    handController.RemoveItem(HandItem.Money, _cost);
                    target = SelectDestination().transform;
                    graphMoveBehaviour._target = target;
                    graphMoveBehaviour._recalculatePath = true;
                }
            }
            if (target == null)
            {
                return;
            }
            _meshRenderer.enabled = false;
            await UniTask.WaitWhile(() =>
                Vector3.Distance(target.transform.position, transform.position) > _parkingTolerance);
            await Task.CompletedTask;
            Disembark();

        }
        private void Disembark()
        {
            _agent.enabled = false;
            _currentAgent.transform.position = transform.position + new Vector3(3f, 0f, 3f);
            _meshRenderer.enabled = true;
            _currentAgent = null;
        }
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
            if (handController.GetItemAmount(HandItem.Money) < _cost)
            {
                return false;
            }
            if (handController.GetItemAmount(HandItem.Fuel) < _minimumFuelAmount)
            {
                return false;
            }
            return base.CanBeUsed(agent);
        }

        public async UniTask RefuelCar(GameObject agent)
        {
            if (!agent.TryGetComponent(out HandController handController))
            {
                return;
            }
            var fuelAmount = handController.GetItemAmount(HandItem.Fuel);
            handController.RemoveItem(HandItem.Fuel, fuelAmount);
            _fuel += fuelAmount / _refuelPerSecond;
            await UniTask.WaitForSeconds(fuelAmount / _refuelPerSecond);
        }

        private GameObject SelectDestination()
        {
            GameObject[] allParkingBays = GameObject.FindGameObjectsWithTag("ParkingBay");
            Dictionary<GameObject, float> targets = new();
            foreach (GameObject parkingBay in allParkingBays)
            {
                var distance = math.distancesq(transform.position, parkingBay.transform.position);
                targets.Add(parkingBay, distance);
            }
            var sortedTargets = targets.OrderBy(kv => kv.Value).ToList();
            if (sortedTargets.Count > 0)
            {
                sortedTargets.RemoveAt(0);
                var randomChoice = Random.Range(0, sortedTargets.Count);
                var targetBay = sortedTargets[randomChoice];
                if (targetBay.Key != null)
                {
                    return targetBay.Key;
                }
            }
            return null;
        }
    }
}

