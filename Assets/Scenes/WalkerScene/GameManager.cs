using Assets.Scenes.WalkerScene;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Walker
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Button _menuButton;
        IEnumerator LoadMenuSceneCoroutine()
        {
            var asyncOperation = SceneManager.LoadSceneAsync("LauncherScene");
            yield return asyncOperation;
        }
        void OnMenuButtonClicked()
        {
            StartCoroutine(LoadMenuSceneCoroutine());
        }
        private void OnEnable()
        {
            if (_menuButton != null)
            {
                _menuButton.onClick.AddListener(OnMenuButtonClicked);
            }
        }
        private void OnDisable()
        {
            if (_menuButton != null)
            {
                _menuButton.onClick.RemoveListener(OnMenuButtonClicked);
            }
        }

        [SerializeField]
        PlayerController _playerController;
        [SerializeField]
        private TextMeshProUGUI _timeText;
        [SerializeField]
        private TextMeshProUGUI _bestTimeText;
        float _time = 0;
        float _Time
        {
            get => _time;
            set
            {
                if (_time != value)
                {
                    _time = value;
                    if (_timeText != null)
                    {
                        _timeText.text = $"Time: {_time:.00}s";
                    }
                }
            }
        }
        float _bestTime = 0;
        float _BestTime
        {
            get => _bestTime;
            set
            {
                if (_bestTime != value)
                {
                    _bestTime = value;
                    if (_bestTimeText != null)
                    {
                        _bestTimeText.text = $"Record: {_bestTime:.00}s";
                    }
                    _playerSaveDataDirtyFlag = true;
                }
            }
        }
        async Task LoadPlayerDataAsync()
        {
            var keys = new HashSet<string>() { "BestTime" };
            var data = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);
            if (data.TryGetValue(keys.First(), out var item))
            {
                _BestTime = item.Value.GetAs<float>();
                _playerSaveDataDirtyFlag = false;
            }
        }
        async Task SavePlayerDataAsync()
        {
            var data = new Dictionary<string, object>
            {
                { "BestTime", _BestTime }
            };
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }
        [SerializeField]
        float _autoSavePeriodTimeMs = 500;
        float _lastAutoSaveEllapsedTimeMs = 0;
        bool _playerSaveDataDirtyFlag = false;

        // Start is called before the first frame update
        async void Start()
        {
            if (_playerController != null)
            {
                _playerController.EnemyCaught += OnEnemyCaught;
            }
            await LoadPlayerDataAsync();
        }

        private void OnEnemyCaught()
        {
            if (_BestTime <= 0 || _time < _BestTime)
            {
                _BestTime = _time;
            }
            _Time = 0;
        }

        async void Update()
        {
            _Time += Time.deltaTime;
            if (_playerSaveDataDirtyFlag)
            {
                _lastAutoSaveEllapsedTimeMs += Time.deltaTime * 1000f;
                if (_lastAutoSaveEllapsedTimeMs > _autoSavePeriodTimeMs)
                {
                    _lastAutoSaveEllapsedTimeMs = 0;
                    await SavePlayerDataAsync();
                    _playerSaveDataDirtyFlag = false;
                }
            }
        }
    }
}