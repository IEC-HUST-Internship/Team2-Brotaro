using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace SquadShooterMVP
{
    public class WavePresenter : MonoBehaviour
    {
        [Header("Config")]
        // REMOVED: _enemyPrefab (We now get this from LevelData per wave!)
        [SerializeField] private Transform[] _spawnPoints;

        [Header("Runtime Info")]
        private List<EnemyPresenter> _activeEnemies = new List<EnemyPresenter>();
        
        // DATA
        private LevelData _currentLevelData;
        private int _currentWaveIndex;
        private int _enemiesRemainingInWave;

        // EVENTS
        public event Action<int, int> OnWaveChanged; 
        public event Action OnLevelCleared; 

        public void Init(LevelData data)
        {
            _currentLevelData = data;
            _currentWaveIndex = 0;
            
            StartCoroutine(RunWave(_currentWaveIndex));
        }

        private IEnumerator RunWave(int index)
        {
            if (index >= _currentLevelData.Waves.Count)
            {
                Debug.Log("All waves finished. LEVEL CLEARED!");
                OnLevelCleared?.Invoke();
                yield break;
            }

            WaveConfig currentWave = _currentLevelData.Waves[index];
            _enemiesRemainingInWave = currentWave.EnemyCount;

            OnWaveChanged?.Invoke(index + 1, _currentLevelData.Waves.Count);
            Debug.Log($"Starting Wave {index + 1} with {currentWave.EnemyCount} enemies.");

            for (int i = 0; i < currentWave.EnemyCount; i++)
            {
                SpawnEnemy(currentWave); 
                
                yield return new WaitForSeconds(currentWave.SpawnInterval);
            }
        }

        private void SpawnEnemy(WaveConfig config)
        {
            if (config.EnemyPrefab == null)
            {
                Debug.LogError($"Wave {_currentWaveIndex + 1} has no Enemy Prefab assigned in LevelData!");
                return;
            }

            Transform spawnPoint = _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Length)];
            
            GameObject newEnemyObj = Instantiate(config.EnemyPrefab, spawnPoint.position, Quaternion.identity);
            
            EnemyPresenter enemy = newEnemyObj.GetComponent<EnemyPresenter>();

            if (enemy == null)
            {
                Debug.LogError("The Enemy Prefab in LevelData is missing the 'EnemyPresenter' script!");
                return;
            }

            UnitModel enemyModel = new UnitModel();
            
            var playerPresenter = FindObjectOfType<CharacterPresenter>();
            if (playerPresenter != null)
            {
                enemy.Init(enemyModel, playerPresenter.transform);
            }

            _activeEnemies.Add(enemy);
            
            enemyModel.IsDead.Subscribe((isDead) => 
            {
                if (isDead) OnEnemyDied(enemy);
            });
        }

        private void OnEnemyDied(EnemyPresenter enemy)
        {
            if (_activeEnemies.Contains(enemy))
            {
                _activeEnemies.Remove(enemy);
                _enemiesRemainingInWave--;

                if (_enemiesRemainingInWave <= 0)
                {
                    Debug.Log($"Wave {_currentWaveIndex + 1} Complete!");
                    
                    _currentWaveIndex++;
                    StartCoroutine(RunWave(_currentWaveIndex));
                }
            }
        }
    }
}