using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Resistance.Scene;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Resistance.Sprite
{
    public class Human : CollidelbleSprite
    {



        private static Texture2D image;

        public override Texture2D Image
        {
            get { return image; }
            set { image = value; }
        }

        public EnemyCollector isCapturedBy;

        private static bool soundLoaded;

        public bool IsCaptured { get { return isCapturedBy != null; } }

        Direction direction;

        public static SoundEffect screem;



        public static readonly Animation WALK = new Animation(Point.Zero, 2, 2, 24, 24, 0.2f, animation => new Vector2(animation.FrameWidth / 2, 0));
        public static readonly Animation STAND = new Animation(Point.Zero, 1, 1, 24, 24, 0.2f, animation => new Vector2(animation.FrameWidth / 2, 0));


        public Human(GameScene scene)
            : base(@"Animation\NewManTiles", scene, new Rectangle(-12, 0, 24, 24))
        {

            Position = new Vector2(Game1.random.Next(scene.configuration.WorldWidth), scene.configuration.WorldHeight - 24);
        }


        public override void Initilize()
        {
            if (!Visible)
                Position = new Vector2(Game1.random.Next(Scene.configuration.WorldWidth), Scene.configuration.WorldHeight - 24);

            base.Initilize();
            CurrentAnimation = STAND;
            direction = Direction.None;
            if (!soundLoaded)
            {
                Game1.instance.QueuLoadContent(@"Sound\scream", (SoundEffect s) => screem = s);
                soundLoaded = true;
            }
        }

        internal void Die()
        {
            Visible = false;

            Scene.notKilledHumans.Remove(this);
            if (IsCaptured)
            {
                isCapturedBy.target = null;
                isCapturedBy = null;
            }
            Scene.score -= 25;
            screem.Play();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 movment = new Vector2();
            if (IsCaptured)
            {
                CurrentAnimation = STAND;
                direction = Direction.None;
                CurrentAnimationFrame = 0;
                Position = new Vector2(isCapturedBy.Position.X, Math.Min(Scene.configuration.WorldWidth - 24, isCapturedBy.Position.Y));
            }
            else if (Position.Y < Scene.configuration.WorldHeight - 24)
            {
                movment += new Vector2(0, 1);
            }
            else
            {
                int newDirection = Game1.random.Next(40);
                if (newDirection < 3 && direction != (Direction)newDirection)
                {
                    direction = (Direction)newDirection;
                    switch (direction)
                    {
                        case Direction.None:
                            CurrentAnimation = STAND;
                            CurrentAnimationFrame = 0;
                            break;
                        case Direction.Right:
                            CurrentAnimation = WALK;
                            CurrentAnimationFrame = 1;
                            SpriteEfekt = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
                            break;
                        case Direction.Left:
                            CurrentAnimation = WALK;
                            SpriteEfekt = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                            CurrentAnimationFrame = 1;
                            break;
                    }
                }

                switch (direction)
                {

                    case Direction.Right:
                        movment += new Vector2(1, 0);
                        break;
                    case Direction.Left:
                        movment -= new Vector2(1, 0);
                        break;
                }
            }
            Position += (movment * 16 * (float)gameTime.ElapsedGameTime.TotalSeconds);

        }








    }
}
