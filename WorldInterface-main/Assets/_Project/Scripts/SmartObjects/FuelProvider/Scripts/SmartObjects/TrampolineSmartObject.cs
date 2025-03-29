using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace WorldInterface.SmartObject
{
    public class ElasticSmartObject : SmartObject
    {

        [SerializeField] private float _jumpForce = 55f;

        public override bool CanBeUsed(GameObject agent)
        {
            if (!agent.TryGetComponent(out HandController handController))
            {
                return false;
            }
            
            /*if (!(handController.CurrentItem is FuelHandItem))
            {
                return false;
            }*/

            return base.CanBeUsed(agent);
        }

        public override async UniTask Activate(GameObject agent)
        {
            if (!agent.TryGetComponent(out HandController handController))
            {
                return;
            }

            if (!agent.TryGetComponent(out Rigidbody rb))
            {
                Debug.LogWarning("Agent does not have a Rigidbody component");
                return;
            }

            if (!agent.TryGetComponent(out NavMeshAgent navMeshAgent))
            {
                Debug.LogWarning("Agent does not have a NavMeshAgent component");
                return;
            }

            if (!agent.TryGetComponent(out AgentGroundChecker groundChecker))
            {
                Debug.LogWarning("Agent does not have an AgentGroundChecker component");
                return;
            }

            //FuelHandItem fuelItem = handController.CurrentItem as FuelHandItem;

            for (var i = 0; i < 3/*fuelItem.FuelTankSize*/; i++)
            {
                if (navMeshAgent.enabled)
                {
                    navMeshAgent.updatePosition = false;
                    navMeshAgent.updateRotation = false;
                }
                // make the jump
                rb.AddRelativeForce(Vector3.up * _jumpForce, ForceMode.Impulse);

                await UniTask.WaitForSeconds(0.3f);
                _jumpForce = 20;
                await UniTask.WaitUntil(() => groundChecker.IsGrounded);
                Debug.Log("GroundCheker " + groundChecker.IsGrounded);
                // Attendi un attimo per stabilizzare l'agente a terra
                

                //rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
                //await UniTask.WaitForSeconds(1.3f);
            }

            Debug.Log("test");
        }


    }
}

