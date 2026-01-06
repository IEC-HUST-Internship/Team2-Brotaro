using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class GameplayPresenter : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private UnitView _playerPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private InputService _inputService;

    [Header("UI")]
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private Slider _expSlider;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Slider _progressionBar; 
    
    [Header("Popups")]
    [SerializeField] private GameObject _settingsPopup;
    [SerializeField] private GameObject _statusPopup;
    [Header("Level System")]
    [SerializeField] private WavePresenter _wavePresenter; 
    [SerializeField] private LevelData_Brotaro _levelData;        

    private PlayerPresenter _playerPresenter;
    private float _timer;
    private float _bossProgress;
    private bool _isPaused;

    private void Start()
    {
        // 1. Tạo Data
        var playerModel = new UnitModel(speed: 5f, hp: 100, dmg: 10);

        // 2. Tạo Visual
        var playerObj = Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);

        // 3. Gắn Logic
        _playerPresenter = playerObj.GetComponent<PlayerPresenter>();

        // 4. Kết nối (Inject Dependency)
        _playerPresenter.Init(
            playerModel, 
            playerObj, 
            _inputService,
            UpdateHpUI, 
            UpdateExpUI, 
            UpdateLevelUI
        );
        var camScript = FindObjectOfType<MVP_CamControl>();
        if (camScript != null)
        {
            camScript.SetTarget(playerObj.transform);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy script Camera Control trong Scene!");
        }
        _isPaused = false;
        if(_settingsPopup) _settingsPopup.SetActive(false);
        if(_statusPopup) _statusPopup.SetActive(false);

        if(_wavePresenter != null && _levelData != null)
        {
            _wavePresenter.Init(_levelData);
        }
        else
        {
            Debug.LogWarning("WavePresenter hoặc LevelData chưa được gán trong Inspector!");
        }
    }

    private void Update()
    {
        if (_isPaused) return;

        if (_playerPresenter) _playerPresenter.Tick();

        HandleGameTimer();
        HandleProgression();
    }

    // --- UI Update Helpers ---
    private void UpdateHpUI(float cur, float max) 
    {
        if (_hpSlider) _hpSlider.value = cur / max;
    }

    private void UpdateExpUI(float cur, float max) 
    {
        if (_expSlider) _expSlider.value = cur / max;
    }

    private void UpdateLevelUI(int lvl) 
    {
        if (_levelText) _levelText.text = lvl.ToString();
    }

    private void HandleGameTimer()
    {
        _timer += Time.deltaTime;
        int m = Mathf.FloorToInt(_timer / 60);
        int s = Mathf.FloorToInt(_timer % 60);
        if (_timerText) _timerText.text = $"{m:00}:{s:00}";
    }

    private void HandleProgression()
    {
        if (_bossProgress < 1f)
        {
            _bossProgress += Time.deltaTime / 900f; 
            if (_progressionBar) _progressionBar.value = _bossProgress;
        }
    }

    public void OnOpenSettings() => SetPause(true, _settingsPopup);
    public void OnOpenStatus() => SetPause(true, _statusPopup);
    public void OnResume()
    {
        SetPause(false, _settingsPopup);
        SetPause(false, _statusPopup);
    }

    private void SetPause(bool pause, GameObject popup)
    {
        _isPaused = pause;
        Time.timeScale = pause ? 0 : 1;
        if (popup) popup.SetActive(pause);
    }
}