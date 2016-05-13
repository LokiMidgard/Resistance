using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Resistance.Scene;
using Microsoft.Xna.Framework;


namespace Resistance.Sprite
{
    class EnemyMine : AbstractEnemy
    {

        private static Microsoft.Xna.Framework.Graphics.Texture2D image;

        public override Microsoft.Xna.Framework.Graphics.Texture2D Image
        {
            get { return image; }
            set { image = value; }
        }

        public static readonly Animation FLY = new Animation(Point.Zero, 3, 3,8, 32, 32, 0.05f, () => new Vector2(16, 16));




        private int direction;
        private const int DIRECTION_UP = 0;
        private const int DIRECTION_DOWN = 1;
        private const int DIRECTION_LEFT = 2;
        private const int DIRECTION_RIGHT = 3;
        private const int DIRECTION_UP_LEFT = 4;
        private const int DIRECTION_UP_RIGHT = 5;
        private const int DIRECTION_DOWN_LEFT = 6;
        private const int DIRECTION_DOWN_RIGHT = 7;

        public EnemyMine(GameScene scene)
            : base(@"Animation\Enemy3", scene, new Rectangle(-16, -16, 32, 32))
        {
        }

        public override void Initilize()
        {
            base.Initilize();
            CurrentAnimation = FLY;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            if (Dead)
                return;



            Player player = Scene.player;

            Vector2 movment = new Vector2();

            if ((player.Position - Position).LengthSquared() < 350 * 350)
            {
                // Spieler rammen
                Vector2 target = player.Position - Position;
                target.Normalize();
                movment += target;

            }
            else
            {
                // Zufaellige bewegung
                int newDirection = Game1.random.Next(512);
                if (newDirection < 8)
                {
                    direction = newDirection;
                }
                if (Position.Y < 0)
                {
                    direction = DIRECTION_DOWN;
                }
                else if (Position.Y > Scene.configuration.WorldHeight - 32 - 5)
                {
                    direction = DIRECTION_UP;
                }
                switch (direction)
                {
                    case DIRECTION_UP:
                        movment += new Vector2(0, -4);
                        break;
                    case DIRECTION_DOWN:
                        movment += new Vector2(0, 4);
                        break;
                    case DIRECTION_LEFT:
                        movment += new Vector2(-4, 0);
                        break;
                    case DIRECTION_RIGHT:
                        movment += new Vector2(4, 0);
                        break;
                    case DIRECTION_UP_LEFT:
                        movment += new Vector2(-3, -1);
                        break;
                    case DIRECTION_UP_RIGHT:
                        movment += new Vector2(3, -1);
                        break;
                    case DIRECTION_DOWN_LEFT:
                        movment += new Vector2(-3, 1);
                        break;
                    case DIRECTION_DOWN_RIGHT:
                        movment += new Vector2(3, 1);
                        break;
                }
                movment.Normalize();
            }

            Position += movment * Scene.configuration.Mine.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (ColideWith(player))
            {
                player.Hit();
                base.Destroy(); // sterben ohne punkte
            }


        }


        public override void Destroy()
        {
            Scene.score += 75;
            base.Destroy();
        }


    }
}
