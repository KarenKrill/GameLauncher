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
using Assets.Scenes.LauncherScene;

namespace Assets.Scenes.GoblinClickerScene
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
        private PlayerController _playerController;
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        [SerializeField]
        private TextMeshProUGUI _recordScoreText;

        int _score = 0;
        int _Score
        {
            get => _score;
            set
            {
                if (_score != value)
                {
                    _score = value;
                    if (_scoreText != null)
                    {
                        _scoreText.text = $"Coins: {_score}";
                    }
                    if (_score > _RecordScore)
                    {
                        _RecordScore = _score;
                    }
                }
            }
        }
        int _recordScore = 0;
        int _RecordScore
        {
            get => _recordScore;
            set
            {
                if (_recordScore != value)
                {
                    _recordScore = value;
                    if (_recordScoreText != null)
                    {
                        _recordScoreText.text = $"Record: {_recordScore}";
                    }
                    _playerSaveDataDirtyFlag = true;
                }
            }
        }
        async Task LoadPlayerDataAsync()
        {
            if (_dataProvider != null)
            {
                var metadata = new Dictionary<string, Type>() { { "RecordScore", typeof(int) } };
                var data = await _dataProvider.LoadAsync(metadata);
                if (data != null && data.TryGetValue("RecordScore", out var value))
                {
                    _RecordScore = (int)value;
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
                { "RecordScore", _RecordScore }
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
            if (_playerController != null)
            {
                _playerController.HitEnemyEvent += () => _Score++;
            }
            await LoadPlayerDataAsync();
        }
        async void Update()
        {
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