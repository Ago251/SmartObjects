using System;

namespace WorldInterface
{
    public enum HandItem
    {
        Ammo,
        Money,
        Fuel,
        Bread
    }

    public static class HandItemUtility
    {
        public static int GetWeight(this HandItem item)
        {
            return item switch
            {
                HandItem.Ammo => 10,
                HandItem.Money => 1,
                HandItem.Fuel => 100,
                HandItem.Bread => 100,
                _ => throw new ArgumentOutOfRangeException(nameof(item), item, null)
            };
        }
    }
}