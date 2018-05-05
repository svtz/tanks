using UnityEngine;

namespace svtz.Tanks.Bonus
{
    internal sealed class BonusImageAnimator : MonoBehaviour
    {
#pragma warning disable 0649
        public float AngleDelta;
        public float AngularSpeed;
#pragma warning restore 0649

        private void Update()
        {
            AnimateRotation();
        }

        private int _sign = 1;

        private void AnimateRotation()
        {
            transform.Rotate(_sign > 0 ? Vector3.forward : Vector3.back, AngularSpeed * Time.deltaTime);

            if (transform.rotation.eulerAngles.z < 360 - AngleDelta
                && transform.rotation.eulerAngles.z > AngleDelta)
            {
                _sign = _sign * -1;
            }
        }
    }
}