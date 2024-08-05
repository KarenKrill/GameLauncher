using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;
using Assets.Common.Scripts;
using Zenject;

namespace Assets.Scenes.DoozyCatcherScene
{
    public class GameManager : MonoBehaviour
    {
        IDataProvider _dataProvider;
        [Inject]
        public void Init(IDataProvider dataProvider)
        {
            Debug.Log($"{nameof(GameManager)}.{nameof(Init)}");
            Debug.Log($"DataProvider: {dataProvider.GetType()}");
            _dataProvider = dataProvider;
        }
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
            if (_dataProvider != null)
            {
                var metadata = new Dictionary<string, Type>() { { "BestTime", typeof(float) } };
                var data = await _dataProvider.LoadAsync(metadata);
                if (data != null && data.TryGetValue("BestTime", out var value))
                {
                    _BestTime = (float)value;
                    _playerSaveDataDirtyFlag = false;
                }
            }
            else
            {
                Debug.LogError($"{nameof(IDataProvider)} not found (save/load function not works)");
            }
        }
        async Task SavePlayerDataAsync()
        {
            var data = new Dictionary<string, object>
            {
                { "BestTime", _BestTime }
            };
            if (_dataProvider != null)
            {
                await _dataProvider.SaveAsync(data);
            }
            else
            {
                Debug.LogError($"{nameof(IDataProvider)} not found (save/load function not works)");
            }
        }
        [SerializeField]
        float _autoSavePeriodTimeMs = 500;
        float _lastAutoSaveEllapsedTimeMs = 0;
        bool _playerSaveDataDirtyFlag = false;

        async void Awake()
        {
            Debug.Log($"{nameof(GameManager)}.{nameof(Awake)}");
            if (_dataProvider is IRemoteDataProvider remoteDataProvider)
            {
                if (remoteDataProvider.IsSignInNeeded && !remoteDataProvider.IsSignedIn)
                {
                    await remoteDataProvider.SignInAnonymouslyAsync();
                }
            }
        }
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