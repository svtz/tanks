using Zenject;

namespace svtz.Tanks.Infra
{
    internal sealed class ZenAutoInjecterOnAwake : ZenAutoInjecter
    {
        public void Awake()
        {
            // всё, что требуется - это заставить ZenAutoInjecter сработать чуть раньше
            Start();
            Destroy(this);
        }
    }
}
