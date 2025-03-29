using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using WorldInterface.SmartObject;

namespace WorldInterface.SmartObjects
{
    public class ParkSmartObject : SmartObject.SmartObject
    {
        [Header("Agent")]
        [SerializeField]
        private int maxAgents=5;

        private List<StatTracker> _currentAgents = new List<StatTracker>();

        [SerializeField]
        private Transform exitPoint;

        [Header("Stress Relief")]
        [SerializeField]
        private int stressReliefValue=2;

        [SerializeField]
        private float stressReliefTime = 1f;
        private float _timeStressPassed;


        private UniTaskCompletionSource _completionSource;


        private void Update()
        {
            if (_currentAgents.Count <= 0)
            {
                return;
            }

            foreach (var currentAgent in _currentAgents)
            {
                ReduceStress(currentAgent);

                if (ShouldFinishUsage(currentAgent))
                {
                    FinishUsage(currentAgent);
                    return;
                }
            }
        }

        public override bool CanBeUsed(GameObject agent)
        {
            if (_currentAgents.Count == maxAgents)
            {
                return false;
            }

            if (agent.TryGetComponent(out StatTracker statTracker))
            {
                if (statTracker == null)
                {
                    return false;
                }

                if (!IsStressHigh(statTracker))
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
            var agentBehaviour = agent.GetComponent<SimpleAgentBehaviour>();
            
            if (agentBehaviour != null)
            {
                agentBehaviour._isDecreasingStress = true;
            }
            
            _currentAgents.Add(agent.GetComponent<StatTracker>());
            
            return _completionSource.Task;
        }

        private bool ShouldFinishUsage(StatTracker currentAgent)
        {
            if (IsStressLow(currentAgent))
            {
                return true;
            }

            return false;
        }

        private void FinishUsage(StatTracker currentAgent)
        {
            var agentBehaviour = currentAgent.GetComponent<SimpleAgentBehaviour>();
            
            if (agentBehaviour)
            {
                agentBehaviour._isDecreasingStress = false;
                currentAgent.gameObject.SetActive(false);
                currentAgent.transform.position = exitPoint.position;
                currentAgent.gameObject.SetActive(true);
            }

            _currentAgents.Remove(currentAgent);
        }

        private void ReduceStress(StatTracker statTracker)
        {
            _timeStressPassed += Time.deltaTime;
            if (_timeStressPassed >= stressReliefTime)
            {
                statTracker.GetStatByType(StatType.Stress).Decrease(stressReliefValue);
                _timeStressPassed = 0;
            }
        }

        private bool IsStressHigh(StatTracker statTracker)
        {
            return statTracker.GetStatByType(StatType.Stress).GetCurrentLevel() >= 50;
        }

        private bool IsStressLow(StatTracker statTracker)
        {
            return statTracker.GetStatByType(StatType.Stress).GetCurrentLevel() <= 0;
        }
    }
}
