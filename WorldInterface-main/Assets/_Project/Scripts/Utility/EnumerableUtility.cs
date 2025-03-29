using System.Collections.Generic;
using System.Linq;

namespace WorldInterface
{
    public static class EnumerableUtility
    {
        public static T Random<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.ElementAt(UnityEngine.Random.Range(0, enumerable.Count()));
        }
    }
}