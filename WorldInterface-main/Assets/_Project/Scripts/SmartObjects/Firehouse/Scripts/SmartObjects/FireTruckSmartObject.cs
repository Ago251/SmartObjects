using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace WorldInterface.SmartObject
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class FireTruckSmartObject : SmartObject
    {
        private GameObject _currentAgent;

        [SerializeField] private float _speed;

        [SerializeField] private float _waterLevel;
        [SerializeField] private float _waterLevelThreshold;
        [SerializeField] private float _detectionRange;

        private GameObject _targetHydrant;
        private GameObject _targetFirePlace;
        public GameObject TargetFirePlace { get => _targetFirePlace; }

        [SerializeField] private GameObject _waterPrefab;

        private NavMeshAgent _navMeshAgent;

        [SerializeField] private StateMachineSchema _schema;
        [SerializeField] private StateMachine _currentStateMachine;

        private CancellationTokenSource _cancellationTokenSource;

        [SerializeField] private bool _onDuty;

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
            _cancellationTokenSource = new CancellationTokenSource();

            OnDuty = true;

            _currentAgent = agent;

            await _currentStateMachine.Init(gameObject, _cancellationTokenSource.Token);

            OnDuty = false;
            _currentAgent = null;

            await UniTask.CompletedTask;
        }

        public override bool CanBeUsed(GameObject agent)
        {
            return false;
        }

        public bool CanBeSentOnDuty()
        {
            if (_currentAgent != null)
            {
                return false;
            }

            if(_onDuty == true)
            {
                return false;
            }
            
            return true;
        }

        public bool IsWaterLevelOk()
        {
            return _waterLevel > _waterLevelThreshold;
        }
    
        public UniTask ShootWater()
        {
            var water = Instantiate(_waterPrefab, transform.position, Quaternion.identity);

            var fireDirection = GetComponent<FiretruckTargetDatabase>().fireplaceTarget.transform.position - transform.position;

            water.GetComponent<Rigidbody>().AddForce(Vector3.up + fireDirection * 10f, ForceMode.Impulse);

            _waterLevel = 0f;

            return UniTask.CompletedTask;
        }

        public async UniTask RechargeWater()
        {
            await UniTask.WaitUntil(() => 
            {
                _waterLevel += 0.8f;
                if(_waterLevel >= 100f)
                {
                    _waterLevel = 100f;
                }
                return _waterLevel >= 100f;
            });

            await UniTask.CompletedTask;
        }

        public void SetFirePlaceTarget(GameObject firePlace)
        {
            if(TryGetComponent(out FiretruckTargetDatabase firetruckTargetDatabase))
            {
                if(firetruckTargetDatabase.fireplaceTarget == null) 
                    firetruckTargetDatabase.fireplaceTarget = firePlace;
            }
        }

        public void StopStateMachine()
        {
            _cancellationTokenSource.Cancel();

            OnDuty = false;
            _currentAgent = null;
        }

        private void OnDrawGizmos() 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 15f);
        }
    }
}