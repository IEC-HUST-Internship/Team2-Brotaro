using UnityEngine;
using System.Collections;

namespace SquadShooterMVP
{
    public class MVP_TestBootstrap : MonoBehaviour
    {
        [Header("System Prefabs")]
        [SerializeField] private CharacterPresenter _playerPrefab;
        [SerializeField] private EnemyPresenter _enemyPrefab;

        [Header("Scene References")]
        [SerializeField] private InputService _inputService; 
        [SerializeField] private MVPCameraController _cameraController;
        
        // Private reference to the live player
        private CharacterPresenter _activePlayer;

        private void Start()
        {
            StartGame();
        }

        private void StartGame()
        {
            // 1. Create Data
            var playerModel = new UnitModel { MoveSpeed = 8f, Damage = 25, FireRate = 0.15f };

            // 2. Spawn Player at Center (0, 2, 0) to avoid floor issues
            _activePlayer = Instantiate(_playerPrefab, new Vector3(0, 2, 0), Quaternion.identity);
            _activePlayer.Init(playerModel, _inputService);
            if (_cameraController != null)
            {
                _cameraController.SetTarget(_activePlayer.transform);
            }
            // 3. Start the Horde!
            StartCoroutine(SpawnEnemyRoutine());
        }

        private IEnumerator SpawnEnemyRoutine()
        {
            while (_activePlayer != null)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(2.0f); 
            }
        }

        private void SpawnEnemy()
        {
            if (_activePlayer == null) return;

            // 1. Pick a random spot 10 meters away from player
            Vector2 randomCircle = Random.insideUnitCircle.normalized * 10f;
            Vector3 spawnPos = _activePlayer.transform.position + new Vector3(randomCircle.x, 2f, randomCircle.y);

            // 2. Create Enemy Data
            var enemyModel = new UnitModel { MoveSpeed = 3f, Damage = 10, FireRate = 1.0f };

            // 3. Spawn & Init
            var enemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
            enemy.Init(enemyModel, _activePlayer.transform);
        }
    }
}