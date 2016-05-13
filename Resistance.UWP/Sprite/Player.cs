using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Resistance.Input;
using Resistance.Scene;
using Resistance.Components;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework.Audio;

namespace Resistance.Sprite
{

    public class Player : CollidelbleSprite
    {


        public static readonly Animation FLY_Left = new Animation(new Point(0, 3 * 24), 1, 1, 48, 24, 0.05f);
        public static readonly Animation FLY_RIGHT = new Animation(Point.Zero, 1, 1, 48, 24, 0.05f);
        public static readonly Animation TURN_LEFT = new Animation(Point.Zero, 6, 3, 48, 24, 0.05f, nextAnimation: FLY_Left);
        public static readonly Animation TURN_RIGHT = new Animation(new Point(0, 3 * 24), 6, 3, 48, 24, 0.05f, nextAnimation: FLY_RIGHT);
        SoundEffect shoot;

        System.Collections.Generic.Dictionary<int, bool> indicis = new Dictionary<int, bool>();
        public Shot[] allShots;
        public Bomb bomb;
        double frameTime;
        public Vector2 movment;


        private static Microsoft.Xna.Framework.Graphics.Texture2D image;
        public int lifePoints;
        public override Microsoft.Xna.Framework.Graphics.Texture2D Image
        {
            get { return image; }
            set { image = value; }
        }


        public Player(GameScene scene)
            : base(@"Animation\SmallShipTiles", scene, new Rectangle(-24, -12, 48, 24))
        {
            bomb = new Bomb(scene);
            Position = new Vector2(scene.configuration.WorldWidth / 2, scene.configuration.WorldHeight / 2);


            allShots = new Shot[scene.configuration.Player.ShotCount];

            for (int i = 0; i < allShots.Length; i++)
            {
                allShots[i] = new Shot(this);
            }

        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var shot in allShots)
            {
                shot.Draw(gameTime);
            }
            bomb.Draw(gameTime);
            base.Draw(gameTime);
        }

        public void Fire(float speed)
        {
            if (indicis.Count == 0)
                return;
            int i = indicis.First().Key;
            indicis.Remove(i);
            Shot s = allShots[i];
            s.Fire(speed, CurrentAnimation == FLY_Left ? Direction.Left : Direction.Right, Position);
            shoot.Play();
        }

        public void Hit()
        {
            --lifePoints;
            if (lifePoints <= 0)
                Scene.GameOver();
        }

        public override void Initilize()
        {
            base.Initilize();
            CurrentAnimation = FLY_RIGHT;
            bomb.Initilize();
            lifePoints = Scene.configuration.Player.Lifepoints;

            Game1.instance.QueuLoadContent(@"Sound\shot2", (SoundEffect s) => shoot = s);

            foreach (var shot in allShots)
            {
                shot.Initilize();
            }
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
            var input = Scene.input;
            bomb.Update(gameTime);
            frameTime += gameTime.ElapsedGameTime.TotalSeconds;


            movment = new Vector2();

            if (input.Down == BaseInput.Type.Hold)
                movment += new Vector2(0, 2);
            if (input.Up == BaseInput.Type.Hold)
                movment += new Vector2(0, -2);
            if (CurrentAnimation == FLY_RIGHT)
            {
                movment += new Vector2(1, 0);
                if (input.Right == BaseInput.Type.Hold)
                    movment += new Vector2(2, 0);
                else if (input.Left == BaseInput.Type.Press)
                    CurrentAnimation = TURN_LEFT;
            }
            else if (CurrentAnimation == FLY_Left)
            {
                movment += new Vector2(-1, 0);
                if (input.Left == BaseInput.Type.Hold)
                    movment += new Vector2(-2, 0);
                else if (input.Right == BaseInput.Type.Press)
                    CurrentAnimation = TURN_RIGHT;
            }
            movment *= Scene.configuration.Player.Speed;
            if (input.Fire == BaseInput.Type.Press && CurrentAnimation != TURN_LEFT && CurrentAnimation != TURN_RIGHT)
            {
                Fire(movment.X);
            }
            if (input.Bomb == BaseInput.Type.Press && !bomb.Visible && CurrentAnimation != TURN_LEFT && CurrentAnimation != TURN_RIGHT)
            {
                bomb.Boom();
            }
            Position += movment * (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < allShots.Length; i++)
            {
                allShots[i].Update(gameTime);
                if (!allShots[i].Visible)
                    indicis[i] = true;
            }

        }

        public class Bomb : Sprite<GameScene>
        {

            private Vector2 destructivRangeWithScaleOne;


            private Vector2 scalePerSeccond;

            public override Texture2D Image
            {
                get
                {
                    return tex;
                }
                set
                {
                    tex = value;
                }
            }

            static Texture2D tex;
            private Vector2 scaleWithMaxRadius;

            public Bomb(GameScene scene)
                : base(@"Animation\BombExplosion", scene)
            {
                CurrentAnimation = new Animation(Point.Zero, 1, 1, 475, 474, float.MaxValue);

                destructivRangeWithScaleOne = new Vector2(CurrentAnimation.FrameWidth / 2, CurrentAnimation.FrameHeight / 2);
                scaleWithMaxRadius = new Vector2(Scene.configuration.Player.MaxBombSizeWidth / destructivRangeWithScaleOne.X, Scene.configuration.Player.MaxBombSizeHeight / destructivRangeWithScaleOne.Y);
                scalePerSeccond = scaleWithMaxRadius / Scene.configuration.Player.TimeTillMaxBombSize;
            }



            public void Boom()
            {
                Position = Scene.player.Position;
                Visible = true;
                Scale = Vector2.Zero;
            }

            public override void Initilize()
            {
                base.Initilize();
                Visible = false;
            }

            public bool PointWithinExplosion(Vector2 pointposition)
            {
                Vector2 realativPosition = pointposition - Position;

                Vector2 radians = destructivRangeWithScaleOne * Scale;

                if (radians.X != radians.Y)
                    realativPosition *= new Vector2(1, radians.X / radians.Y);
                return realativPosition.LengthSquared() <= radians.X * radians.X;
            }

            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);

                if (!Visible)
                    return;
                if (Scale.X >= scaleWithMaxRadius.X && Scale.X >= scaleWithMaxRadius.X)
                {
                    Visible = false;
                    return;
                }

                float transparentPercent = 1f - Scale.X / scaleWithMaxRadius.X;

                Color = new Color(transparentPercent, transparentPercent, transparentPercent, transparentPercent);

                foreach (var enemy in Scene.notDestroyedEnemys)
                {
                    if (PointWithinExplosion(enemy.Position))
                        enemy.Destroy();
                }


                Scale += scalePerSeccond * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public class Shot : CollidelbleSprite
        {

            public const double SHOT_LIFETIME = 2;
            public const float SPEED = 720;
            private static readonly Animation DIE_LEFT;
            private static readonly Animation DIE_RIGHT;
            private static readonly Animation FLY_LEFT;
            private static readonly Animation FLY_RIGHT;
            private static readonly Animation CREATE_LEFT;
            private static readonly Animation CREATE_RIGHT;
            public Direction direction;

            static Shot()
            {
                Shot.DIE_LEFT = new Animation(new Point(0, 8), 4, 2, 160, 8, 0.05f, () => new Vector2(2, 4), loop: false);
                Shot.DIE_RIGHT = new Animation(new Point(0, 8), 4, 2, 160, 8, 0.05f, () => new Vector2(158, 4), loop: false);
                Shot.FLY_LEFT = new Animation(new Point(0, 8), 1, 1, 160, 8, 0.05f, () => new Vector2(2, 4));
                Shot.FLY_RIGHT = new Animation(new Point(0, 8), 1, 1, 160, 8, 0.05f, () => new Vector2(158, 4));
                Shot.CREATE_LEFT = new Animation(Point.Zero, 4, 2, 5, 160, 8, 0.05f, () => new Vector2(2, 4), Shot.FLY_LEFT);
                Shot.CREATE_RIGHT = new Animation(Point.Zero, 4, 2, 5, 160, 8, 0.05f, () => new Vector2(158, 4), Shot.FLY_RIGHT);
            }

            private static Microsoft.Xna.Framework.Graphics.Texture2D image;
            public override Microsoft.Xna.Framework.Graphics.Texture2D Image
            {
                get { return image; }
                set { image = value; }
            }

            private double lifetime;
            Player player;
            public float speed;

            public Shot(Player player)
                : base(@"Animation\FireBlastTiles", player.Scene, new Rectangle(-4, -2, 8, 4))
            {
                this.player = player;

            }



            private void Die()
            {

                switch (direction)
                {
                    case Direction.Left:
                        CurrentAnimation = DIE_LEFT;
                        break;
                    case Direction.Right:
                        CurrentAnimation = DIE_RIGHT;
                        break;
                }
                CurrentAnimationFrame = 0;
            }

            public override void Draw(GameTime gameTime)
            {
                if (Visible)
                    base.Draw(gameTime);
            }

            public void Fire(float playerSpeed, Direction playerDirection, Vector2 position)
            {
                this.direction = playerDirection;
                switch (direction)
                {
                    case Direction.Left:
                        speed = playerSpeed - SPEED;
                        position.X -= 2f;
                        CurrentAnimation = CREATE_LEFT;
                        SpriteEfekt = SpriteEffects.FlipHorizontally;
                        break;
                    case Direction.Right:
                        speed = playerSpeed + SPEED;
                        position.X += 2f;
                        CurrentAnimation = CREATE_RIGHT;
                        SpriteEfekt = SpriteEffects.None;
                        break;
                }
                this.Position = position;
                Visible = true;
                CurrentAnimationFrame = 0;
                lifetime = 0;
            }

            public override void Initilize()
            {
                base.Initilize();
                Visible = false;
            }

            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);
                if (!Visible)
                    return;

                Vector2 tmp = player.Position;

                Position += new Vector2((float)(speed * gameTime.ElapsedGameTime.TotalSeconds), 0);

                lifetime += gameTime.ElapsedGameTime.TotalSeconds;

                if (lifetime > SHOT_LIFETIME && !(CurrentAnimation == DIE_LEFT || CurrentAnimation == DIE_RIGHT || CurrentAnimation == null))
                {
                    Die();
                }






                if (CurrentAnimation != DIE_LEFT && CurrentAnimation != DIE_RIGHT && CurrentAnimation != null)
                {

                    var enemys = player.Scene.notDestroyedEnemys.Union(new AbstractEnemy[] { Scene.destroyer });
                    foreach (var e in enemys)
                    {
                        if (ColideWith(e) && !e.Dead
                            && ((direction == Direction.Right && e.Position.X >= tmp.X)
                                || (direction == Direction.Left && e.Position.X <= tmp.X)))
                        {
                            e.Destroy();
                            Die();
                            break;
                        }
                    }
                    var humans = player.Scene.notKilledHumans;
                    foreach (var h in humans)
                    {
                        if (ColideWith(h)
                            && ((direction == Direction.Right && h.Position.X >= tmp.X)
                                || (direction == Direction.Left && h.Position.X <= tmp.X)))
                        {
                            h.Die();
                            Die();
                            break;
                        }
                    }
                }
            }
        }
    }
}
