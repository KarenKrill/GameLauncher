using Assets.Common.Scripts;
using UnityEngine;

public class LauncherStartup : MonoBehaviour
{
    [SerializeField]
    GamesViewer _gamesViewer;
    [SerializeField]
    AppContext _appContext;
    private async void Awake()
    {
        await _appContext.Init();
        _gamesViewer.Init(_appContext.ContentProvider);
    }
}
