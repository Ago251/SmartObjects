using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WorldInterface.SmartObject
{
    public class TurretAmmoReloaderSmartObject : SmartObject
    {
        private GameObject _currentAgent;
        [SerializeField] private TurretSmartObject _turret;
        
        public override bool CanBeUsed(GameObject agent)
        {
            if (_currentAgent != null)
            {
                return false;
            }

            if (!agent.TryGetComponent(out HandController handController))
            {
                return false;
            }

            if (handController.GetItemAmount(HandItem.Ammo) <= 0)
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
            
            _currentAgent = agent;
            while (handController.GetItemAmount(HandItem.Ammo) > 0 && _turret.CanBeReloaded())
            {
               _turret.Reload(1);
               handController.RemoveItem(HandItem.Ammo, 1);
               await UniTask.WaitForSeconds(.5f);
            }

            _currentAgent = null;
        }
    }
}