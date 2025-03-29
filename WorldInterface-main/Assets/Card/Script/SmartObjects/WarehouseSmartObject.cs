using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldInterface.SmartObject;

public class WarehouseSmartObject : SmartObject
{
    private GameObject _currentAgent;
    [Header("References")]
    [SerializeField] private List<BuildingTruckSmartObject> _buildingTruckSmartObjects;

    [Header("Detection")]
    [SerializeField] private List<GameObject> _damagedBuildingsFound;
    [SerializeField] private GameObject _damagedBuildingFound;
    [SerializeField] private float _detectionRange = 15f;
    [SerializeField] private LayerMask _layerMask;

    public override bool CanBeUsed(GameObject agent)
    {
        if (_currentAgent == null)
        {
            return false;

        }
        if(_damagedBuildingFound == null)
        { 
            return false; 
        }
        return base.CanBeUsed(agent);
    }

    public override async UniTask Activate(GameObject agent)
    {
        _currentAgent = agent;
        while (gameObject.activeSelf)
        {
            _damagedBuildingFound = await FindDamagedBuilding();
            foreach (var truck in _buildingTruckSmartObjects)
            {
                if(truck.gameObject.GetComponent<BuildingComponents>().damagedBuilding == null
                    && truck.CanBeUsed(_currentAgent))
                {
                    truck.SetDamagedBuildingTarget(_damagedBuildingFound);
                    truck.Activate(_currentAgent).Forget();
                    break;
                }
            }
        }
        await UniTask.CompletedTask;
    }
    private async UniTask<GameObject> FindDamagedBuilding()
    {
        GameObject nearestFirePlace = null;

        await UniTask.WaitWhile(() =>
        {
            Debug.Log("Finding nearest fire place...");

            var colliders =
                Physics.OverlapSphere(
                        transform.position,
                        _detectionRange,
                        _layerMask);

            foreach (var collider in colliders)
            {
                Debug.Log("Found collider: " + collider.gameObject);
            }

            var nearestColliders = colliders
                .OrderBy(x =>
                    Vector3.Distance(
                            x.transform.position,
                            transform.position)
                ).Where(x => {
                    if (_damagedBuildingFound == null) return true;
                    return x.gameObject.name != _damagedBuildingFound.name;
                });

            if (nearestColliders.Count() > 0)
                nearestFirePlace =
                    nearestColliders
                    .FirstOrDefault()
                    .gameObject;

            return nearestFirePlace == null;
        });

        _damagedBuildingFound = null;
        return nearestFirePlace;
    }
}
