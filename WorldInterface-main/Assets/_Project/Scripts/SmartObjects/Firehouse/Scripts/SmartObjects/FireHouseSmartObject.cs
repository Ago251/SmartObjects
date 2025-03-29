using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WorldInterface.SmartObject
{
    public class FireHouse : SmartObject
    {
        private GameObject _currentAgent;

        [Header("References")]
        [SerializeField] private List<FireTruckSmartObject> _fireTrucks;
        [SerializeField] private List<GameObject> _fireHouseSlot;

        [Header("Detection")]
        [SerializeField] private List<GameObject> _firePlacesFound;
        [SerializeField] private GameObject _firePlaceFound;
        [SerializeField] private float _detectionRange;
        [SerializeField] private LayerMask _detectionMask;


        private void Awake() 
        {
            for (int i = 0; i < _fireHouseSlot.Count; i++)
            {
                _fireTrucks[i].gameObject.GetComponent<FiretruckTargetDatabase>()._fireHouse = _fireHouseSlot[i];
            }    
        }

        public override async UniTask Activate(GameObject agent)
        {
            _currentAgent = agent;

            while(true)
            {
                _firePlaceFound = await FindFirePlace();

                if(_firePlaceFound == null) 
                    continue;



                foreach (var fireTruck in _fireTrucks)
                {
                    if(fireTruck == null) continue;

                    if(fireTruck.gameObject.TryGetComponent(out FiretruckTargetDatabase firetruckTargetDatabase))
                    {
                        if(firetruckTargetDatabase.fireplaceTarget != null 
                        && firetruckTargetDatabase.fireplaceTarget.name == _firePlaceFound.name)
                        {
                            break;
                        }
                    }

                    if(firetruckTargetDatabase.fireplaceTarget == null
                        && fireTruck.CanBeSentOnDuty())
                    {
                        fireTruck.SetFirePlaceTarget(_firePlaceFound);
                        fireTruck.Activate(_currentAgent).Forget();

                        break;
                    }
                }
            }
        }

        private async UniTask<GameObject> FindFirePlace()
        {
            GameObject nearestFirePlace = null;

            await UniTask.WaitWhile(() => 
                {
                    Debug.Log("Finding nearest fire place...");

                    var colliders = 
                        Physics.OverlapSphere(
                                transform.position, 
                                _detectionRange, 
                                _detectionMask);

                    // foreach (var collider in colliders)
                    // {
                    //     Debug.Log("Found collider: " + collider.gameObject);
                    // }

                    var nearestColliders = colliders
                        .OrderBy(x => 
                            Vector3.Distance(
                                    x.transform.position, 
                                    transform.position)
                        ).Where(x => 
                            {
                                // if(_firePlaceFound == null) return false;
                                
                                bool isOnFire = false;

                                if(x.gameObject.TryGetComponent(out HouseSmartObject houseSmartObject))
                                {
                                    isOnFire = houseSmartObject.IsOnFire();
                                }

                                bool isAlreadyFound = false;

                                isAlreadyFound = _firePlaceFound != null &&  x.gameObject.name == _firePlaceFound.name;

                                return  isOnFire && !isAlreadyFound;
                            });
                        
                    if(nearestColliders.Count() > 0)
                        nearestFirePlace = 
                            nearestColliders
                            .FirstOrDefault()
                            .gameObject;

                    return nearestFirePlace == null;
                });

            _firePlaceFound = null;
            return nearestFirePlace;
        }

        public override bool CanBeUsed(GameObject agent)
        {
            if (_currentAgent != null)
            {
                return false;
            }

            // if(_firePlaceFound != null)
            // {
            //     return false;
            // }

            return base.CanBeUsed(agent);
        }

        private bool ThereIsFirePlace(GameObject firePlaceToCompare)
        {
            foreach (var firePlace in _firePlacesFound)
            {
                if(firePlace == firePlaceToCompare)
                {
                    return true;
                }
            }
            return false;
        }
    
        private void OnDrawGizmos() 
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);
        }
    }
}