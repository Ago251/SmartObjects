using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldInterface.SmartObjects
{
    public class AgentGroundChecker : MonoBehaviour
    {
        public bool IsGrounded { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Trampoline"))
            {
                IsGrounded = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Trampoline"))
            {
                IsGrounded = false;
            }
        }
    }
}