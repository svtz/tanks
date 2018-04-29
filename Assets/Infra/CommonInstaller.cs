using svtz.Tanks.Common;
using Zenject;

namespace svtz.Tanks.Infra
{
    internal sealed class CommonInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TeamManager>().AsSingle().NonLazy();
        }
    }
}
