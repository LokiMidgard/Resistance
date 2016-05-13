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
    public abstract class AbstractEnemy : CollidelbleSprite
    {
        public static Texture2D explosion;
        public static SoundEffect bam;

        private bool explosionLoaded = false;

        public static readonly Animation EXPLOAD = new Animation(Point.Zero, 5, 4, 64, 64, 0.1, () => new Vector2(23, 23));


        public bool Dead { get; set; }

        public override void Initilize()
        {
            base.Initilize();
            Dead = false;
            Position = new Vector2(Game1.random.Next(Scene.configuration.WorldWidth), -30 - Game1.random.Next(100));
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

                base.Draw(gameTime); //TODO: Why do I revert this?

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
            CurrentAnimationFrame = 0;
            Position += new Vector2(-64 >> 2, -64 >> 2);
            bam.Play();
        }


        private int lastExplosionFrame = -1;

        public AbstractEnemy(string imageName, GameScene scene, Rectangle collisionRec) : base(imageName, scene, collisionRec)
        {
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            if (Visible && CurrentAnimation == EXPLOAD)
            {
                if (lastExplosionFrame > CurrentAnimationFrame)
                    Visible = false;
                lastExplosionFrame = CurrentAnimationFrame;
            }
        }
    }
}
