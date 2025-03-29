using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WorldInterface.SmartObject
{
    public class MoneyProviderSmartObject : SmartObject
    {
        [SerializeField] private int _moneyToGive;
        
        private GameObject _currentAgent;
        
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

            return base.CanBeUsed(agent);
        }

        public override async UniTask Activate(GameObject agent)
        {
            if (!agent.TryGetComponent(out HandController handController))
            {
                return;
            }
            _currentAgent = agent;
            
            for (var i = 0; i < _moneyToGive; i++)
            {
                handController.AddItem(HandItem.Money, 1);
                await UniTask.WaitForSeconds(0.5f);
            }
            
            _currentAgent = null;
        }
    }
}