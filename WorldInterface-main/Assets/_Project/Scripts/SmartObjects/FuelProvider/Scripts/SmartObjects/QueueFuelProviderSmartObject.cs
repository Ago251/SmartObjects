using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AI;


namespace WorldInterface.SmartObjects
{
    public class QueueFuelProviderSmartObject : SmartObject.SmartObject
    {
        private Queue<GameObject> _agentQueue = new Queue<GameObject>();
        private bool[] _pumpsAvailable;

        private int _fuelToGive;
        public int FuelCost = 2;

        [SerializeField] private Transform[] _pumpPositions;
        [SerializeField] private Transform _queuePosition;

        private void Start()
        {
            _pumpsAvailable = new bool[_pumpPositions.Length];

            for (int i = 0; i < _pumpsAvailable.Length; i++)
            {
                _pumpsAvailable[i] = true;
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

            return base.CanBeUsed(agent);
        }


        public override async UniTask Activate(GameObject agent)
        {
            _agentQueue.Enqueue(agent);

            var targetPosition = _queuePosition.position + Vector3.right * 2 * _agentQueue.Count;
            await MoveAgentToPosition(agent, targetPosition, 1f);

            await UniTask.WaitUntil(() => _agentQueue.Peek() == agent && IsPumpAvailable());

            // Unregister the agent from the queue
            _agentQueue.Dequeue();

            // Assign a pump to the agent
            int pumpIndex = AssignPump(agent);

            targetPosition = _pumpPositions[pumpIndex].position;
            await MoveAgentToPosition(agent, targetPosition, 2f);

            await UpdateQueuePositions();

            await FillFuel(agent);

            _pumpsAvailable[pumpIndex] = true;

        }

        private bool IsPumpAvailable()
        {
            foreach (bool available in _pumpsAvailable)
            {
                if (available) return true;
            }
            return false;
        }

        private int AssignPump(GameObject agent)
        {
            for (int i = 0; i < _pumpsAvailable.Length; i++)
            {
                if (_pumpsAvailable[i])
                {
                    _pumpsAvailable[i] = false;
                    //Debug.Log(agent + " is assigned to pump " + i);
                    return i;
                }
            }
            return -1; // Should never happen if IsPumpAvailable is checked
        }

        private async UniTask FillFuel(GameObject agent)
        {
            if (!agent.TryGetComponent(out HandController handController))
            {
                return;
            }

            _fuelToGive = Mathf.FloorToInt(handController.GetItemAmount(HandItem.Money) / (float)FuelCost);

            for (var i = 0; i < _fuelToGive; i++)
            {
                handController.RemoveItem(HandItem.Money, FuelCost);
                handController.AddItem(HandItem.Fuel, 1);
                await UniTask.WaitForSeconds(0.5f);
                //Debug.Log($"Agent {agent.name} refueled 1 unit. Remaining: {_fuelToGive - i - 1}");
            }

        }

        private async UniTask MoveAgentToPosition(GameObject agent, Vector3 targetPosition, float duration)
        {
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


        private async UniTask UpdateQueuePositions()
        {
            // Create a copy of the queue to iterate over
            var queueCopy = new List<GameObject>(_agentQueue);

            int count = 1;
            foreach (var queuedAgent in queueCopy)
            {
                var newPosition = _queuePosition.position + Vector3.right * 2 * count;
                await MoveAgentToPosition(queuedAgent, newPosition, 0.4f);
                count++;
            }
        }
    }
}