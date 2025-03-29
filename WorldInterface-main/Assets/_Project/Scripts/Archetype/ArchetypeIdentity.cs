using System;
using System.Linq;
using UnityEngine;

namespace WorldInterface
{
    public class ArchetypeIdentity : MonoBehaviour
    {
        [SerializeField]
        private ArchetypeTrait[] _traits = Array.Empty<ArchetypeTrait>();
        
        public ArchetypeTrait[] Traits => _traits;

        public void SetArchetype(Archetype archetype)
        {
            _traits = new ArchetypeTrait[archetype.Traits.Length];
            for (var i = 0; i < archetype.Traits.Length; i++)
            {
                _traits[i] = archetype.Traits[i];
            }
            GetComponentInChildren<Renderer>().material.color = archetype.Color;
        }
        
        public bool HasTrait(ArchetypeTrait trait)
        {
            return _traits.Contains(trait);
        }
    }
}