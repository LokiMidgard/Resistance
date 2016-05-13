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

        public static readonly Animation FLY = new Animation(Point.Zero, 2, 2, 32, 32, 0.05f, () => new Vector2(16, 32));

        public EnemyCollector(GameScene scene)
            : base(@"Animation\Enemy2", scene, new Rectangle(-16, -32, 32, 32))
        {
            CurrentAnimation = FLY;
        }


        public Human target;
        public override void Initilize()
        {
            base.Initilize();
            target = null;
            CurrentAnimation = FLY;
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

                if (Position.Y < -50)
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
                var sqrDistance = (Position - target.Position).LengthSquared();
                if (sqrDistance <= 16)
                {

                    target.isCapturedBy = this;
                }
                else
                {

                    var direction = (target.Position - Position);

                    direction.Normalize();
                    movement += direction;
                }
            }

            Position += movement * Scene.configuration.Collector.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;




        }

        private void searchTarget()
        {
            var v = Scene.notKilledHumans;

            var newV = (from x in v where !x.IsCaptured orderby (Position - x.Position).LengthSquared() select x);
            target = newV.FirstOrDefault();

        }


        public override void Destroy()
        {
            if (target != null && target.isCapturedBy == this)
            {
                target.isCapturedBy = null;
            }
            Scene.score += 75;
            base.Destroy();
        }

    }
}
