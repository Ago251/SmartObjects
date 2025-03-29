using UnityEngine;

namespace WorldInterface
{
    public class PrefabSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _prefabToSpawn;
        [SerializeField] private bool _execute;

        private void Update()
        {
            if (!_execute)
            {
                return;
            }

            _execute = false;
            Spawn();
        }

        private void Spawn()
        {
            if (_prefabToSpawn == null)
            {
                return;
            }
            
            Instantiate(_prefabToSpawn, transform.position, transform.rotation);
        }
    }
}