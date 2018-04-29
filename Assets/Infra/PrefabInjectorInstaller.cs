using UnityEngine;
using Zenject;

namespace svtz.Tanks.Infra
{
    public class PrefabInjectorInstaller : MonoInstaller<PrefabInjectorInstaller>
    {
        public static DiContainer SceneContainer { get; private set; }

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