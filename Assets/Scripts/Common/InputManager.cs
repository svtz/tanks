using System;
using System.Linq;
using UnityEngine;

namespace svtz.Tanks.Common
{
    internal sealed class InputManager
    {
        private static class Constants
        {
            public const string MoveLeft = "Left";
            public const string MoveRight = "Right";
            public const string MoveUp = "Up";
            public const string MoveDown = "Down";

            public const string Brake = "Brake";

            public const string Fire = "Fire";

            public const string TurretLeft = "TurretLeft";
            public const string TurretUp = "TurretUp";
            public const string TurretRight = "TurretRight";
            public const string TurretDown = "TurretDown";
        }


        public bool Left()
        {
            return Button(Constants.MoveLeft);
        }

        public bool Right()
        {
            return Button(Constants.MoveRight);
        }

        public bool Up()
        {
            return Button(Constants.MoveUp);
        }

        public bool Down()
        {
            return Button(Constants.MoveDown);
        }


        public bool Brake()
        {
            return Button(Constants.Brake);
        }

        public bool Fire()
        {
            return ButtonDown(Constants.Fire);
        }

        public bool TurretLeft()
        {
            return ButtonDown(Constants.TurretLeft)
                && !AnyButton(Constants.TurretDown, Constants.TurretRight, Constants.TurretUp);
        }

        public bool TurrentUpLeft()
        {
            return AllButtons(Constants.TurretUp, Constants.TurretLeft)
                && AnyButtonDown(Constants.TurretUp, Constants.TurretLeft)
                && !AnyButton(Constants.TurretDown, Constants.TurretRight);
        }

        public bool TurretUp()
        {
            return ButtonDown(Constants.TurretUp)
                   && !AnyButton(Constants.TurretDown, Constants.TurretRight, Constants.TurretLeft);
        }

        public bool TurrentUpRight()
        {
            return AllButtons(Constants.TurretUp, Constants.TurretRight)
                   && AnyButtonDown(Constants.TurretUp, Constants.TurretRight)
                   && !AnyButton(Constants.TurretDown, Constants.TurretLeft);
        }

        public bool TurretRight()
        {
            return ButtonDown(Constants.TurretRight)
                   && !AnyButton(Constants.TurretDown, Constants.TurretUp, Constants.TurretLeft);
        }

        public bool TurretDownRight()
        {
            return AllButtons(Constants.TurretDown, Constants.TurretRight)
                   && AnyButtonDown(Constants.TurretDown, Constants.TurretRight)
                   && !AnyButton(Constants.TurretUp, Constants.TurretLeft);
        }

        public bool TurretDown()
        {
            return ButtonDown(Constants.TurretDown)
                   && !AnyButton(Constants.TurretRight, Constants.TurretUp, Constants.TurretLeft);
        }

        public bool TurretDownLeft()
        {
            return AllButtons(Constants.TurretDown, Constants.TurretLeft)
                   && AnyButtonDown(Constants.TurretDown, Constants.TurretLeft)
                   && !AnyButton(Constants.TurretUp, Constants.TurretRight);
        }

        public bool Return()
        {
            return Input.GetButtonDown("Return");
        }

        public bool StatScreen()
        {
            return Input.GetButton("StatScreen");
        }


        #region Сокращённые формы

        private static bool Button(string name)
        {
            return Input.GetButton(name);
        }

        private static bool ButtonDown(string name)
        {
            return Input.GetButtonDown(name);
        }

        private static bool AnyButton(params string[] names)
        {
            return names.Any(Input.GetButton);
        }

        private static bool AnyButtonDown(params string[] names)
        {
            return names.Any(Input.GetButtonDown);
        }

        private static bool AllButtons(params string[] names)
        {
            return names.All(Input.GetButton);
        }

        #endregion
    }
}
