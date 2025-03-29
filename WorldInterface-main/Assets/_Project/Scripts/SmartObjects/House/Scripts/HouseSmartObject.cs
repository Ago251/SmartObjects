using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WorldInterface.SmartObject
{
    public class HouseSmartObject : SmartObject, IRescueable
    {
        [Header("Agent")]
        [SerializeField]
        private int maxAgents;

        [SerializeField]
        private Transform exitPoint;

        [Header("Energy")]
        [SerializeField]
        private int energyValue;

        [SerializeField]
        private float energyIncreaseTime = 1f;

        [Header("Integrity")]
        [SerializeField]
        private int maxIntegrity;

        [SerializeField]
        private int currentIntegrity;

        [SerializeField]
        private float integrityDecreaseTime = 1f;

        [SerializeField]
        private int integrityDecayAmount;

        [Header("Fire")]
        [SerializeField]
        private GameObject[] fireFXs;

        [SerializeField]
        [Range(0, 100)]
        private int _fireProbability = 20;

        [Header("Health")]
        [SerializeField]
        private int healthDamage;

        private readonly List<StatTracker> _currentAgents = new();
        private readonly float healthDamageTime = 1f;
        private readonly float hungerIncreaseTime = 1f;


        private UniTaskCompletionSource _completionSource;
        [SerializeField] private bool _isOnFire;
        private float _timeDamagePassed;
        private float _timeEnergyPassed;
        private float _timeFirePassed;
        private float _timeHungerPassed;
        private float _timeIntegrityPassed;

        [Header("Hunger")] private int hungerValue;

        private void Awake()
        {
            currentIntegrity = maxIntegrity;
        }

        private void Update()
        {
            if (_isOnFire) DecreaseIntegrity();

            if (_currentAgents.Count <= 0)
            {
                return;
            }

            if (!_isOnFire)
            {
                TryCatchFire(); // The house can catch fire only if the agents are inside
            }
            foreach (var currentAgent in _currentAgents)
            {
                if (_isOnFire) DecreaseHealth(currentAgent);

                if (ShouldFinishUsage(currentAgent) || IsIntegrityLow())
                {
                    FinishUsage(currentAgent);
                    return;
                }

                IncreaseEnergy(currentAgent);

                IncreaseHunger(currentAgent);
            }
        }

        private void OnEnable()
        {
            RescueManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            RescueManager.Instance.Unregister(this);
        }

        public event Action<IRescueable> OnEmergency;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public List<GameObject> Rescue(int capacityMax)
        {
            var totalRescues = Mathf.Clamp(capacityMax, 0, _currentAgents.Count);
            var agents = _currentAgents.GetRange(0, totalRescues);
            _currentAgents.RemoveRange(0, totalRescues);
            var injured = agents.Select(agent => agent.gameObject).ToList();
            return injured;
        }

        public override bool CanBeUsed(GameObject agent)
        {
            if (_currentAgents.Count == maxAgents)
            {
                return false;
            }

            if (IsIntegrityLow())
            {
                return false;
            }

            if (agent.TryGetComponent(out StatTracker statTracker))
            {
                if (statTracker == null)
                {
                    return false;
                }

                if (!IsEnergyLow(statTracker) && !IsHungerLow(statTracker))
                {
                    return false;
                }
            }

            return base.CanBeUsed(agent);
        }

        public override UniTask Activate(GameObject agent)
        {
            _completionSource = new UniTaskCompletionSource();

            if (_currentAgents.Contains(agent.GetComponent<StatTracker>()))
            {
                return _completionSource.Task;
            }

            _currentAgents.Add(agent.GetComponent<StatTracker>());
            agent.SetActive(false);

            return _completionSource.Task;
        }

        private bool ShouldFinishUsage(StatTracker currentAgent)
        {
            Debug.Log(currentAgent.name + " has bread " + AgentHasBread(currentAgent));
            if (IsEnergyFull(currentAgent) && !AgentHasBread(currentAgent) ||
                IsHungerFull(currentAgent) && !IsEnergyLow(currentAgent) ||
                IsEnergyFull(currentAgent) && IsHungerFull(currentAgent))
            {
                return true;
            }

            return false;
        }

        private void FinishUsage(StatTracker currentAgent)
        {
            currentAgent.transform.position = exitPoint.position;
            currentAgent.gameObject.SetActive(true);
            _currentAgents.Remove(currentAgent);
        }

        #region Health

        private void DecreaseHealth(StatTracker statTracker)
        {
            _timeDamagePassed += Time.deltaTime;
            if (_timeDamagePassed >= healthDamageTime)
            {
                statTracker.GetStatByType(StatType.Health).Decrease(healthDamage);
                _timeDamagePassed = 0;
            }
        }

        #endregion

        #region Energy

        private void IncreaseEnergy(StatTracker statTracker)
        {
            _timeEnergyPassed += Time.deltaTime;
            if (_timeEnergyPassed >= energyIncreaseTime)
            {
                statTracker.GetStatByType(StatType.Energy).Increase(energyValue);
                _timeEnergyPassed = 0;
            }
        }

        private bool IsEnergyLow(StatTracker statTracker)
        {
            return statTracker.GetStatByType(StatType.Energy).GetCurrentLevel() <= 50;
        }

        private bool IsEnergyFull(StatTracker statTracker)
        {
            return statTracker.GetStatByType(StatType.Energy).GetCurrentLevel() == 100;
        }

        #endregion

        #region Hunger

        private void IncreaseHunger(StatTracker statTracker)
        {
            _timeHungerPassed += Time.deltaTime;
            if (_timeHungerPassed >= hungerIncreaseTime)
            {
                statTracker.GetStatByType(StatType.Hunger).Increase(statTracker.gameObject
                    .GetComponent<HandController>().GetItemAmount(HandItem.Bread));
                _timeHungerPassed = 0;
            }
        }

        private bool AgentHasBread(StatTracker currentAgent)
        {
            Debug.Log(currentAgent.GetComponent<HandController>().GetItemAmount(HandItem.Bread));
            return currentAgent.GetComponent<HandController>().GetItemAmount(HandItem.Bread) > 0;
        }

        private bool IsHungerLow(StatTracker statTracker)
        {
            return statTracker.GetStatByType(StatType.Hunger).GetCurrentLevel() <= 50;
        }

        private bool IsHungerFull(StatTracker statTracker)
        {
            return statTracker.GetStatByType(StatType.Hunger).GetCurrentLevel() == 100;
        }

        #endregion

        #region Fire

        private void TryCatchFire()
        {
            _timeFirePassed += Time.deltaTime;
            if (_timeFirePassed >= 1f)
            {
                SetRandomOnFire();
                _timeFirePassed = 0;
            }
        }

        public void EstinguishFire()
        {
            SetOnFire(false);
            foreach (var fireFX in fireFXs) fireFX.SetActive(false);
        }

        private void SetRandomOnFire()
        {
            float randomValue = Random.Range(0, 100);
            if (randomValue <= _fireProbability)
            {
                SetOnFire(true);
                OnEmergency?.Invoke(this);
            }
        }

        public void SetOnFire(bool onFire)
        {
            _isOnFire = onFire;
            foreach (var fireFX in fireFXs) fireFX.SetActive(true);
        }

        public bool IsOnFire()
        {
            return _isOnFire;
        }

        #endregion

        #region Integrity

        private bool IsIntegrityLow()
        {
            return GetIntegrityPercentage() < 25;
        }

        private float GetIntegrityPercentage()
        {
            return currentIntegrity * maxIntegrity / 100;
        }

        private void DecreaseIntegrity()
        {
            _timeIntegrityPassed += Time.deltaTime;
            if (_timeIntegrityPassed >= integrityDecreaseTime)
            {
                currentIntegrity = math.clamp(currentIntegrity - integrityDecayAmount, 0, maxIntegrity);
                _timeIntegrityPassed = 0;
            }
        }

        public void AddIntegrity(int amount)
        {
            currentIntegrity += amount;
        }

        #endregion
    }
}