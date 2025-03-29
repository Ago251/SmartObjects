using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/*
namespace WorldInterface.SmartObjects
{
    public class OldQueueFuelProviderSmartObject : SmartObject
    {
        private int _fuelToGive;
        public int FuelCost = 2;

        private Queue<GameObject> _waitingQueue = new Queue<GameObject>();
        private List<ActiveAgent> _activeAgents = new List<ActiveAgent>();
        private const int MaxActiveAgents = 2;

        [SerializeField] private Transform[] _pumpPositions; // Array of positions at the pumps
        [SerializeField] private Transform _queuePosition; // Position for the queue
        [SerializeField] private Transform _agentExitPosition; // Position for the exit

        private object _queueLock = new object(); // Lock object for queue management
        private object _activeAgentsLock = new object(); // Lock object for active agents management

        private struct ActiveAgent
        {
            public GameObject Agent;
            public int PumpIndex;

            public ActiveAgent(GameObject agent, int pumpIndex)
            {
                Agent = agent;
                PumpIndex = pumpIndex;
            }
        }

        public override bool CanBeUsed(GameObject agent)
        {
            if (!agent.TryGetComponent(out HandController handController))
            {
                return false;
            }

            if (handController.GetItemAmount(HandItem.Money) < 1)
            {
                return false;
            }

            lock (_queueLock)
            {
                if (_waitingQueue.Contains(agent) || _activeAgents.Exists(a => a.Agent == agent))
                {
                    return false;
                }
            }

            return true;
        }

        private async void Update()
        {
            await ProcessQueue();
        }

        public override async UniTask Activate(GameObject agent)
        {
            if (!agent.TryGetComponent(out HandController handController))
            {
                return;
            }

            lock (_queueLock)
            {
                _waitingQueue.Enqueue(agent);
            }

            //Debug.Log($"Agent {agent.name} added to queue.");
        }

        private async UniTask ProcessQueue()
        {
            GameObject agentToProcess = null;
            int pumpIndex = -1;

            lock (_queueLock)
            {
                if (_waitingQueue.Count == 0)
                {
                    return;
                }

                lock (_activeAgentsLock)
                {
                    if (_activeAgents.Count < MaxActiveAgents)
                    {
                        agentToProcess = _waitingQueue.Dequeue();
                        pumpIndex = GetNextAvailablePumpIndex();
                        _activeAgents.Add(new ActiveAgent(agentToProcess, pumpIndex));
                        //Debug.Log($"Agent {agentToProcess.name} moved to pump {pumpIndex}.");
                    }
                }
            }

            if (agentToProcess != null)
            {
                var targetPosition = _pumpPositions[pumpIndex].position;
                await MoveAgentToPosition(agentToProcess, targetPosition);

                await FillFuel(agentToProcess);

                lock (_activeAgentsLock)
                {
                    _activeAgents.RemoveAll(a => a.Agent == agentToProcess);
                    //Debug.Log($"Agent {agentToProcess.name} finished refueling and left the pump.");
                }
            }

            await UpdateQueuePositions();
            await UniTask.Yield();
        }

        private int GetNextAvailablePumpIndex()
        {
            for (int i = 0; i < _pumpPositions.Length; i++)
            {
                if (!_activeAgents.Exists(a => a.PumpIndex == i))
                {
                    return i;
                }
            }
            return -1; // This should never happen if the logic is correct
        }

        private async UniTask MoveAgentToPosition(GameObject agent, Vector3 targetPosition)
        {
            float duration = 2.8f;
            float elapsedTime = 0.0f;
            Vector3 startPosition = agent.transform.position;
            Vector3 targetPosition2D = new Vector3(targetPosition.x, startPosition.y, targetPosition.z);

            while (elapsedTime < duration)
            {
                agent.transform.position = Vector3.Lerp(startPosition, targetPosition2D, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                await UniTask.Yield();
            }

            agent.transform.position = targetPosition;
        }

        private async UniTask FillFuel(GameObject agent)
        {
            if (!agent.TryGetComponent(out HandController handController))
            {
                return;
            }

            _fuelToGive = Mathf.FloorToInt(handController.GetItemAmount(HandItem.Money) / (float)FuelCost);

            //Debug.Log($"Agent {agent.name} starts refueling with {_fuelToGive} units of fuel.");
            NavMeshAgent nAgent = agent.GetComponent<NavMeshAgent>();

            nAgent.isStopped = true;

            for (var i = 0; i < _fuelToGive; i++)
            {
                handController.RemoveItem(HandItem.Money, FuelCost);
                handController.AddItem(HandItem.Fuel, 1);
                await UniTask.WaitForSeconds(0.5f);
                //Debug.Log($"Agent {agent.name} refueled 1 unit. Remaining: {_fuelToGive - i - 1}");
            }

            nAgent.isStopped = false;
        }

        private async UniTask UpdateQueuePositions()
        {
            int queueIndex = 0;

            lock (_queueLock)
            {
                foreach (var agent in _waitingQueue)
                {
                    var targetPosition = _queuePosition.position + Vector3.right * 2 * queueIndex;
                    MoveAgentToPosition(agent, targetPosition).Forget();
                    queueIndex++;
                }
            }

        }
    }
}
*/