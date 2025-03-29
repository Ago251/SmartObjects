using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WorldInterface.SmartObject
{
    public class TurretAmmoProviderSmartObject : SmartObject
    {
        [SerializeField] private int _ammoToGive;
        
        public override bool CanBeUsed(GameObject agent)
        {
            if (!agent.TryGetComponent(out HandController handController))
            {
                return false;
            }

            if (handController.CurrentWeight + _ammoToGive * HandItem.Ammo.GetWeight() > handController.MaxWeight)
            {
                return false;
            }

            return base.CanBeUsed(agent);
        }

        public override async UniTask Activate(GameObject agent)
        {
            if (!agent.TryGetComponent(out HandController handController))
            {
                return;
            }
            
            for (var i = 0; i < _ammoToGive; i++)
            {
                handController.AddItem(HandItem.Ammo, 1);
                await UniTask.WaitForSeconds(0.5f);
            }
        }
    }
}