using UnityEngine;

namespace SquadShooterMVP
{
    public class CharacterPresenter : BaseUnitPresenter
    {
        [Header("Components")]
        [SerializeField] private EnemyDetector _detector;
        [SerializeField] private AimRingView _aimRingPrefab; 

        private AimRingView _aimRingInstance;
        private InputService _inputService;
        
        private float _attackRange = 8f; 

        public void Init(UnitModel model, InputService input)
        {
            base.Setup(model);
            _inputService = input;

            // 1. SETUP DETECTOR
            _detector.Init(_attackRange);

            // 2. SETUP AIM RING
            if (_aimRingPrefab != null)
            {
                _aimRingInstance = Instantiate(_aimRingPrefab, transform.position, Quaternion.identity);
                _aimRingInstance.Init(transform);
                _aimRingInstance.SetRadius(_attackRange);
            }
        }

        protected override void HandleMovement()
        {
            if (_inputService == null) return;

            // --- MOVEMENT ---
            Vector2 input = _inputService.Movement;
            Vector3 inputDir = new Vector3(input.x, 0, input.y);

            if (inputDir.sqrMagnitude > 0.01f)
                _movementView.Move(inputDir, _model.MoveSpeed);
            else
                _movementView.Stop();

            // --- ROTATION / AIMING ---
            
            // Priority 1: Look at Closest Enemy (from Detector)
            if (_detector.HasTarget)
            {
                Transform target = _detector.ClosestEnemy.transform;
                Vector3 dirToEnemy = (target.position - transform.position).normalized;
                dirToEnemy.y = 0;

                _movementView.Rotate(dirToEnemy);
                
                // Show Ring because we are in combat mode
                if(_aimRingInstance) _aimRingInstance.Toggle(true);
            }
            // Priority 2: Look where moving
            else
            {
                if (inputDir.sqrMagnitude > 0.01f)
                    _movementView.Rotate(inputDir);
                
                // Hide Ring when peaceful (Optional style choice)
                if(_aimRingInstance) _aimRingInstance.Toggle(false); 
            }
        }

        protected override void HandleShooting()
        {
            if (_detector.HasTarget)
            {
                _shootingView.Shoot(_model.Damage, _model.FireRate);
            }
        }
    }
}