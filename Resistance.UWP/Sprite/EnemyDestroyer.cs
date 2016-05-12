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



        public static readonly Animation FLY = new Animation(Point.Zero, 3, 3, 32, 32);


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

        double frameTime;
        const double animationSpeed = 0.05f;

        public EnemyDestroyer(GameScene scene)
            : base(@"Animation\Enemy4", scene)
        {
            Dead = true;
            CurrentAnimation = FLY;
            collisonRec = new Rectangle(-15, -15, -32, 32);
            origion = new Vector2(16, 16);
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
                scene.allEnemyShots.Add(shots[i]);

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

            frameTime += gameTime.ElapsedGameTime.TotalSeconds;

            while (frameTime > animationSpeed)
            {
                ++currentAnimationFrame;
                if (currentAnimationFrame >= 8)
                {
                    currentAnimationFrame = 0;
                }

                frameTime -= animationSpeed;
            }
            Vector2 movment = new Vector2();


            if (position.Y < 0)
            {
                movment = new Vector2(0, 1);
            }
            else if (position.Y > scene.configuration.WorldHeight - CurrentAnimation.frameHeight - 5)
            {
                movment = new Vector2(0, -1);
            }
            Debug.Assert(!float.IsNaN(position.Y));
            if (aktuellerTic > MAX_TIME_TO_LIVE)
            {
                movment = new Vector2(0, -1);
                if (position.Y < -100 || float.IsNaN(position.Y))
                {
                    this.Destroy(true);
                }

                position += movment * scene.configuration.Destroyer.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                return;
            }
            //        if (getY() > 0 && (getX() - StaticFields.currentLevel.getPlayer().getX()) * (getX() - StaticFields.currentLevel.getPlayer().getX()) + (getY() - StaticFields.currentLevel.getPlayer().getY()) * (getY() - StaticFields.currentLevel.getPlayer().getY()) < 500 * 500) {
            //            fire();
            //        }



            Player player = scene.player;

            Vector2 playerMovement = player.movment;
            if (playerMovement != Vector2.Zero)
                playerMovement.Normalize();
            else
                playerMovement = new Vector2(0, -1);
            Vector2 targetPosition = player.position + playerMovement * 180;



            Vector2 movmentDirection = targetPosition - position;

            if (movmentDirection.Length() > scene.configuration.Destroyer.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds)
            {
                movmentDirection.Normalize();
                movment += movmentDirection;
                position += movment * scene.configuration.Destroyer.Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                position = targetPosition;
            }

            if ((position - targetPosition).LengthSquared() < 800)
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

            s.init(position, movment, 3.0);
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
            scene.score += 10;
            if (livePoints > 0)
            {
                return;
            }
            scene.score += 150;

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
                collisonRec = new Rectangle(-16, -16, 32, 32);
                CurrentAnimation = FLY;
                origion = new Vector2(16, 16);
                position = new Vector2(Game1.random.Next(scene.configuration.WorldWidth), Game1.random.Next(300) - 400);
            }

        }


    }
}
