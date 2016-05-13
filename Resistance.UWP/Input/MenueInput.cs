using System.Linq;
using Resistance.Scene;
using Resistance.Components;
using Microsoft.Xna.Framework;
using Resistance.Sprite;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace Resistance.Input
{
    class SpriteMenueInput : IDrawableComponent
    {
        private readonly Sprite.ISprite<IScene>[] sprites;

        private readonly Selector Curser;
        private readonly float marginTop;

        public int SelectedIndex { get; set; }

        public Vector2 Position { get; }

        public bool[] MenueEntryChoosen { get; }

        private Vector2[] menueItemOffsets;
        private BaseInput input;

        public SpriteMenueInput(float marginTop, Vector2 postiont, params Sprite.ISprite<IScene>[] sprites)
        {
            this.sprites = sprites;
            System.Diagnostics.Debug.Assert(sprites.All(x => x.CurrentAnimation != null && x.CurrentAnimation == x.CurrentAnimation?.NextAnimation), "All Animations should loop themselves.");
            this.marginTop = marginTop;
            this.Curser = new Selector(sprites.First().Scene);
            MenueEntryChoosen = new bool[sprites.Length];
            Position = postiont;
        }

        public bool Visible { get; set; }

        public void Draw(GameTime gameTime)
        {
            if (Visible)
            {
                foreach (var s in sprites)
                    s.Draw(gameTime);
                this.Curser.Draw(gameTime);
            }
        }

        public void Initilize()
        {
            this.Curser.Initilize();
            foreach (var s in sprites)
                s.Initilize();

            menueItemOffsets = new Vector2[sprites.Length];
            var list = new List<BaseInput.InputMapping>();
            for (int i = 0; i < sprites.Length; i++)
            {
                var animation = sprites[i].CurrentAnimation;
                menueItemOffsets[i] = new Vector2(0, marginTop + animation.FrameHeight / 2) + (i > 0 ? (menueItemOffsets[i - 1] + new Vector2(0, sprites[i - 1].CurrentAnimation.FrameHeight / 2)) : Vector2.Zero);

                var animationSize = new Vector2(animation.FrameWidth, animation.FrameHeight);
                list.Add(new BaseInput.InputMapping(new Rectangle((menueItemOffsets[i] - animationSize / 2 + Position).ToPoint(), animationSize.ToPoint())));

            }

            list.Add(new BaseInput.InputMapping(
                gamepad: Windows.Gaming.Input.GamepadButtons.DPadUp,
                keyboard: Microsoft.Xna.Framework.Input.Keys.Up));
            list.Add(new BaseInput.InputMapping(
                gamepad: Windows.Gaming.Input.GamepadButtons.DPadDown,
                keyboard: Microsoft.Xna.Framework.Input.Keys.Down));
            list.Add(new BaseInput.InputMapping(
                gamepad: Windows.Gaming.Input.GamepadButtons.A,
                keyboard: Microsoft.Xna.Framework.Input.Keys.Enter));



            input = new BaseInput(list.ToArray());


            Visible = true;

            input.Initilize();

        }

        public void Update(GameTime gameTime)
        {
            var gamepad = Windows.Gaming.Input.Gamepad.Gamepads.FirstOrDefault()?.GetCurrentReading();
            var keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            var gesture = TouchPanel.IsGestureAvailable ? TouchPanel.ReadGesture() : null as GestureSample?;

            input.Update(gameTime);


            if (input[this.menueItemOffsets.Length].HasFlag(BaseInput.Type.Press)) // up
                this.SelectedIndex--;
            else if (input[this.menueItemOffsets.Length + 1].HasFlag(BaseInput.Type.Press)) // down
                this.SelectedIndex++;
            else if (input[this.menueItemOffsets.Length + 2].HasFlag(BaseInput.Type.Press)) // enter
                MenueEntryChoosen[SelectedIndex] ^= true;
            for (int i = 0; i < menueItemOffsets.Length; i++)
            {
                if (input[i].HasFlag(BaseInput.Type.Press))
                    MenueEntryChoosen[i] ^= true;
            }


            while (SelectedIndex < 0)
                SelectedIndex += this.menueItemOffsets.Length;
            SelectedIndex %= this.menueItemOffsets.Length;


            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].Update(gameTime);
                if (i != SelectedIndex)
                {
                    sprites[i].CurrentAnimationFrame = 0;
                }
                else
                {
                    this.Curser.Position = this.Position + menueItemOffsets[i] - new Vector2(sprites[i].CurrentAnimation.FrameWidth, 0);
                }
                sprites[i].Position = this.Position + menueItemOffsets[i];
            }

            this.Curser.Update(gameTime);

        }



        class Selector : Sprite.Sprite<IScene>
        {
            public Selector(IScene scene) : base("Animation/Difficulties/ArrowTiles", scene)
            {
                CurrentAnimation = new Animation(Point.Zero, 3, 3, 840 / 3, 225 / 3, 0.05f);
            }

        }
    }
}
