using Zenject;

namespace svtz.Tanks.Assets.Scripts.Common
{
    internal sealed class CommonInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<TeamManager>().AsSingle().NonLazy();
        }
    }
}
