using UnityEngine;
using Zenject;

namespace svtz.Tanks.Infra
{
    internal sealed class PrefabInjectorInstaller : MonoInstaller<PrefabInjectorInstaller>
    {
        private static DiContainer SceneContainer { get; set; }

        public override void InstallBindings()
        {
            SceneContainer = Container;
        }

        public static void InjectGameObject(GameObject gameObject)
        {
            SceneContainer.InjectGameObject(gameObject);
        }
    }
}