using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;
using Resistance.Sprite;

namespace Resistance.Input
{
    public abstract class AbstractInput : Components.IComponent, IDrawable
    {
        public class InputMapping
        {
            public Rectangle? Rectangle { get; }
            public Windows.Gaming.Input.GamepadButtons GamepadButton { get; }
            public Microsoft.Xna.Framework.Input.Keys KeyboardButton { get; }
            public String ButtonGraphic { get; }

            public InputMapping(Rectangle? rec = null, Windows.Gaming.Input.GamepadButtons gamepad = Windows.Gaming.Input.GamepadButtons.None, Microsoft.Xna.Framework.Input.Keys keyboard = Microsoft.Xna.Framework.Input.Keys.None, string buttonGraphic = null)
            {
                Rectangle = rec;
                GamepadButton = gamepad;
                KeyboardButton = keyboard;
                ButtonGraphic = buttonGraphic;
            }

        }

        RectButton[] buttons;


        protected InputMapping[] recs;

        protected BitArray pressed;
        protected BitArray hold;
        protected BitArray released;

        public Type this[int index]
        {
            get
            {
                return (Type)((hold[index] ? 2 : 1) | (released[index] ? 4 : 0) | (pressed[index] ? 8 : 0));
            }
        }


        public AbstractInput(params InputMapping[] recs)
        {
            this.recs = recs;
            pressed = new BitArray(recs.Length);
            hold = new BitArray(recs.Length);
            released = new BitArray(recs.Length);


            var buttonRectangels = recs.Where(x => x.Rectangle.HasValue).Select(x => new { Rect = x.Rectangle.Value, Graphic = x.ButtonGraphic }).ToArray();
            buttons = new RectButton[buttonRectangels.Length];

            for (int i = 0; i < buttonRectangels.Length; i++)
            {
                buttons[i] = new RectButton(buttonRectangels[i].Rect, buttonRectangels[i].Graphic);
            }


        }



        public virtual void Initilize()
        {
            foreach (var r in buttons)
            {
                r.Initilize();
            }
        }

        public virtual void Update(GameTime gametime)
        {
            BitArray newDirection = GetActualInput();

            pressed = new BitArray(hold).Xor(newDirection).And(newDirection);
            released = new BitArray(hold.Xor(newDirection).And(new BitArray(newDirection).Not()));
            hold = newDirection;


            foreach (var item in buttons)
            {
                item.Update(gametime);
            }


        }

        private BitArray GetActualInput()
        {
            var scaleMatrix =  Matrix.CreateTranslation(-Game1.instance.GraphicsDevice.Viewport.X, -Game1.instance.GraphicsDevice.Viewport.Y, 0f)* Matrix.Invert(Game1.instance.ScaleMatrix);

            var touchPoints = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState().Select(x => Vector2.Transform(x.Position, scaleMatrix));
            var gamepad = Windows.Gaming.Input.Gamepad.Gamepads.FirstOrDefault()?.GetCurrentReading();
            var keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            BitArray bit = new BitArray(recs.Length);

            bool? shouldbeVisible = null;

            if (touchPoints.Any())
                shouldbeVisible = true;
            if (gamepad.HasValue && gamepad.Value.Buttons != Windows.Gaming.Input.GamepadButtons.None)
                shouldbeVisible = false;
            if (keyboard.GetPressedKeys().Any())
                shouldbeVisible = false;

            if (shouldbeVisible.HasValue)
                visible = shouldbeVisible.Value;

            for (int i = 0; i < recs.Length; i++)
            {
                if (recs[i].Rectangle.HasValue)
                    foreach (var t in touchPoints)
                        bit[i] |= recs[i].Rectangle.Value.Contains(new Point((int)t.X, (int)t.Y));
                if (gamepad.HasValue && recs[i].GamepadButton != Windows.Gaming.Input.GamepadButtons.None)
                    bit[i] |= gamepad.Value.Buttons.HasFlag(recs[i].GamepadButton);
                if (recs[i].KeyboardButton != Microsoft.Xna.Framework.Input.Keys.None)
                    bit[i] |= keyboard.IsKeyDown(recs[i].KeyboardButton);
            }


            return bit;
        }





        [Flags]
        public enum Type
        {
            None = 1,
            Hold = 2,
            Release = 4 | 1,
            Press = 8 | 2
        }


        private int drawOrder;
        public int DrawOrder
        {
            get { return drawOrder; }
            set
            {
                if (value != drawOrder)
                {
                    drawOrder = value;
                    DrawOrderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler<EventArgs> DrawOrderChanged;

        private bool visible = true;
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (value != visible)
                {
                    visible = value;
                    VisibleChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler<EventArgs> VisibleChanged;

        public void Draw(GameTime gameTime)
        {
            if (Visible)
                for (int i = 0; i < recs.Length; i++)
                {
                    buttons[i].Draw(gameTime);
                };
        }


    }
}
