using UnityEngine;

namespace WorldInterface.SmartObjects
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _lifeTime;
        [SerializeField] private float _speed;

        private void Start()
        {
            Destroy(gameObject, _lifeTime);
        }

        private void Update()
        {
            transform.position += transform.forward * _speed * Time.deltaTime;
        }
    }
}