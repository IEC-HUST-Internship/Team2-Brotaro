using System.Collections.Generic;
using UnityEngine;

    public class EnemyDetector : MonoBehaviour
    {
        private SphereCollider _collider;
        public List<EnemyPresenter> _detectedEnemies = new List<EnemyPresenter>();
        public EnemyPresenter ClosestEnemy { get; private set; }
        public bool HasTarget => ClosestEnemy != null;

        public void Init(float radius)
        {
            _collider = GetComponent<SphereCollider>();
            if (_collider == null) _collider = gameObject.AddComponent<SphereCollider>();
            
            _collider.isTrigger = true;
            _collider.radius = radius;
        }

        public void UpdateClosestEnemy()
        {
            // 1. Clean up dead enemies or nulls from the list
            _detectedEnemies.RemoveAll(x => x == null || x.GetComponent<EnemyPresenter>().Model.IsDead.Value);

            if (_detectedEnemies.Count == 0)
            {
                ClosestEnemy = null;
                return;
            }

            // 2. Find closest
            EnemyPresenter nearest = null;
            float minDist = float.MaxValue;
            Vector3 myPos = transform.position;

            foreach (var enemy in _detectedEnemies)
            {
                float dist = Vector3.SqrMagnitude(enemy.transform.position - myPos);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = enemy;
                }
            }

            ClosestEnemy = nearest;
        }

        // --- PHYSICS TRIGGERS ---

        private void OnTriggerEnter(Collider other)
        {
            EnemyPresenter enemy = other.GetComponent<EnemyPresenter>();
            if(!enemy)
            {
                Debug.Log("Here is not enemy");
            }
            if (enemy != null)
            {
                Debug.Log($"[Detector] Bắt được bộ phận: {other.name} của {enemy.name}");
                var model = enemy.GetComponent<EnemyPresenter>().Model;
                if (!_detectedEnemies.Contains(enemy) && model != null && !model.IsDead.Value)
                {
                    _detectedEnemies.Add(enemy);
                    UpdateClosestEnemy(); 
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            EnemyPresenter enemy = other.GetComponentInParent<EnemyPresenter>();
            
            if (enemy != null && _detectedEnemies.Contains(enemy))
            {
                _detectedEnemies.Remove(enemy);
                Debug.Log($"[EnemyDetector] Removed: {enemy.name}");
                UpdateClosestEnemy();
            }
        }
    }