using svtz.Tanks.Infra;
using Zenject;

namespace svtz.Tanks.BattleStats
{
    internal sealed class BattleStatsInstaller : Installer<BattleStatsInstaller>
    {
        public override void InstallBindings()
        {
            Container.DeclareSignal<BattleStatsUpdateSignal>();
            Container.Bind<BattleStatsUpdateSignal.ServerToClient>().AsSingle().WithArguments(MessageCodes.BattleStats).NonLazy();
            Container.BindInterfacesAndSelfTo<BattleStatsManager>().AsSingle().NonLazy();
        }
    }
}
