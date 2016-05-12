using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Resistance.Scene;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Resistance.Sprite
{
    public abstract class AbstractEnemy : Sprite
    {
        public static Texture2D explosion;
        public static SoundEffect bam;

        private bool explosionLoaded = false;

        public static readonly Animation EXPLOAD = new Animation(Point.Zero, 5, 4, 64, 64);

        public AbstractEnemy(String name, GameScene scene)
            : base(name, scene)
        {
        }
        public bool Dead { get; set; }

        public override void Initilize()
        {
            base.Initilize();
            Dead = false;
            position = new Vector2(Game1.random.Next(scene.configuration.WorldWidth), -30 - Game1.random.Next(100));
            if (!explosionLoaded)
            {
                Game1.instance.QueuLoadContent(@"Animation\ExplosionTiledsSmall", (Texture2D t) => explosion = t);
                Game1.instance.QueuLoadContent(@"Sound\blast", (SoundEffect t) => bam = t);

                explosionLoaded = true;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (Dead)
            {
                var oldImage = Image;
                Image = explosion;

                base.Draw(gameTime);

                Image = oldImage;
            }
            else
                base.Draw(gameTime);


        }

        public virtual void Destroy()
        {
            if (Dead)
                return;
            Dead = true;

            CurrentAnimation = EXPLOAD;
            currentAnimationFrame = 0;
            origion = new Vector2(23, 23);
            position += new Vector2(-64 >> 2, -64 >> 2);
            bam.Play();
        }



        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Visible && CurrentAnimation == EXPLOAD)
            {
                ++currentAnimationFrame;
                if (currentAnimationFrame > CurrentAnimation.Length)
                {
                    Visible = false;
                }
            }
        }
    }
}
