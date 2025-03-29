using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using WorldInterface.SmartObjects;


namespace WorldInterface.MiniCity.Hospital
{
    public class HospitalSmartObject : SmartObject.SmartObject
    {
        [SerializeField]
        private int _healthRate;
        [SerializeField]
        private int _seatMax = 10;
        [SerializeField]
        private List<Seat> _seats = new List<Seat>();
        [SerializeField]
        private Transform _entryPoint;
        private GameObject _medic;
        private UniTaskCompletionSource _completionSource;

        private void Update()
        {
            if (_medic != null)
            {
                Heal();

                if (_seats.Count == 0)
                    Deactivate();
            }
        }

        public override UniTask Activate(GameObject agent)
        {
            agent.SetActive(false);
            _medic = agent;
            _completionSource = new UniTaskCompletionSource();

            return _completionSource.Task;
        }

        public void Deactivate()
        {
            _medic.SetActive(true);
            _medic = null;

            _completionSource.TrySetResult();
        }

        public override bool CanBeUsed(GameObject agent)
        {
            return base.CanBeUsed(agent) && _medic == null && _seats.Count > 0;
        }

        public bool TryAdd(GameObject agent)
        {
            if (_seats.Count >= _seatMax)
                return false;

            _seats.Add(new Seat { Agent = agent, StatTracker = agent.GetComponent<StatTracker>() });
            return true;
        }

        private void Heal()
        {
            for (int i = _seats.Count - 1; i >= 0; i--)
            {
                var healthStat = _seats[i].StatTracker.GetStatByType(StatType.Health);
                healthStat.Increase((int)(_healthRate * Time.deltaTime));
                if (healthStat.CurrentLevel < healthStat.GetMaxLevel())
                {
                    _seats[i].Agent.transform.position = _entryPoint.position;
                    _seats[i].Agent.SetActive(true);
                    _seats.RemoveAt(i);
                }
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
    }
}
