using Unity.VisualScripting;
using UnityEngine;

namespace SquadShooterMVP
{
    public class UnitMovementView : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Transform _visualRoot; 

        // Command: Move(x, y, z)
        public void Move(Vector3 direction, float speed)
        {
            if (_rb != null)
            {
                Vector3 newVelocity = direction * speed;
                _rb.velocity = new Vector3(newVelocity.x, _rb.velocity.y, newVelocity.z);
            }
        }

        public void Rotate(Vector3 direction)
        {
            if (direction.magnitude > 0.1f && _visualRoot != null)
            {
                _visualRoot.rotation = Quaternion.LookRotation(direction);
            }
        }

        public void Stop()
        {
            if (_rb != null) 
            {
                _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            }
        }
    }
}