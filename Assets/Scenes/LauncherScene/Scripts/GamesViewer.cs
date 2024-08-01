using System.Collections.Generic;

using UnityEngine;

using Assets.Common.Scripts.Configs;
using Assets.Common.Scripts;

public class GamesViewer : MonoBehaviour
{
    [SerializeField]
    GameObject _gameLoadEntryPrefab;
    [SerializeField]
    Transform _gameLoadEntriesContainerTransform;
    [SerializeField]
    GameDatabase _gameDatabase;
    IContentProvider _gameContentLoader;
    public void Init(IContentProvider gameContentLoader)
    {
        _gameContentLoader = gameContentLoader;
        foreach (var gameInfo in _gameDatabase.GamesList)
        {
            if (gameInfo != null)
            {
                var gameLoadEntryObject = Instantiate(_gameLoadEntryPrefab, _gameLoadEntriesContainerTransform);
                if (gameLoadEntryObject.TryGetComponent<GameLoadEntry>(out var gameLoadEntry))
                {
                    gameLoadEntry.Init(_gameContentLoader, gameInfo);
                }
                else Destroy(gameLoadEntryObject);
            }
        }
    }
}
