using UnityEngine;

using Assets.Common.Scripts;
using Assets.Common.Scripts.Configs;
using Zenject;

namespace Assets.Scenes.LauncherScene
{
    public class GamesViewer : MonoBehaviour
    {
        [SerializeField]
        GameObject _gameLoadEntryPrefab;
        [SerializeField]
        Transform _gameLoadEntriesContainerTransform;
        [SerializeField]
        GameDatabase _gameDatabase;
        IContentProvider _contentProvider;
        [Inject]
        public void Init(IContentProvider contentProvider)
        {
            _contentProvider = contentProvider;
        }
        public void Awake()
        {
            foreach (var gameInfo in _gameDatabase.GamesList)
            {
                if (gameInfo != null)
                {
                    var gameLoadEntryObject = Instantiate(_gameLoadEntryPrefab, _gameLoadEntriesContainerTransform);
                    if (gameLoadEntryObject.TryGetComponent<GameLoadEntry>(out var gameLoadEntry))
                    {
                        gameLoadEntry.Init(_contentProvider, gameInfo);
                    }
                    else Destroy(gameLoadEntryObject);
                }
            }
        }
    }
}