using UnityEngine;

using Assets.Common.Scripts;

namespace Assets.Scenes.LauncherScene
{
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
}
