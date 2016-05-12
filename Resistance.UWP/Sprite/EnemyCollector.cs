using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Resistance.Scene;
using Microsoft.Xna.Framework;


namespace Resistance.Sprite
{
    public class EnemyCollector : AbstractEnemy
    {

        private static Microsoft.Xna.Framework.Graphics.Texture2D image;

        public override Microsoft.Xna.Framework.Graphics.Texture2D Image
        {
            get { return image; }
            set { image = value; }
        }

        public static readonly Animation FLY = new Animation(Point.Zero, 2, 2, 32, 32);

        public EnemyCollector(GameScene scene)
            : base(@"Animation\Enemy2", scene)
        {
            CurrentAnimation = FLY;
            origion = new Vector2(16, 32);
            collisonRec = new Rectangle(-16, -32, 32, 32);
        }


        public Human target;
        public override void Initilize()
        {
            base.Initilize();
            target = null;
            CurrentAnimation = FLY;


        }

        double frameTime;
        const double animationSpeed = 0.05f;

        protected override void AnimationChanged()
        {
            origion = new Vector2(CurrentAnimation.frameWidth / 2, CurrentAnimation.frameHeight);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 movement = new Vector2();

            if (Dead)
            {
                return;
            }

            if (target == null)
            {
                if (Game1.random.Next(5) == 0)
                    searchTarget();
            }
            else if (target.isCapturedBy == this)
            {
                movement += new Vector2(0, -1);

                if (position.Y < -50)
                {
                    target.Die();
                    target = null;
                }
            }
            else if (target.isCapturedBy != null)
            {
                if (Game1.random.Next(5) == 0)
                    searchTarget();
            }
            else
            {
                var sqrDistance = (position - target.position).LengthSquared();
                if (sqrDistance <= 16)
                {

                    target.isCapturedBy = this;
                }
                else
                {

                    var direction = (target.position - position);

                    direction.Normalize();
                    movement += direction;
                }
            }

            position += movement * scene.configuration.Collector.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;


            frameTime += gameTime.ElapsedGameTime.TotalSeconds;

            while (frameTime > animationSpeed)
            {
                ++currentAnimationFrame;
                if (currentAnimationFrame >= CurrentAnimation.Length)
                {
                    currentAnimationFrame = 0;
                }

                frameTime -= animationSpeed;
            }

        }

        private void searchTarget()
        {
            var v = scene.notKilledHumans;

            var newV = (from x in v where !x.IsCaptured orderby (position - x.position).LengthSquared() select x);
            target = newV.FirstOrDefault();

        }


        public override void Destroy()
        {
            if (target != null && target.isCapturedBy == this)
            {
                target.isCapturedBy = null;
            }
            scene.score += 75;
            base.Destroy();
        }

    }
}
