using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WorldInterface.SmartObject
{
    public abstract class SmartObject : MonoBehaviour
    {
        [SerializeField] protected ArchetypeTrait RequiredTrait;

        public virtual bool CanBeUsed(GameObject agent)
        {
            return RequiredTrait == null || agent.TryGetComponent(out ArchetypeIdentity archetype) && archetype.HasTrait(RequiredTrait);
        }

        public abstract UniTask Activate(GameObject agent);
    }
}