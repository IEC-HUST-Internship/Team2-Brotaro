using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class WavePresenter : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float _spawnOffset = 0.1f; 
    public GameObject[] spawnPoints;
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
        StartCoroutine(RunWave(_currentWaveIndex));
    }

    private IEnumerator RunWave(int index)
    {
        if (index >= _currentLevelData.Waves.Count)
        {
            OnLevelCleared?.Invoke();
            yield break;
        }

        WaveConfig currentWave = _currentLevelData.Waves[index];
        _enemiesRemainingInWave = currentWave.EnemyCount;
        OnWaveChanged?.Invoke(index + 1, _currentLevelData.Waves.Count);

        for (int i = 0; i < currentWave.EnemyCount; i++)
        {
            SpawnEnemy(currentWave); 
            yield return new WaitForSeconds(currentWave.SpawnInterval);
        }
    }

    private void SpawnEnemy(WaveConfig config)
    {
        Vector3 spawnPosition = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;

        GameObject newEnemyObj = Instantiate(config.EnemyPrefab, spawnPosition, Quaternion.identity);
        
        EnemyPresenter enemy = newEnemyObj.GetComponent<EnemyPresenter>();
        if (enemy == null) enemy = newEnemyObj.AddComponent<EnemyPresenter>();

        UnitModel enemyModel = new UnitModel(speed: 3, hp: 50, dmg: 25); 
        
        var player = FindObjectOfType<PlayerPresenter>();
        
        if (player != null)
        {
            enemy.Init(enemyModel, newEnemyObj.GetComponent<UnitView>(), player.transform);
        }

        _activeEnemies.Add(enemy);
        
        enemyModel.IsDead.Subscribe((isDead) => 
        {
            if (isDead) OnEnemyDied(enemy);
        });
    }

    private Vector3 GetSpawnPositionOutsideCamera()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;
        int side = UnityEngine.Random.Range(0, 4);
        

        float x = 0, y = 0;

        switch (side)
        {
            case 0: // Top (Trên đầu) -> y > 1
                x = UnityEngine.Random.Range(0f, 1f);
                y = 1f + _spawnOffset;
                break;
            case 1: // Bottom (Dưới chân) -> y < 0
                x = UnityEngine.Random.Range(0f, 1f);
                y = -_spawnOffset;
                break;
            case 2: // Left (Bên trái) -> x < 0
                x = -_spawnOffset;
                y = UnityEngine.Random.Range(0f, 1f);
                break;
            case 3: // Right (Bên phải) -> x > 1
                x = 1f + _spawnOffset;
                y = UnityEngine.Random.Range(0f, 1f);
                break;
        }

        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(x, y, 0));
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float enter;

        if (groundPlane.Raycast(ray, out enter))
        {
            return ray.GetPoint(enter);
        }
        return Vector3.zero; 
    }

    private void OnEnemyDied(EnemyPresenter enemy)
    {
        if (_activeEnemies.Contains(enemy))
        {
            _activeEnemies.Remove(enemy);
            if (enemyDetector != null) enemyDetector.UpdateClosestEnemy();                
            _enemiesRemainingInWave--;
            
            if (_enemiesRemainingInWave <= 0)
            {
                _currentWaveIndex++;
                StartCoroutine(RunWave(_currentWaveIndex));
            }
        }
    }
}