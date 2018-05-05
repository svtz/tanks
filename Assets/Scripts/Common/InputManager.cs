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
            return Input.GetButton("TurretLeft");
        }

        public bool TurretRight()
        {
            return Input.GetButton("TurretRight");
        }

        public bool TurretUp()
        {
            return Input.GetButton("TurretUp");
        }
        public bool TurretDown()
        {
            return Input.GetButton("TurretDown");
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
