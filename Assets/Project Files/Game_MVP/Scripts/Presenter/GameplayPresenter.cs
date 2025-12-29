using UnityEngine;
using System;

namespace SquadShooterMVP
{
    public class GameplayPresenter : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private CharacterPresenter _player;
        [SerializeField] private WavePresenter _wavePresenter;
        [SerializeField] private InputService _inputService; 

        [SerializeField] private MVP_CamControl _camera; 
        
        [Header("Config")]
        [SerializeField] private LevelData _level;

        public event Action OnGameWin;
        public event Action OnGameLose;

        private void Start()
        {
            UnitModel playerModel = new UnitModel();
            _player.Init(playerModel, _inputService);

            if (_camera != null)
            {
                _camera.SetTarget(_player.transform);
            }

            playerModel.IsDead.Subscribe((isDead) =>
            {
                if (isDead) GameOver(false); 
            });

            _wavePresenter.OnLevelCleared += () => 
            {
                GameOver(true); 
            };

            if (_level != null)
            {
                _wavePresenter.Init(_level);
            }
        }

        private void GameOver(bool playerWon)
        {
            _inputService.enabled = false;

            if (playerWon)
            {
                OnGameWin?.Invoke();
                Debug.Log("Player Win!");
            }
            else
            {
                OnGameLose?.Invoke();
                Debug.Log("Player Lose!");
            }
        }
    }
}