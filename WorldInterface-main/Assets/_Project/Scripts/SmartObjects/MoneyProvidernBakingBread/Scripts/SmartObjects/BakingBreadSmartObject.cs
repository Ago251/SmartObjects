using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace WorldInterface.SmartObject
{
    public class BackingBreadSmartObject : SmartObject
    {
        private GameObject _currentAgent;
        [SerializeField] private int _breadAmount;
        [SerializeField] private int _breadPrice;
        [SerializeField] private int _maxAmount = 5;
        
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

            if (handController.GetItemAmount(HandItem.Money) < _breadPrice)
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
            while (handController.GetItemAmount(HandItem.Money) >= _breadPrice && _breadAmount > 0)
            {
                var amountToBuy = math.min(Mathf.FloorToInt(handController.GetItemAmount(HandItem.Money) / (float)_breadPrice),
                    _maxAmount);
                
                handController.RemoveItem(HandItem.Money, amountToBuy * _breadPrice);
                handController.AddItem(HandItem.Bread, amountToBuy);
                
                await UniTask.WaitForSeconds(.5f);
            }
            
            _currentAgent = null;
        }
    }
}