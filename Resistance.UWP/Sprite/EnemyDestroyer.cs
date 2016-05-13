using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Resistance.Scene;
using Microsoft.Xna.Framework;

using System.Diagnostics;

namespace Resistance.Sprite
{
    public class EnemyDestroyer : AbstractEnemy
    {

        private static Microsoft.Xna.Framework.Graphics.Texture2D image;

        public override Microsoft.Xna.Framework.Graphics.Texture2D Image
        {
            get { return image; }
            set { image = value; }
        }



        public static readonly Animation FLY = new Animation(Point.Zero, 3, 3,8, 32, 32, 0.05f, () => new Vector2(16, 16));


        /**
         * Gibt an welcher schuss zuletzt geschossen wurde um den Winkel des nächsten schusse festzulegen.
         */
        private int lastShotNumber;
        /**
         * Vorberechnete werte für eine Kreisartig schuss, so muss net immer cos in sin berechnet werden
         */
        private static readonly int[] xShootArray = new int[]{256,
        252,
        241,
        222,
        196,
        165,
        128,
        88,
        44,
        0,
        -44,
        -88,
        -128,
        -165,
        -196,
        -222,
        -241,
        -252,
        -256,
        -252,
        -241,
        -222,
        -196,
        -165,
        -128,
        -88,
        -44,
        0,
        44,
        88,
        128,
        165,
        196,
        222,
        241,
        252,
        256};
        /**
         * Vorberechnete werte für eine Kreisartig schuss, so muss net immer cos in sin berechnet werden
         */
        private static readonly int[] yShootArray = new int[]{0,
        44,
        88,
        128,
        165,
        196,
        222,
        241,
        252,
        256,
        252,
        241,
        222,
        196,
        165,
        128,
        88,
        44,
        0,
        -44,
        -88,
        -128,
        -165,
        -196,
        -222,
        -241,
        -252,
        -256,
        -252,
        -241,
        -222,
        -196,
        -165,
        -128,
        -88,
        -44,
        0};
        /**
         * Zählt wie lange der Destroyer im Spiel ist um ihn nach kurzer Zeit zurück zu ziehen
         */
        private double aktuellerTic;
        private const double MAX_TIME_TO_LIVE = 20.0;
        private const int MAX_LIVEPOINTS = 4;
        private int livePoints = 4;
        private const int HOCH = 0;
        private const int RUNTER = 1;
        private const int LINKS = 2;
        private const int RECHTS = 3;
        private const int LINKSOBEN = 4;
        private const int RECHTSOBEN = 5;
        private const int LINKSUNTEN = 6;
        private const int RECHTSUNTEN = 7;
        private EnemyPredator.Shot[] shots;
        System.Collections.Generic.Dictionary<int, bool> indicis = new Dictionary<int, bool>();

        private const int MAX_NUMBER_OF_SHOTS = 36;



        public EnemyDestroyer(GameScene scene)
            : base(@"Animation\Enemy4", scene, new Rectangle(-15, -15, -32, 32))
        {
            Dead = true;
            CurrentAnimation = FLY;

            shots = new EnemyPredator.Shot[MAX_NUMBER_OF_SHOTS];
            for (int i = 0; i < MAX_NUMBER_OF_SHOTS; i++)
            {
                shots[i] = new EnemyPredator.Shot(scene);
            }
        }


        public override void Initilize()
        {
            base.Initilize();
            Visible = false;
            Dead = true;
            for (int i = 0; i < MAX_NUMBER_OF_SHOTS; i++)
            {
                shots[i].Initilize();
                Scene.allEnemyShots.Add(shots[i]);

            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            aktuellerTic += gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < shots.Length; i++)
            {
                if (!shots[i].Visible)
                    indicis[i] = true;
            }

            if (Dead)
            {
                return;
            }


            Vector2 movment = new Vector2();


            if (Position.Y < 0)
            {
                movment = new Vector2(0, 1);
            }
            else if (Position.Y > Scene.configuration.WorldHeight - CurrentAnimation.FrameHeight - 5)
            {
                movment = new Vector2(0, -1);
            }
            Debug.Assert(!float.IsNaN(Position.Y));
            if (aktuellerTic > MAX_TIME_TO_LIVE)
            {
                movment = new Vector2(0, -1);
                if (Position.Y < -100 || float.IsNaN(Position.Y))
                {
                    this.Destroy(true);
                }

                Position += movment * Scene.configuration.Destroyer.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                return;
            }
            //        if (getY() > 0 && (getX() - StaticFields.currentLevel.getPlayer().getX()) * (getX() - StaticFields.currentLevel.getPlayer().getX()) + (getY() - StaticFields.currentLevel.getPlayer().getY()) * (getY() - StaticFields.currentLevel.getPlayer().getY()) < 500 * 500) {
            //            fire();
            //        }



            Player player = Scene.player;

            Vector2 playerMovement = player.movment;
            if (playerMovement != Vector2.Zero)
                playerMovement.Normalize();
            else
                playerMovement = new Vector2(0, -1);
            Vector2 targetPosition = player.Position + playerMovement * 180;



            Vector2 movmentDirection = targetPosition - Position;

            if (movmentDirection.Length() > Scene.configuration.Destroyer.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds)
            {
                movmentDirection.Normalize();
                movment += movmentDirection;
                Position += movment * Scene.configuration.Destroyer.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                Position = targetPosition;
            }

            if ((Position - targetPosition).LengthSquared() < 800)
            {
                fire();
            }

        }

        private void fire()
        {
            lastShotNumber++;
            lastShotNumber %= xShootArray.Length;
            if (indicis.Count == 0)
                return;

            int dx = xShootArray[lastShotNumber];
            int dy = yShootArray[lastShotNumber];

            Vector2 movment = new Vector2(dx, dy);

            movment.Normalize();

            int index = indicis.First().Key;
            indicis.Remove(index);
            EnemyPredator.Shot s = shots[index];

            s.init(Position, movment, 3.0);
            lastShotNumber++;

            //TODO Make sound;
            //StaticFields.getSound().playSFX("shot", 20);
            //     direction = 8;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

        }

        public override void Destroy()
        {
            livePoints--;
            Scene.score += 10;
            if (livePoints > 0)
            {
                return;
            }
            Scene.score += 150;

            base.Destroy();
        }

        private void Destroy(bool b)
        {
            base.Destroy();
        }


        public void ReEnter()
        {

            if (!Visible)
            {
                Dead = false;
                Visible = true;
                livePoints = EnemyDestroyer.MAX_LIVEPOINTS;
                aktuellerTic = 0;
                CurrentAnimation = FLY;

                Position = new Vector2(Game1.random.Next(Scene.configuration.WorldWidth), Game1.random.Next(300) - 400);
            }

        }


    }
}
