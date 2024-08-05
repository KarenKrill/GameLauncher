using Assets.Common.Scripts;
using Assets.Scenes.LauncherScene;
using Zenject;

public class AppInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IContentProvider>().To<AddressablesContentProvider>().FromNew().AsSingle();
        Container.Bind<IDataProvider>().To<CloudSaveDataProvider>().FromNew().AsSingle();
    }
}
