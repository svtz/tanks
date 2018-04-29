using UnityEngine;

namespace svtz.Tanks.Infra
{
    internal sealed class PrefabInjector : MonoBehaviour
    {
        private void Awake()
        {
            PrefabInjectorInstaller.InjectGameObject(gameObject);
            Destroy(this);
        }
    }
}