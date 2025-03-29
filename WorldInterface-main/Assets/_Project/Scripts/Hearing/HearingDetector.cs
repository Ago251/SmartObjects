using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace WorldInterface.Hearing
{
    [RequireComponent(typeof(SphereCollider)), RequireComponent(typeof(Rigidbody))]
    public class HearingDetector : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _falloffCurve = AnimationCurve.Linear(0,1, 1, 0);
        
        private readonly HashSet<Stimulus> _detectedStimuli = new();
        private SphereCollider _sphereCollider;

        private void Start()
        {
            _sphereCollider = GetComponent<SphereCollider>();
        }

        private void Update()
        {
            _detectedStimuli.RemoveWhere(x => x == null);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<Stimulus>(out var stimulus))
            {
                return;
            }

            _detectedStimuli.Add(stimulus);
        }

        public bool TryGetMostRelevantStimulus(out Stimulus stimulus)
        {
            if (_detectedStimuli.Count == 0)
            {
                stimulus = null;
                return false;
            }
            
            stimulus = _detectedStimuli.Aggregate((first, second) => ProcessIntensity(first) > ProcessIntensity(second) ? first : second);
            return true;
        }

        private float ProcessIntensity(Stimulus stimulus)
        {
            return stimulus.Intensity *
                   _falloffCurve.Evaluate(math.distance(transform.position, stimulus.transform.position) / _sphereCollider.radius);
        }

        private void OnDrawGizmos()
        {
            foreach (var detectedStimulus in _detectedStimuli.Where(detectedStimulus => detectedStimulus != null))
            {
                Gizmos.color = Color.Lerp(Color.red, Color.green, ProcessIntensity(detectedStimulus));
                Gizmos.DrawWireSphere(detectedStimulus.transform.position, .5f);
            }
        }
    }
}