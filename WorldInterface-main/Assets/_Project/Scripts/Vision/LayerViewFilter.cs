using System;
using UnityEngine;

namespace WorldInterface.Vision
{
    [Serializable]
    public class LayerViewFilter : ViewFilter
    {
        [SerializeField] private LayerMask _layerMask;
        
        public override bool ShouldFilter(Transform agent, Transform objectToTest)
        {
            return ((1 << objectToTest.gameObject.layer) & _layerMask) == 0;
            
        }
    }
}