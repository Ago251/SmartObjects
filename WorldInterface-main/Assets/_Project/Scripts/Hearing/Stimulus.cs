using UnityEngine;

namespace WorldInterface.Hearing
{
    [RequireComponent(typeof(SphereCollider))]
    public class Stimulus : MonoBehaviour
    {
        [field: SerializeField]
        public float Intensity { get; private set; }

        [SerializeField] private float _lifeTime;

        private void Start()
        {
            Destroy(gameObject, _lifeTime);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var sphereCollider = GetComponent<SphereCollider>();
            Gizmos.DrawSphere(transform.TransformPoint(sphereCollider.center), sphereCollider.radius);
        }
    }
}