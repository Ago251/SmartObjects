using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace WorldInterface.MiniCity.Hospital
{
    public class AmbulanceSmartObject : SmartObject.SmartObject
    {
        [SerializeField]
        private int _seatMax = 10;
        [SerializeField]
        private NavMeshAgent _navMeshAgent;
        private GameObject _driver;
        [SerializeField]
        private List<Seat> _seats = new List<Seat>();
        private IRescueable _target;
        [SerializeField]
        private GameObject _exitPoint;
        [SerializeField]
        private GameObject _parking;

        public override async UniTask Activate(GameObject agent)
        {
            agent.SetActive(false);
            _driver = agent;

            do
            {
                _target = RescueManager.Instance.emergencies[0];
                RescueManager.Instance.emergencies.RemoveAt(0);

                await GoTo(_target.GetGameObject());

                AddInjured(_target.Rescue(_seatMax));

                var hospital = GetNearestHospital();

                await GoTo(hospital.gameObject);

                do
                {

                    for (int i = _seats.Count - 1; i >= 0; i--)
                    {
                        if (hospital.TryAdd(_seats[i].Agent))
                        {
                            _seats.RemoveAt(i);
                        }
                    }

                    await UniTask.NextFrame();

                } while (_seats.Count > 0);

            }while(RescueManager.Instance.emergencies.Count > 0);

            await GoTo(_parking);

            Deactivate();
        }

        public void Deactivate()
        {
            _driver.transform.position = _exitPoint.transform.position;
            _driver.SetActive(true);
            _driver = null;
        }

        private void AddInjured(List<GameObject> agents)
        {
            for (int i = 0; i < agents.Count; i++)
            {
                _seats.Add(new Seat { Agent = agents[i] });
            }
        }


        private async UniTask GoTo(GameObject destination)
        {
            _navMeshAgent.SetDestination(destination.transform.position);
            _navMeshAgent.isStopped = false;

            await UniTask.WaitForSeconds(1f);

            await UniTask.WaitUntil(() => _navMeshAgent.remainingDistance < 1f);

            _navMeshAgent.isStopped = true;
        }

        public override bool CanBeUsed(GameObject agent)
        {
            return base.CanBeUsed(agent) && _driver == null && RescueManager.Instance?.emergencies.Count > 0;
        }

        private HospitalSmartObject GetNearestHospital()
        {
            HospitalSmartObject nearestHospital = null;
            float minDistance = float.MaxValue;
            foreach(var hospital in RescueManager.Instance.Hospitals)
            {
                var distance = Vector3.Distance(transform.position, hospital.transform.position);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    nearestHospital = hospital;
                }
            }

            return nearestHospital;
        }
    }
}
