using UnityEngine;
using UnityEngine.UI;

namespace svtz.Tanks.UserInterface
{
    [RequireComponent(typeof(Button))]
    public sealed class ToggleButton : MonoBehaviour
    {
#pragma warning disable 0649
        public GameObject Toggle;
#pragma warning restore 0649

        public void Reset()
        {
            GetComponent<Button>().onClick.AddListener(OnButtonClick);
        }

        public void OnEnable()
        {
            GetComponent<Button>().onClick.AddListener(OnButtonClick);
        }

        public void OnDisable()
        {
            GetComponent<Button>().onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            var toggleComponent = Toggle.GetComponent<Toggle>();
            if (toggleComponent != null)
                toggleComponent.isOn = !toggleComponent.isOn;
        }
    }
}