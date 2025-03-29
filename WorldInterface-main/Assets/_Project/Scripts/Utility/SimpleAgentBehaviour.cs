using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace WorldInterface.SmartObject
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SimpleAgentBehaviour : MonoBehaviour
    {
        [HideInInspector] public bool _isDecreasingStress;

        [SerializeField] private float _smartObjectRange = 500f;

        public float DecreaseStepTime = 1;

        private float _currentAwaitedTime;
        private Stat _energyStat;
        private NavMeshAgent _navMeshAgent;
        private StatTracker _statTracker;
        private Stat _stressStat;
        private Stat _hungerStat;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _statTracker = GetComponent<StatTracker>();
        }

        private void Start()
        {
            _energyStat = _statTracker.GetStatByType(StatType.Energy);
            _stressStat = _statTracker.GetStatByType(StatType.Stress);
            _hungerStat = _statTracker.GetStatByType(StatType.Hunger);
        }

        private void Update()
        {
            ConsumeStat();
        }

        private void OnEnable()
        {
            BrainLoop().Forget();

            _currentAwaitedTime = 0;
        }

        private void ConsumeStat()
        {
            _currentAwaitedTime += Time.deltaTime;
            if (_currentAwaitedTime >= DecreaseStepTime)
            {
                _energyStat.Decrease(_energyStat.Decay);
                _hungerStat.Decrease(_hungerStat.Decay);
                if (!_isDecreasingStress) _stressStat.Increase(_stressStat.Decay);
                _currentAwaitedTime = 0;
            }
        }

        private void ConsumeEnergy()
        {
            _currentAwaitedTime += Time.deltaTime;
            if (_currentAwaitedTime >= DecreaseStepTime)
            {
                _energyStat.Decrease(_energyStat.Decay);
                if (!_isDecreasingStress) _stressStat.Increase(_stressStat.Decay);
                _currentAwaitedTime = 0;
            }
        }


        private async UniTaskVoid BrainLoop()
        {
            while (enabled)
            {
                var possibleSmartObjects = FindObjectsOfType<SmartObject>().Where(x =>
                    x.CanBeUsed(gameObject) && math.distancesq(transform.position, x.transform.position) <=
                    _smartObjectRange * _smartObjectRange);
                if (possibleSmartObjects.Any())
                {
                    var targetSmartObject = possibleSmartObjects.Random();
                    await ReachSmartObject(targetSmartObject);
                    await ActivateSmartObject(targetSmartObject);
                }

                await UniTask.NextFrame();
            }
        }

        private async UniTask ReachSmartObject(SmartObject targetSmartObject)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(targetSmartObject.transform.position);

            await UniTask.WaitWhile(() =>
                targetSmartObject != null && targetSmartObject.CanBeUsed(gameObject) &&
                Vector3.Distance(targetSmartObject.transform.position, transform.position) > 4f);

            _navMeshAgent.isStopped = true;
        }

        private async UniTask ActivateSmartObject(SmartObject targetSmartObject)
        {
            if (!targetSmartObject.CanBeUsed(gameObject)) return;

            await targetSmartObject.Activate(gameObject);
        }
    }
}