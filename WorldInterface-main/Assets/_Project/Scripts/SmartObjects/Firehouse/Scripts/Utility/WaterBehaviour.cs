using UnityEngine;

namespace WorldInterface.SmartObject
{
    public class WaterBehaviour : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.TryGetComponent(out HouseSmartObject house))
            {
                house.EstinguishFire();
                Destroy(gameObject);
            }

            if(other.tag == "Fire")
            {
                Destroy(gameObject);
            }
        }
    }
}
