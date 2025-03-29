using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using WorldInterface.SmartObject;

namespace WorldInterface.SmartObject
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class BuildingTruckSmartObject : SmartObject
    {
        private GameObject _currentAgent;

        [SerializeField] private float _speed;

        [SerializeField] private float _materialLevels;
        [SerializeField] private float _materialLevelThreshold;
        [SerializeField] private float _detectionRange;
        [SerializeField] private int _integrityPerSecond = 1;
        [SerializeField] private int _maxIntegrity = 100;

        private GameObject _targetWarehouse;
        private GameObject _targetDamagedBuilding;
        public GameObject TargetDamagedBuilding { get => _targetDamagedBuilding; }

        [SerializeField] private GameObject _warehousePrefab;

        private NavMeshAgent _navMeshAgent;
        private StatTracker _statTracker = null;
        private readonly StatType integrityType = StatType.Health;

        [SerializeField] private StateMachineSchema _schema;
        [SerializeField] private StateMachine _currentStateMachine;

        private CancellationTokenSource _cancellationTokenSource;

        private bool _onDuty;
        

        public bool OnDuty
        {
            get { return _onDuty; }
            private set { _onDuty = value; }
        }
        void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _navMeshAgent = GetComponent<NavMeshAgent>();

            _currentStateMachine = _schema.StateMachine.Clone();
        }
        public override async UniTask Activate(GameObject agent)
        {
            Debug.Log("Activating FireTruckSmartObject...");

            OnDuty = true;

            _currentAgent = agent;
            agent.SetActive(false);

            await _currentStateMachine.Init(gameObject, _cancellationTokenSource.Token);
        }
        public override bool CanBeUsed(GameObject agent)
        {
            if (_currentAgent != null)
            {
                return false;
            }

            if (_onDuty == true)
            {
                return false;
            }

            return base.CanBeUsed(agent);
        }
        public async UniTask BuildingRepair(GameObject building)
        {
            
            if (_statTracker == null)
            {
                _statTracker = _currentAgent.GetComponent<StatTracker>();
            }
            var integrityStats = _statTracker.GetStatByType(integrityType);
            integrityStats.Increase(_integrityPerSecond);
            _materialLevels -= _integrityPerSecond * Time.deltaTime;
            await UniTask.WaitUntil(()=> integrityStats.GetCurrentLevel() == _maxIntegrity
            || _materialLevels <= 1 );
        }
        public async UniTask RechargeBuildingComponents()
        {
            await UniTask.WaitUntil(() =>
            {
                _materialLevels += 0.8f;
                if (_materialLevels >= 100f)
                {
                    _materialLevels = 100f;
                }
                return _materialLevels >= 100f;
            });

            await UniTask.CompletedTask;
        }
        public void StopStateMachine()
        {
            _cancellationTokenSource.Cancel();

            OnDuty = false;
            _currentAgent.SetActive(true);
            _currentAgent = null;
        }
        public bool HasEnoughMaterial()
        {
            return _materialLevels > _materialLevelThreshold;
        }

        internal void SetDamagedBuildingTarget(GameObject damageBuilding)
        {
            if (TryGetComponent(out BuildingComponents buildingComponents))
            {
                if (buildingComponents.damagedBuilding == null)
                    buildingComponents.damagedBuilding = damageBuilding;
            }
        }
    }

}
