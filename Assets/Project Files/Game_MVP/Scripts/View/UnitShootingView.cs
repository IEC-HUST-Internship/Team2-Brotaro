using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SquadShooterMVP
{
    public class UnitShootingView : MonoBehaviour
    {
        [SerializeField] private Transform _firePoint;
        [SerializeField] private GameObject _bulletPrefab; 

        private float _nextFireTime;

        // Command: Shoot(damage)
        public void Shoot(int damage, float fireRate)
        {
            if (Time.time < _nextFireTime) return;
            _nextFireTime = Time.time + fireRate;

            if (_bulletPrefab != null && _firePoint != null)
            {
                var bullet = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
            }
        }
    }
}
