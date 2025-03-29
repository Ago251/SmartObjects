using System;
using System.Linq;
using UnityEngine;

namespace WorldInterface
{
    public class HandController : MonoBehaviour
    {
        [field: SerializeField]
        public int MaxWeight { get; private set; }

        [SerializeField]
        private int[] _currentItems = Array.Empty<int>();

        public int CurrentWeight => _currentItems.Select((t, i) => t * ((HandItem)i).GetWeight()).Sum();

        public int GetItemAmount(HandItem item)
        {
            if (_currentItems.Length == 0)
            {
                _currentItems = new int[Enum.GetValues(typeof(HandItem)).Length];
            }
            return _currentItems[(int)item];
        }

        public void AddItem(HandItem item, int amount)
        {
            if (_currentItems.Length == 0)
            {
                _currentItems = new int[Enum.GetValues(typeof(HandItem)).Length];
            }
            
            if (amount * item.GetWeight() + CurrentWeight > MaxWeight)
            {
                return;
            }

            _currentItems[(int)item] += amount;
        }

        public void RemoveItem(HandItem item, int amount)
        {
            if (_currentItems.Length == 0)
            {
                _currentItems = new int[Enum.GetValues(typeof(HandItem)).Length];
            }
            
            if (_currentItems[(int)item] < amount)
            {
                return;
            }

            _currentItems[(int)item] -= amount;
        }
    }
}