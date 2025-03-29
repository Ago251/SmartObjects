using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WorldInterface.SmartObject
{
    public class TurretSmartObject : SmartObject
    {
        [SerializeField] private int _ammoCount = 10;

        [SerializeField] private float _fireRate = 1;

        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Transform _muzzle;

        [SerializeField] private float _detectionRange;
        [SerializeField] private LayerMask _detectionMask;

        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _shootAngle;

        private bool _canShoot = true;

        private UniTaskCompletionSource _completionSource;

        private GameObject _currentAgent;

        private void Update()
        {
            if (_currentAgent == null) return;

            if (ShouldFinishUsage())
            {
                FinishUsage();
                return;
            }

            var target = SelectTarget();

            if (target == null) FinishUsage();


            AimTowardsTarget(target);

            if (ReadyToShoot(target)) Fire();
        }

        public override bool CanBeUsed(GameObject agent)
        {
            if (_currentAgent != null)
            {
                return false;
            }

            if (_ammoCount <= 0)
            {
                return false;
            }

            return base.CanBeUsed(agent);
        }

        public override UniTask Activate(GameObject agent)
        {
            _currentAgent = agent;
            _currentAgent.SetActive(false);

            _completionSource = new UniTaskCompletionSource();
            return _completionSource.Task;
        }


        private void Fire()
        {
            if (!_canShoot && _ammoCount <= 0) return;

            Instantiate(_projectilePrefab, _muzzle.position, _muzzle.rotation);
            _ammoCount--;
            Cooldown().Forget();
        }

        private async UniTaskVoid Cooldown()
        {
            _canShoot = false;
            await UniTask.WaitForSeconds(1 / _fireRate);
            _canShoot = true;
        }

        private GameObject SelectTarget()
        {
            var result = new Collider[1];
            if (Physics.OverlapSphereNonAlloc(transform.position, _detectionRange, result, _detectionMask) <=
                0) return null;

            return result[0].gameObject;
        }

        private void AimTowardsTarget(GameObject target)
        {
            var currentTransform = transform;
            var currentRotation = currentTransform.rotation;
            currentRotation = Quaternion.RotateTowards(currentRotation,
                Quaternion.LookRotation((target.transform.position - currentTransform.position).normalized, Vector3.up),
                _rotationSpeed * Time.deltaTime);
            transform.rotation = currentRotation;
        }

        private bool ReadyToShoot(GameObject target)
        {
            if (!_canShoot) return false;

            if (_ammoCount <= 0) return false;

            if (Quaternion.Angle(transform.rotation,
                    Quaternion.LookRotation((target.transform.position - transform.position).normalized, Vector3.up)) >
                _shootAngle)
                return false;

            return true;
        }

        private bool ShouldFinishUsage()
        {
            if (_ammoCount <= 0) return true;

            return false;
        }

        private void FinishUsage()
        {
            _currentAgent.SetActive(true);
            _currentAgent = null;

            _completionSource.TrySetResult();
        }

        public bool CanBeReloaded()
        {
            return true;
        }

        public void Reload(int amount)
        {
            _ammoCount += amount;
        }
        
    }
}