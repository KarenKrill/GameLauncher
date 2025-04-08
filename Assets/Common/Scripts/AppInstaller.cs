using Assets.Common.Scripts;
using Zenject;

public class AppInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IContentProvider>().To<AddressablesContentProvider>().FromNew().AsSingle();
        Container.Bind<IDataProvider>().To<CloudSaveDataProvider>().FromNew().AsSingle();
#if UNITY_EDITOR
        UnityEngine.Cursor.SetCursor(UnityEditor.PlayerSettings.defaultCursor, UnityEngine.Vector2.zero, UnityEngine.CursorMode.ForceSoftware);
#endif
    }
}
