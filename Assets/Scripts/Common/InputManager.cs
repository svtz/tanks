using UnityEngine;

namespace svtz.Tanks.Common
{
    internal sealed class InputManager
    {
        public bool Left()
        {
            return Input.GetButton("Left");
        }

        public bool Right()
        {
            return Input.GetButton("Right");
        }

        public bool Up()
        {
            return Input.GetButton("Up");
        }

        public bool Down()
        {
            return Input.GetButton("Down");
        }


        public bool Brake()
        {
            return Input.GetButton("Brake");
        }

        public bool Fire()
        {
            return Input.GetButtonDown("Fire");
        }

        public bool TurretLeft()
        {
            return Input.GetButtonDown("TurretLeft");
        }
        public bool TurretRight()
        {
            return Input.GetButtonDown("TurretRight");
        }

        public bool Return()
        {
            return Input.GetButtonDown("Return");
        }

        public bool StatScreen()
        {
            return Input.GetButton("StatScreen");
        }
    }
}
