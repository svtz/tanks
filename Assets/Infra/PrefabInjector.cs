using UnityEngine;

namespace svtz.Tanks.Infra
{
    public class PrefabInjector : MonoBehaviour
    {
        private void Awake()
        {
            PrefabInjectorInstaller.InjectGameObject(gameObject);
            Destroy(this);
        }
    }
}