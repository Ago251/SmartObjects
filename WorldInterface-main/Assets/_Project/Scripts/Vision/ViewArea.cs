using System;
using System.Collections.Generic;
using UnityEngine;

namespace WorldInterface.Vision
{
    [Serializable]
    public abstract class ViewArea
    {
        public abstract bool ContainsObject(Transform objectToTest, Transform ownerTransform);
        public abstract IEnumerable<Transform> GetVisibleObjects(Transform ownerTransform);
        public abstract void Draw(Transform ownerTransform);
    }
}