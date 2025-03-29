using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace WorldInterface.Vision
{
    [Serializable]
    public class LineOfSightViewFilter : ViewFilter
    {
        [SerializeField] private LayerMask _obstacleMask;

        public override bool ShouldFilter(Transform agent, Transform objectToTest)
        {
            var deltaPosition = objectToTest.position - agent.position;

            if (!Physics.Raycast(agent.position, deltaPosition.normalized, deltaPosition.magnitude, _obstacleMask))
                return false;

            switch (objectToTest.GetComponent<Collider>())
            {
                case BoxCollider boxCollider:
                {
                    var size = boxCollider.size;
                    var center = boxCollider.center;
                    var vertices = new[]
                    {
                        boxCollider.transform.TransformPoint(center.x - size.x / 2, center.y - size.y / 2,
                            center.z - size.z / 2),
                        boxCollider.transform.TransformPoint(center.x + size.x / 2, center.y - size.y / 2,
                            center.z - size.z / 2),
                        boxCollider.transform.TransformPoint(center.x - size.x / 2, center.y + size.y / 2,
                            center.z - size.z / 2),
                        boxCollider.transform.TransformPoint(center.x + size.x / 2, center.y + size.y / 2,
                            center.z - size.z / 2),
                        boxCollider.transform.TransformPoint(center.x - size.x / 2, center.y - size.y / 2,
                            center.z + size.z / 2),
                        boxCollider.transform.TransformPoint(center.x + size.x / 2, center.y - size.y / 2,
                            center.z + size.z / 2),
                        boxCollider.transform.TransformPoint(center.x - size.x / 2, center.y + size.y / 2,
                            center.z + size.z / 2),
                        boxCollider.transform.TransformPoint(center.x + size.x / 2, center.y + size.y / 2,
                            center.z + size.z / 2)
                    };

                    if (vertices.Select(vertex => vertex - agent.position)
                        .Any(vertexDeltaPosition =>
                            !Physics.Raycast(agent.position, vertexDeltaPosition.normalized,
                                vertexDeltaPosition.magnitude, _obstacleMask)))
                    {
                        return false;
                    }

                    break;
                }
                case CapsuleCollider capsuleCollider:
                {
                    var parallelDirection = math.cross(deltaPosition.normalized, Vector3.up);
                    Vector3 minPosition = (float3)capsuleCollider.transform.TransformPoint(capsuleCollider.center) - parallelDirection * capsuleCollider.radius;
                    var minPositionDelta = minPosition - agent.position;
                    if (!Physics.Raycast(agent.position, minPositionDelta.normalized, minPositionDelta.magnitude,
                            _obstacleMask))
                    {
                        return false;
                    }
                    
                    Vector3 maxPosition = (float3)capsuleCollider.transform.TransformPoint(capsuleCollider.center) + parallelDirection * capsuleCollider.radius;
                    var maxPositionDelta = maxPosition - agent.position;
                    if (!Physics.Raycast(agent.position, maxPositionDelta.normalized, maxPositionDelta.magnitude,
                            _obstacleMask))
                    {
                        return false;
                    }
                    break;
                }
                case SphereCollider sphereCollider:
                {
                    break;
                }
            }

            return true;
        }
    }
}