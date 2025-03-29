using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorldInterface.Vision
{
    [Serializable]
    public class SphereViewArea : ViewArea
    {
        [SerializeField] private Vector3 _offset;
        [SerializeField] private float _radius;
        
        public override bool ContainsObject(Transform objectToTest, Transform ownerTransform)
        {
            return Vector3.Distance(objectToTest.position, ownerTransform.TransformPoint(_offset)) <= _radius;
        }

        public override IEnumerable<Transform> GetVisibleObjects(Transform ownerTransform)
        {
            return Physics.OverlapSphere(ownerTransform.TransformPoint(_offset), _radius)
                .Select(collider => collider.transform);
        }

        public override void Draw(Transform ownerTransform)
        {
            Gizmos.DrawWireSphere(ownerTransform.TransformPoint(_offset), _radius);
        }
    }
}