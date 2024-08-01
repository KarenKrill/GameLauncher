using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

using Assets.Common.Scripts.Configs;
using Assets.Common.Scripts.Utils.Convert;
using Assets.Common.Scripts;

public class GameLoadEntry : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameText;
    [SerializeField]
    private Image _iconImage;
    [SerializeField]
    private Button _playButton;
    [SerializeField]
    private Button _loadButton;
    [SerializeField]
    private Button _unloadButton;
    [SerializeField]
    private Slider _loadSlider;
    [SerializeField]
    private GameObject _loadSliderFiller;
    [SerializeField]
    private TextMeshProUGUI _loadedSizeText;
    [SerializeField]
    private TextMeshProUGUI _totalSizeText;

    IContentProvider _contentLoader = null;
    GameInfo _gameInfo;
    float _loadProgress;
    float _targetLoadProgress;
    float? _startLoadProgress = null;
    long _totalBytes = 0;
    bool _isCached = false;
    /// <summary>
    /// Range: [0,1]
    /// </summary>
    float _LoadProgress
    {
        get => _loadProgress;
        set
        {
            if (_loadProgress != value)
            {
                value = Mathf.Clamp01(value);
                if (_loadProgress != value)
                {
                    if (_loadSlider != null)
                    {
                        _loadSlider.value = Mathf.Lerp(_loadSlider.minValue, _loadSlider.maxValue, value);
                    }
                    if (_loadSliderFiller != null)
                    {
                        _loadSliderFiller.SetActive(value != 0);
                    }
                    if (_loadedSizeText != null)
                    {
                        _loadedSizeText.text = HumanReadableFormats.FormatBytes((long)(_totalBytes * value));
                    }
                    _loadProgress = value;
                }
            }
        }
    }
    long _TotalBytes
    {
        get => _totalBytes;
        set
        {
            if (_totalBytes != value)
            {
                _totalBytes = value;
                if (_totalSizeText != null)
                {
                    _totalSizeText.text = HumanReadableFormats.FormatBytes(value);
                }
                if (_loadedSizeText != null)
                {
                    _loadedSizeText.text = HumanReadableFormats.FormatBytes(0);
                }
            }
        }
    }
    bool _IsCached
    {
        get => _isCached;
        set
        {
            _isCached = value;
            if (_playButton != null)
            {
                _playButton.interactable = _isCached;
            }
            if (_loadButton != null)
            {
                _loadButton.interactable = !_isCached;
            }
            if (_unloadButton != null)
            {
                _unloadButton.interactable = _isCached;
            }
            if (_totalSizeText != null)
            {
                var parent = _totalSizeText.transform.parent.gameObject;
                if (parent != null)
                {
                    parent.SetActive(!_isCached);
                }
            }
        }
    }
    async Task UpdateCacheInfoTask()
    {
        bool isCached = await _contentLoader.IsCachedAsync(_gameInfo.ContentId);
        if (isCached)
        {
            _LoadProgress = 1;
        }
        else
        {
            _TotalBytes = await _contentLoader.GetDownloadSizeAsync(_gameInfo.ContentId);
        }
        _IsCached = isCached;
    }
    public void Init(IContentProvider gameContentLoader, GameInfo gameInfo)
    {
        _contentLoader = gameContentLoader;
        _gameInfo = gameInfo;
        if (_nameText != null)
        {
            _nameText.text = _gameInfo.Name;   
        }
        if (_iconImage != null)
        {
            _iconImage.sprite = _gameInfo.Icon;
        }
        _ = UpdateCacheInfoTask();
        if (enabled)
        {
            UnsubscribeFromEvents();
            SubscribeOnEvents();
        }
    }
    #region Button event handlers
    private void OnLoadButtonClick()
    {
        _loadButton.interactable = false;
        _targetLoadProgress = 1;
        _startLoadProgress = null;
        var coroutine = StartCoroutine(_contentLoader.DownloadContentCoroutine(_gameInfo.ContentId));
    }
    private void OnUnloadButtonClick()
    {
        _unloadButton.interactable = false;
        _targetLoadProgress = 0;
        _startLoadProgress = null;
        var coroutine = StartCoroutine(_contentLoader.UnloadContentCoroutine(_gameInfo.ContentId));
    }
    private void OnPlayButtonClick()
    {
        _playButton.interactable = false;
        _targetLoadProgress = 1;
        _startLoadProgress = null;
        var coroutine = StartCoroutine(_contentLoader.LoadSceneCoroutine(_gameInfo.ContentId));
    }
    #endregion
    #region ContentLoader event handlers
    private void OnContentOperationProgressChanged(string contentId, float progress)
    {
        if (contentId == _gameInfo.ContentId)
        {
            _startLoadProgress ??= progress;
            progress = Mathf.InverseLerp(_startLoadProgress.Value, _targetLoadProgress, progress);
            _LoadProgress = progress;
        }
    }
    private void OnContentDownloadCompleted(string contentId, OperationResult result)
    {
        if (contentId == _gameInfo.ContentId)
        {
            if (!result.Succeeded)
            {
                _loadButton.interactable = true;
                _IsCached = false;
                _LoadProgress = 0;
                Debug.LogError($"Loading \"{contentId}\" failed with {result.Exception.GetType()}: {result.Exception}");
            }
            else
            {
                _IsCached = true;
            }
        }
    }
    static IEnumerator ActivateSceneCoroutine(SceneInstance sceneInstance)
    {
        var asyncOperation = sceneInstance.ActivateAsync();
        yield return asyncOperation;
    }
    private void OnLoadSceneCompleted(string contentId, OperationResult<SceneInstance> result)
    {
        if (contentId == _gameInfo.ContentId)
        {
            if (result.Succeeded)
            {
                var coroutine = StartCoroutine(ActivateSceneCoroutine(result.Result));
            }
            else
            {
                _loadButton.interactable = true;
                Debug.LogError("Scene can't be loaded!");
            }
        }
    }
    private async void OnContentUnloadCompleted(string contentId, OperationResult result)
    {
        if (contentId == _gameInfo.ContentId)
        {
            if (!result.Succeeded)
            {
                _unloadButton.interactable = true;
                Debug.LogError($"Unloading \"{contentId}\" failed with {result.Exception.GetType()}: {result.Exception}");
            }
            else
            {
                _IsCached = false;
                _TotalBytes = await _contentLoader.GetDownloadSizeAsync(contentId);
            }
        }
    }
    #endregion
    private void SubscribeOnEvents()
    {
        if (_loadButton != null)
        {
            _loadButton.onClick.AddListener(OnLoadButtonClick);
        }
        if (_unloadButton != null)
        {
            _unloadButton.onClick.AddListener(OnUnloadButtonClick);
        }
        if (_playButton != null)
        {
            _playButton.onClick.AddListener(OnPlayButtonClick);
        }
        if (_contentLoader != null)
        {
            _contentLoader.ProgressChanged += OnContentOperationProgressChanged;
            _contentLoader.DownloadCompleted += OnContentDownloadCompleted; ;
            _contentLoader.LoadSceneCompleted += OnLoadSceneCompleted; ;
            _contentLoader.UnloadCompleted += OnContentUnloadCompleted;
        }
    }
    private void UnsubscribeFromEvents()
    {
        if (_loadButton != null)
        {
            _loadButton.onClick.RemoveListener(OnLoadButtonClick);
        }
        if (_unloadButton != null)
        {
            _unloadButton.onClick.RemoveListener(OnUnloadButtonClick);
        }
        if (_playButton != null)
        {
            _playButton.onClick.RemoveListener(OnPlayButtonClick);
        }
        if (_contentLoader != null)
        {
            _contentLoader.ProgressChanged -= OnContentOperationProgressChanged;
            _contentLoader.DownloadCompleted -= OnContentDownloadCompleted; ;
            _contentLoader.LoadSceneCompleted -= OnLoadSceneCompleted; ;
            _contentLoader.UnloadCompleted -= OnContentUnloadCompleted;
        }
    }
    private void OnEnable() => SubscribeOnEvents();
    private void OnDisable() => UnsubscribeFromEvents();
}
