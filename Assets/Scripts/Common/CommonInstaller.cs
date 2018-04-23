using Zenject;

namespace svtz.Tanks.Common
{
    internal sealed class CommonInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TeamManager>().AsSingle().NonLazy();
        }
    }
}
