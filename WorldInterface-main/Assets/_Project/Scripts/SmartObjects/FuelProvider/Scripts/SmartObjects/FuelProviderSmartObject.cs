using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldInterface.SmartObject
{
    public class FuelProviderSmartObject : SmartObject
    {
        [SerializeField] private int _fuelToGive;
        public int FuelCost = 2;


        public override bool CanBeUsed(GameObject agent)
        {
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


            _fuelToGive = Mathf.FloorToInt(handController.GetItemAmount(HandItem.Money) / (float)FuelCost);

            for (var i = 0; i < _fuelToGive; i++)
            {
                handController.RemoveItem(HandItem.Money, FuelCost);
                handController.AddItem(HandItem.Fuel, 1);
                await UniTask.WaitForSeconds(0.5f);
            }
        }
    }
}