using System;
using UnityEngine;

namespace WorldInterface.Vision
{
    [Serializable]
    public abstract class ViewFilter
    {
        public abstract bool ShouldFilter(Transform agent, Transform objectToTest);
    }
}