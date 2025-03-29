using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorldInterface.Vision
{
    [Serializable]
    public class ConeViewArea : ViewArea
    {
        [SerializeField] private Vector3 _offset;
        [SerializeField] private float _radius;
        [SerializeField, Range(0, 360)] private float _angle;
        public override bool ContainsObject(Transform objectToTest, Transform ownerTransform)
        {
            if (Vector3.Distance(objectToTest.position, ownerTransform.TransformPoint(_offset)) > _radius)
            {
                return false;
            }
            var vectorToObject = (objectToTest.position - ownerTransform.TransformPoint(_offset)).normalized;
            return Vector3.Angle(vectorToObject, ownerTransform.forward) <= _angle / 2;
        }

        public override IEnumerable<Transform> GetVisibleObjects(Transform ownerTransform)
        {
            return Physics.OverlapSphere(ownerTransform.TransformPoint(_offset), _radius)
                .Where(x => ContainsObject(x.transform, ownerTransform)).Select(collider => collider.transform);
        }

        public override void Draw(Transform ownerTransform)
        {
            var forward = ownerTransform.forward;
            var worldPosition = ownerTransform.TransformPoint(_offset);
            Gizmos.DrawRay(worldPosition, Quaternion.Euler(0, -_angle/2f, 0) * forward * _radius);
            Gizmos.DrawRay(worldPosition, Quaternion.Euler(0, _angle/2f, 0) * forward * _radius);
            GizmosUtility.DrawWireArc(worldPosition, _radius, _angle, 8, Quaternion.Euler(0, -_angle /2f, 0) *  ownerTransform.rotation);
        }
    }
}