using Unity.Mathematics;
using UnityEngine;

namespace WorldInterface
{
    public class DayNightCycle : Singleton<DayNightCycle>
    {
        [SerializeField] private float _dayLength = 3;
        
        private float _currentTime;
        
        public float CurrentTime => _currentTime;

        private void Update()
        {
            _currentTime += Time.deltaTime / _dayLength;
            while (_currentTime > 24f)
            {
                _currentTime -= 24f;
            }
            
            transform.rotation = Quaternion.Euler(math.remap(0, 24, -90, 270, _currentTime),0 ,0);
        }
    }
}