using UnityEngine;

namespace SquadShooterMVP
{
    public class MVPCameraController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Vector3 _offset = new Vector3(0, 15, -10); // High up and back
        [SerializeField] private float _smoothSpeed = 5f; // Higher = Snappier, Lower = Lazier

        private Transform _target;

        public void SetTarget(Transform target)
        {
            _target = target;
            
            // Instantly jump to start position so we don't drift from 0,0,0
            transform.position = _target.position + _offset;
        }

        private void LateUpdate()
        {
            if (_target == null) return;

            // 1. Calculate where the camera WANTS to be
            Vector3 desiredPosition = _target.position + _offset;

            // 2. Smoothly slide towards that position (Lerp)
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);

            // 3. Apply position
            transform.position = smoothedPosition;

            // 4. Ensure we are always looking at the player
            transform.LookAt(_target);
        }
    }
}