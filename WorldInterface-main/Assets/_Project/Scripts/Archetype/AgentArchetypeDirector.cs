using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WorldInterface
{
    public class AgentArchetypeDirector : MonoBehaviour
    {
        [SerializeField] private Archetype[] _archetypes;

        private readonly HashSet<ArchetypeIdentity> _configuredAgents = new();

        private void Update()
        {
            ConfigureAgents();
        }

        private void ConfigureAgents()
        {
            foreach (var agentArchetype in FindObjectsOfType<ArchetypeIdentity>()
                         .Where(x => !_configuredAgents.Contains(x)))
            {
                _configuredAgents.Add(agentArchetype);
                agentArchetype.SetArchetype(_archetypes.Random());
            }
        }
    }
}