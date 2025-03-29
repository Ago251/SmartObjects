using UnityEngine;

namespace WorldInterface
{
    [CreateAssetMenu]
    public class Archetype : ScriptableObject
    {
        [field: SerializeField]
        public ArchetypeTrait[] Traits { get; private set; }

        public Color Color;
    }
}