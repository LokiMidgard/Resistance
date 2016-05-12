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
    public class Human : Sprite
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

        double frameTime;
        const double animationSpeed = 0.2f;

        public static readonly Animation WALK = new Animation(Point.Zero, 2, 2, 24, 24);
        public static readonly Animation STAND = new Animation(Point.Zero, 1, 1, 24, 24);


        public Human(GameScene scene)
            : base(@"Animation\NewManTiles", scene)
        {
            origion = new Vector2(24, 0);
            collisonRec = new Rectangle(-12, 0, 24, 24);
            position = new Vector2(Game1.random.Next(scene.configuration.WorldWidth), scene.configuration.WorldHeight - 24);
        }

        protected override void AnimationChanged()
        {
            base.AnimationChanged();
            origion = new Vector2(CurrentAnimation.frameWidth / 2, 0);
        }

        public override void Initilize()
        {
            if (!Visible)
                position = new Vector2(Game1.random.Next(scene.configuration.WorldWidth), scene.configuration.WorldHeight - 24);

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

            scene.notKilledHumans.Remove(this);
            if (IsCaptured)
            {
                isCapturedBy.target = null;
                isCapturedBy = null;
            }
            scene.score -= 25;
            screem.Play();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Vector2 movment = new Vector2();
            if (IsCaptured)
            {
                CurrentAnimation = STAND;
                direction = Direction.None;
                currentAnimationFrame = 0;
                position = new Vector2(isCapturedBy.position.X, Math.Min(scene.configuration.WorldWidth - 24, isCapturedBy.position.Y));
            }
            else if (position.Y < scene.configuration.WorldHeight - 24)
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
                            currentAnimationFrame = 0;
                            break;
                        case Direction.Right:
                            CurrentAnimation = WALK;
                            currentAnimationFrame = 1;
                            spriteEfekt = Microsoft.Xna.Framework.Graphics.SpriteEffects.None;
                            break;
                        case Direction.Left:
                            CurrentAnimation = WALK;
                            spriteEfekt = Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally;
                            currentAnimationFrame = 1;
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


                frameTime += gameTime.ElapsedGameTime.TotalSeconds;

                while (frameTime > animationSpeed)
                {
                    ++currentAnimationFrame;
                    if (currentAnimationFrame >= CurrentAnimation.Length)
                    {
                        currentAnimationFrame = 1;
                    }

                    frameTime -= animationSpeed;
                }



            }
            position += (movment * 16 * (float)gameTime.ElapsedGameTime.TotalSeconds);

        }








    }
}
