using UnityEngine;

namespace WorldInterface.SmartObject
{
    public enum FiretruckTargetType
    {
        Hydrant,
        Fire,
        FireHouse
    }

    public class FiretruckTargetDatabase : MonoBehaviour
    {
        public GameObject hydrantTarget;
        public GameObject fireplaceTarget;
        public GameObject _fireHouse;
    }
}