using Assets.Scenes.GoblinClickerScene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace GoblinClicker
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
        private PlayerController _playerController;
        [SerializeField]
        private EnemyController _enemyController;
        [SerializeField]
        private TextMeshProUGUI _scoreText;
        [SerializeField]
        private TextMeshProUGUI _recordScoreText;
        [SerializeField]
        private Button _loadButton;
        [SerializeField]
        private Button _saveButton;

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
            var keys = new HashSet<string>() { "RecordScore" };
            var data = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);
            if (data.TryGetValue(keys.First(), out var item))
            {
                _RecordScore = item.Value.GetAs<int>();
                _playerSaveDataDirtyFlag = false;
            }
        }
        async Task SavePlayerDataAsync()
        {
            var data = new Dictionary<string, object>
            {
                { "RecordScore", _RecordScore }
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