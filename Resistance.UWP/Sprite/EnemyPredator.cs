using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Resistance.Scene;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Audio;


namespace Resistance.Sprite
{
    public class EnemyPredator : AbstractEnemy
    {

        private static Microsoft.Xna.Framework.Graphics.Texture2D image;

        public override Microsoft.Xna.Framework.Graphics.Texture2D Image
        {
            get { return image; }
            set { image = value; }
        }

        SoundEffect zap;

        private int direction;
        private const int HOCH = 0;
        private const int RUNTER = 1;
        private const int LINKS = 2;
        private const int RECHTS = 3;
        private const int LINKSOBEN = 4;
        private const int RECHTSOBEN = 5;
        private const int LINKSUNTEN = 6;
        private const int RECHTSUNTEN = 7;


        public Shot[] shots;
        System.Collections.Generic.Dictionary<int, bool> indicis = new Dictionary<int, bool>();






        const float SPEED = 16;



        private static readonly Animation FLY = new Animation(Point.Zero, 3, 3, 7,32, 32, 0.05, () => new Vector2(16, 16));

        public EnemyPredator(GameScene scene)
            : base(@"Animation\Enemy1", scene, new Rectangle(-16, -16, 32, 32))
        {
            CurrentAnimation = FLY;
            shots = new Shot[scene.configuration.Predator.NumberOfShots];
            for (int i = 0; i < scene.configuration.Predator.NumberOfShots; i++)
            {
                shots[i] = new Shot(scene);
                scene.allEnemyShots.Add(shots[i]);
            }
        }


        public override void Initilize()
        {
            base.Initilize();

            Game1.instance.QueuLoadContent(@"Sound\shot", (SoundEffect t) => zap = t);

            CurrentAnimation = FLY;



            for (int i = 0; i < Scene.configuration.Predator.NumberOfShots; i++)
            {
                shots[i].Initilize();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);


        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            for (int i = 0; i < shots.Length; i++)
            {
                if (!shots[i].Visible)
                    indicis[i] = true;
            }

            if (Dead)
            {
                return;
            }



            int newDirection = Game1.random.Next(512);
            if (newDirection < 8)
            {
                direction = newDirection;
            }
            if (Position.Y < 0)
            {
                direction = RUNTER;
            }
            else if (Position.Y > Scene.configuration.WorldHeight - CurrentAnimation.FrameHeight - 5)
            {
                direction = HOCH;
            }
            Vector2 movment = new Vector2();
            switch (direction)
            {
                case 0:
                    movment += new Vector2(0, -2);
                    break;
                case 1:
                    movment += new Vector2(0, 2);
                    break;
                case 2:
                    movment += new Vector2(-3, 0);
                    break;
                case 3:
                    movment += new Vector2(3, 0);
                    break;
                case 4:
                    movment += new Vector2(-2, -1);
                    break;
                case 5:
                    movment += new Vector2(2, -1);
                    break;
                case 6:
                    movment += new Vector2(-2, 1);
                    break;
                case 7:
                    movment += new Vector2(2, 1);
                    break;

            }

            Position += movment * (float)gameTime.ElapsedGameTime.TotalSeconds * SPEED;

            if (Game1.random.Next((3 - (int)Scene.difficulty) << 5) < 1)
            {
                fire();
            }
        }

        private void fire()
        {
            if (indicis.Count == 0)
                return;



            Vector2 target;

            Player player = Scene.player;

            if (Scene.configuration.EnemyTargetting)
            {
                for (float i = 0; i < 6f; i += 0.3f)
                {


                    var mov = player.movment;

                    var newPlayerPosition = player.Position + (i * mov);

                    target = newPlayerPosition - Position;
                    if (target.LengthSquared() <= Scene.configuration.EnemyShotSpeed * i * Scene.configuration.EnemyShotSpeed * i)
                        goto targetin;

                }
                return;

            }
            else
                target = player.Position - Position;
            if (target.LengthSquared() > 400 * 400)
            {
                return;


            }
            targetin:

            float distance = target.Length();




            if (distance < 48)
            {
                return;
            }

            int index = indicis.First().Key;
            indicis.Remove(index);
            Shot s = shots[index];

            target.Normalize();
            target *= Scene.configuration.EnemyShotSpeed;
            s.init(Position, target, distance + 150);
            zap.Play();
            //TODO: Shoot
            //((Shot) shots.pop()).init(getRefPixelX(), getRefPixelY(), dx, dy, distance + 150);
            //StaticFields.getSound().playSFX("shot", 20);

        }

        public class Shot : CollidelbleSprite
        {

            private static Microsoft.Xna.Framework.Graphics.Texture2D image;

            public override Microsoft.Xna.Framework.Graphics.Texture2D Image
            {
                get { return image; }
                set { image = value; }
            }

            public static readonly Animation FLY = new Animation(Point.Zero, 2, 2, 8, 8, 0.05f);


            double livetime = 0;
            /**
             * Maximale Lebenszeit
             */
            double timeToLive;
            Vector2 movment;

            public override void Initilize()
            {
                base.Initilize();
                Visible = false;
            }

            public Shot(GameScene scene)
                : base(@"Animation\newAlienFireBlast", scene, new Rectangle(-4, -4, 8, 8))
            {
                CurrentAnimation = FLY;
            }

            public void init(Vector2 position, Vector2 movement, double lifetime)
            {
                timeToLive = lifetime;
                this.livetime = 0;

                this.Position = position;
                movement.Normalize();
                movement *= Scene.configuration.EnemyShotSpeed;
                this.movment = movement;

                Visible = true;
            }

            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);

                if (!Visible)
                    return;
                if ((Position.Y < 0 && movment.Y <= 0) || (Position.Y > Scene.configuration.WorldHeight && movment.Y >= 0))
                {
                    Die();
                }
                livetime += gameTime.ElapsedGameTime.TotalSeconds;
                if (livetime > timeToLive)
                {
                    Die();
                }
                else if (ColideWith(Scene.player))
                {
                    Scene.player.Hit();
                    Die();
                }
                else
                {
                    Position += movment * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            private void Die()
            {
                Visible = false;

            }
        }

        public override void Destroy()
        {
            Scene.score += 75;
            base.Destroy();
        }



    }
}
