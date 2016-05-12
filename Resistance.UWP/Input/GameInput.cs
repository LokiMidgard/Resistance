using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Resistance.Sprite;

namespace Resistance.Input
{
    public class GameInput : AbstractInput
    {


        public GameInput()
            : base(
                  new InputMapping(
                        new Rectangle(50, 400, 130, 100),
                        Windows.Gaming.Input.GamepadButtons.DPadLeft,
                        Microsoft.Xna.Framework.Input.Keys.Left,
                        "Buttons/Left")
                  , new InputMapping(
                        new Rectangle(400, 400, 130, 100),
                        Windows.Gaming.Input.GamepadButtons.DPadRight,
                        Microsoft.Xna.Framework.Input.Keys.Right,
                        "Buttons/Right")
                  , new InputMapping(
                        new Rectangle(800 - 80, 460 - 250, 100, 130),
                        Windows.Gaming.Input.GamepadButtons.DPadUp,
                        Microsoft.Xna.Framework.Input.Keys.Up,
                        "Buttons/Up")
                  , new InputMapping(
                        new Rectangle(800 - 80, 460 - 100, 100, 130),
                        Windows.Gaming.Input.GamepadButtons.DPadDown,
                        Microsoft.Xna.Framework.Input.Keys.Down,
                        "Buttons/Down")
                  , new InputMapping(
                        new Rectangle(200, 400, 180, 100),
                        Windows.Gaming.Input.GamepadButtons.A,
                        Microsoft.Xna.Framework.Input.Keys.LeftControl,
                        "Buttons/Fire")
                  , new InputMapping(
                        new Rectangle(800 - 80, 460 - 500, 100, 130),
                        Windows.Gaming.Input.GamepadButtons.Y,
                        Microsoft.Xna.Framework.Input.Keys.Space,
                        "Buttons/Bomb")
                  )
        {

        }

        public Type Left => this[0];
        public Type Right => this[1];
        public Type Up => this[2];
        public Type Down => this[3];
        public Type Fire => this[4];
        public Type Bomb => this[5];






    }
}
