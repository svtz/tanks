using UnityEngine;
using UnityEngine.UI;

namespace svtz.Tanks.UserInterface
{
    public class VersionFiller : MonoBehaviour
    {
        private void Start ()
        {
            GetComponent<Text>().text = string.Format("версия {0}", Application.version);
            Destroy(this);
        }
    }
}
