using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WavePresenter : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float _spawnOffset = 0.1f; 
    public GameObject[] spawnPoints;
    
    // Tracks live enemies
    private List<EnemyPresenter> _activeEnemies = new List<EnemyPresenter>();
    public EnemyDetector enemyDetector;
    
    private LevelData_Brotaro _currentLevelData;
    private int _currentWaveIndex;
    private int _enemiesRemainingInWave;
    private Camera _mainCamera; 

    public event Action<int, int> OnWaveChanged; 
    public event Action OnLevelCleared; 

    public void Init(LevelData_Brotaro data)
    {
        _currentLevelData = data;
        _currentWaveIndex = 0;
        _mainCamera = Camera.main; 
        
        // Start the first wave
        StartCoroutine(RunWave(_currentWaveIndex));
    }

    private IEnumerator RunWave(int index)
    {
        // Safety Check: If we ran out of waves, we stop spawning.
        // The actual WIN condition is handled in OnEnemyDied (when the last enemy is killed).
        if (index >= _currentLevelData.Waves.Count)
        {
            yield break;
        }

        WaveConfig currentWave = _currentLevelData.Waves[index];
        _enemiesRemainingInWave = currentWave.EnemyCount;
        
        // Notify UI that wave changed
        OnWaveChanged?.Invoke(index + 1, _currentLevelData.Waves.Count);

        for (int i = 0; i < currentWave.EnemyCount; i++)
        {
            SpawnEnemy(currentWave); 
            yield return new WaitForSeconds(currentWave.SpawnInterval);
        }
    }

    private void SpawnEnemy(WaveConfig config)
    {
        // 1. Determine Position
        Vector3 spawnPosition = Vector3.zero;
        if(spawnPoints.Length > 0)
            spawnPosition = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;
        else 
            spawnPosition = GetSpawnPositionOutsideCamera(); // Fallback to camera logic if no points

        // 2. Instantiate
        GameObject newEnemyObj = Instantiate(config.EnemyPrefab, spawnPosition, Quaternion.identity);
        
        // 3. Setup Logic
        EnemyPresenter enemy = newEnemyObj.GetComponent<EnemyPresenter>();
        if (enemy == null) enemy = newEnemyObj.AddComponent<EnemyPresenter>();

        UnitModel enemyModel = new UnitModel(speed: 3, hp: 50, dmg: 25); 
        var player = FindObjectOfType<PlayerPresenter>();
        
        if (player != null)
        {
            enemy.Init(enemyModel, newEnemyObj.GetComponent<UnitView>(), player.transform);
        }

        // 4. Add to list & Subscribe to Death
        _activeEnemies.Add(enemy);
        
        enemyModel.IsDead.Subscribe((isDead) => 
        {
            if (isDead) OnEnemyDied(enemy);
        });
    }

    // (Kept your existing Camera Spawn logic mostly same, just ensured it compiles)
    private Vector3 GetSpawnPositionOutsideCamera()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;
        int side = UnityEngine.Random.Range(0, 4);
        float x = 0, y = 0;

        switch (side)
        {
            case 0: x = UnityEngine.Random.Range(0f, 1f); y = 1f + _spawnOffset; break;
            case 1: x = UnityEngine.Random.Range(0f, 1f); y = -_spawnOffset; break;
            case 2: x = -_spawnOffset; y = UnityEngine.Random.Range(0f, 1f); break;
            case 3: x = 1f + _spawnOffset; y = UnityEngine.Random.Range(0f, 1f); break;
        }

        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(x, y, 0));
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float enter)) return ray.GetPoint(enter);
        
        return transform.position; // Safe fallback
    }

    private void OnEnemyDied(EnemyPresenter enemy)
    {
        if (_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Remove(enemy);
            
            // Update detector so player doesn't target dead bodies
            if (enemyDetector != null) enemyDetector.UpdateClosestEnemy();                
            
            _enemiesRemainingInWave--;
            
            // --- WAVE LOGIC ---
            if (_enemiesRemainingInWave <= 0)
            {
                // Move to next wave
                _currentWaveIndex++;
                
                // If there are more waves, start the next one
                if (_currentWaveIndex < _currentLevelData.Waves.Count)
                {
                    StartCoroutine(RunWave(_currentWaveIndex));
                }
            }
        }

        // --- WIN CONDITION CHECK ---
        // 1. Have we passed the last wave index?
        // 2. Are there zero enemies left alive?
        if (_currentWaveIndex >= _currentLevelData.Waves.Count && _activeEnemies.Count == 0)
        {
            // Trigger the Event
            OnLevelCleared?.Invoke();

            // Find GameplayPresenter and tell it we won
            var gameplay = FindObjectOfType<GameplayPresenter>();
            if(gameplay != null)
            {
                gameplay.WinCondition();
            }
        }
    }
}