using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Resistance.Musicplayer;
using Microsoft.Xna.Framework.Input.Touch;
using Resistance.Input;

namespace Resistance.Scene
{
    class TitleScene : IScene
    {
        Texture2D background;

        SpriteBatch batch;

        Song techno;
        private GameScene.GameTombstone? tombstone;

        public SpriteMenueInput Menue { get; }

        public TitleScene(GameScene.GameTombstone? tombstone) : this()
        {
            this.tombstone = tombstone;
        }

        public TitleScene()
        {
            Menue = new Input.SpriteMenueInput(5, new Vector2(GameScene.VIEWPORT_WIDTH / 2, 200),
                            new Sprite.Sprite<TitleScene>("Animation/Difficulties/ExplorationTiles", this)
                            {
                                CurrentAnimation = new Sprite.Animation(Point.Zero, 4, 4, 15, 280, 75, 0.05)
                            },
                            new Sprite.Sprite<TitleScene>("Animation/Difficulties/InvasionTiles", this)
                            {
                                CurrentAnimation = new Sprite.Animation(Point.Zero, 4, 4, 15, 280, 75, 0.05)
                            }, new Sprite.Sprite<TitleScene>("Animation/Difficulties/ExterminationTiles", this)
                            {
                                CurrentAnimation = new Sprite.Animation(Point.Zero, 2, 2, 560 / 2, 150 / 2, 0.05)
                            });

        }

        public void Initilize()
        {
            batch = Game1.instance.spriteBatch;
            Game1.instance.QueuLoadContent(@"Menue\ResTitelWP7", (Texture2D t) => background = t);
            Game1.instance.QueuLoadContent(@"Music\techno", (Song t) => techno = t);
            Menue.Initilize();
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            var gamepad = Windows.Gaming.Input.Gamepad.Gamepads.FirstOrDefault()?.GetCurrentReading();
            var keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            var gesture = TouchPanel.IsGestureAvailable ? TouchPanel.ReadGesture() : null as GestureSample?;

            Menue.Update(gameTime);


            if (this.tombstone != null)
                Game1.instance.SwitchToScene(new GameScene(tombstone.Value));

            if (Menue.MenueEntryChoosen[0])
                Game1.instance.SwitchToScene(new GameScene(GameScene.Dificulty.Easy));
            if (Menue.MenueEntryChoosen[1])
                Game1.instance.SwitchToScene(new GameScene(GameScene.Dificulty.Medium));
            if (Menue.MenueEntryChoosen[2])
                Game1.instance.SwitchToScene(new GameScene(GameScene.Dificulty.Hard));
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            batch.Begin(transformMatrix: Game1.instance.ScaleMatrix);

            batch.Draw(background, Vector2.Zero, Color.White);
            Menue.Draw(gameTime);

            batch.End();
        }

        public void DoneLoading()
        {
            MusicManager.PlaySong(techno);
        }
    }
}
