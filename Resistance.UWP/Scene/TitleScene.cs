using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Resistance.Musicplayer;
using Microsoft.Xna.Framework.Input.Touch;

namespace Resistance.Scene
{
    class TitleScene : IScene
    {
        Texture2D background;

        SpriteBatch batch;

        Song techno;
        private GameScene.GameTombstone? tombstone;

        public TitleScene(GameScene.GameTombstone? tombstone)
        {
            this.tombstone = tombstone;
        }

        public TitleScene()
        {
        }

        public void Initilize()
        {
            batch = Game1.instance.spriteBatch;
            Game1.instance.QueuLoadContent(@"Menue\ResTitelWP7", (Texture2D t) => background = t);
            Game1.instance.QueuLoadContent(@"Music\techno", (Song t) => techno = t);

        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            var gamepad = Windows.Gaming.Input.Gamepad.Gamepads.FirstOrDefault()?.GetCurrentReading();
            var keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            var gesture = TouchPanel.IsGestureAvailable ? TouchPanel.ReadGesture() : null as GestureSample?;

            {




                if (gesture?.GestureType == GestureType.Tap
                    || (gamepad?.Buttons ?? Windows.Gaming.Input.GamepadButtons.None) != Windows.Gaming.Input.GamepadButtons.None
                    || keyboard.GetPressedKeys().Any())
                {
                    if (this.tombstone != null)
                        Game1.instance.SwitchToScene(new GameScene(tombstone.Value));
                    else
                        Game1.instance.SwitchToScene(new GameScene(GameScene.Dificulty.Easy));


                }
            }

        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            batch.Begin(transformMatrix: Game1.instance.ScaleMatrix);

            batch.Draw(background, Vector2.Zero, Color.White);

            batch.End();
        }

        public void DoneLoading()
        {
            MusicManager.PlaySong(techno);
        }
    }
}
