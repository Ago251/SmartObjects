using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace WorldInterface.Vision
{
    [ExecuteInEditMode]
    public class ViewCone : MonoBehaviour
    {
        // ReSharper disable once UseArrayEmptyMethod
        [SerializeReference, SubclassSelector]
        private ViewFilter[] _filters = new ViewFilter[0];

        // ReSharper disable once UseArrayEmptyMethod
        [SerializeReference, SubclassSelector] 
        private ViewArea[] _engageVisionArea = new ViewArea[0];

        // ReSharper disable once UseArrayEmptyMethod
        [SerializeReference, SubclassSelector] 
        private ViewArea[] _disengageVisionArea = new ViewArea[0];

        public UnityEvent<Transform> ObjectEnteredVision = new();
        public UnityEvent<Transform> ObjectExitedVision = new();

        public HashSet<Transform> CurrentVisibleObjects { get; private set; } = new();
        
        private void Update()
        {
            foreach (var visibleObject in CurrentVisibleObjects.ToArray())
            {
                if (_disengageVisionArea.Any(x => x.ContainsObject(visibleObject, transform))
                    && !_filters.Any(x => x.ShouldFilter(transform, visibleObject)))
                {
                    continue;
                }

                CurrentVisibleObjects.Remove(visibleObject);
                ObjectExitedVision?.Invoke(visibleObject);
            }

            foreach (var visionArea in _engageVisionArea)
            {
                foreach (var visibleObject in visionArea.GetVisibleObjects(transform))
                {
                    if (_filters.Any(x => x.ShouldFilter(transform, visibleObject)))
                    {
                        continue;
                    }
                    
                    if (!CurrentVisibleObjects.Add(visibleObject))
                    {
                        continue;
                    }

                    ObjectEnteredVision?.Invoke(visibleObject);
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach (var visionArea in _engageVisionArea)
            {
                visionArea?.Draw(transform);
            }
            
            Gizmos.color = Color.red;
            foreach (var visionArea in _disengageVisionArea)
            {
                visionArea?.Draw(transform);
            }
            
            Gizmos.color = Color.blue;
            foreach (var visibleObject in CurrentVisibleObjects)
            {
                Gizmos.DrawWireSphere(visibleObject.transform.position, .5f);
            }
        }
    }
}