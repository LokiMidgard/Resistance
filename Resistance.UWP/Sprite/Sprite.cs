using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Resistance.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Resistance.Scene;
using Resistance.UWP;

namespace Resistance.Sprite
{

    public enum Direction
    {
        None,
        Left,
        Right

    }

    public abstract class CollidelbleSprite : Sprite
    {
        private Rectangle collisionRec;

        public CollidelbleSprite(string imageName, GameScene scene, Rectangle collisionRec) : base(imageName, scene)
        {
            this.collisionRec = collisionRec;
        }

        public bool ColideWith(CollidelbleSprite other)
        {
            var t = collisionRec;
            t.Offset((int)Position.X, (int)Position.Y);
            var o = other.collisionRec;
            o.Offset((int)other.Position.X, (int)other.Position.Y);
            return t.Intersects(o);
        }
    }

    public abstract class Sprite : IDrawableComponent
    {


        private Animation currentAnimation;
        private double frameTime;
        private String imageName;
        public Sprite(String imageName, GameScene scene)
        {
            this.imageName = imageName;
            this.Scene = scene;
        }

        public Color Color { get; set; } = Color.White;
        public Animation CurrentAnimation
        {
            get { return currentAnimation; }
            set
            {
                currentAnimation = value;
                if (currentAnimation != null)
                    this.Origin = currentAnimation.CalculateOriginForAnimation();
                else
                    this.Origin = Vector2.Zero;
            }
        }

        public int CurrentAnimationFrame { get; set; }
        public abstract Texture2D Image { get; set; }


        public Vector2 Origin { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public GameScene Scene { get; }
        public SpriteEffects SpriteEfekt { get; set; } = SpriteEffects.None;

        public bool Visible { get; set; }

        public virtual void Draw(GameTime gameTime)
        {
            if (CurrentAnimation == null)
                Visible = false;
            if (Visible)
                Game1.instance.spriteBatch.Draw(Image, Position - Scene.ViewPort, CurrentAnimation[CurrentAnimationFrame], Color, 0f, Origin, Scale, SpriteEfekt, 0f);
        }

        public virtual void Initilize()
        {
            Visible = true;
            if (Image == null)
                Game1.instance.QueuLoadContent(imageName, (Texture2D t) => Image = t);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (CurrentAnimation != null && Visible)
            {
                frameTime += gameTime.ElapsedGameTime.TotalSeconds;
                while (frameTime > CurrentAnimation.AnimationSpeed)
                {
                    var epleapsedFrames = (int)Math.Floor(frameTime / CurrentAnimation.AnimationSpeed);
                    CurrentAnimationFrame += epleapsedFrames;
                    if (CurrentAnimationFrame >= CurrentAnimation.Length)
                    {
                        var lastAnimationFrames = epleapsedFrames - (CurrentAnimationFrame - CurrentAnimation.Length);
                        frameTime -= lastAnimationFrames * CurrentAnimation.AnimationSpeed;

                        epleapsedFrames -= lastAnimationFrames;
                        CurrentAnimation = CurrentAnimation.NextAnimation;
                        if (CurrentAnimation == null)
                            break;
                        CurrentAnimationFrame = 0;
                    }
                    else
                        frameTime -= epleapsedFrames * CurrentAnimation.AnimationSpeed;
                }
            }

        }




        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CS0660", Justification = "Provided by Fody")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CS0661", Justification = "Provided by Fody")]
        public class Animation
        {
            private Func<Animation, Vector2> calculateOrigin;
            private int? frameCount;

            public Animation(Point leftTop, int width, int heigth, int frameWidth, int frameHeighr, double animationSpeed, Func<Animation, Vector2> calculateOrigin = null, Animation nextAnimation = null, bool loop = true)
            {
                this.LeftTop = leftTop;
                this.Width = width;
                this.Height = heigth;
                this.FrameWidth = frameWidth;
                this.FrameHeight = frameHeighr;
                this.AnimationSpeed = animationSpeed;
                this.NextAnimation = nextAnimation ?? (loop ? this : null);
                this.calculateOrigin = calculateOrigin ?? new Func<Animation, Vector2>(animation => new Vector2(animation.FrameWidth / 2, animation.FrameHeight / 2));

            }

            public Animation(Point leftTop, int width, int heigth, int frameWidth, int frameHeighr, double animationSpeed, Func<Vector2> calculateOrigin, Animation nextAnimation = null, bool loop = true) : this(leftTop, width, heigth, frameWidth, frameHeighr, animationSpeed, animation => calculateOrigin(), nextAnimation, loop)
            { }

            public Animation(Point leftTop, int width, int heigth, int frameCount, int frameWidth, int frameHeighr, double animationSpeed, Func<Vector2> calculateOrigin, Animation nextAnimation = null, bool loop = true) : this(leftTop, width, heigth, frameWidth, frameHeighr, animationSpeed, animation => calculateOrigin(), nextAnimation, loop)
            { this.frameCount = frameCount; }


            public Animation NextAnimation { get; }

            public double AnimationSpeed { get; }
            public int FrameHeight { get; }
            public int FrameWidth { get; }
            public int Height { get; }
            public Point LeftTop { get; }
            public int Length => frameCount ?? Width * Height;
            public int Width { get; }


            public Rectangle this[int index] =>
                new Rectangle(
                    LeftTop.X + (index % Width) * FrameWidth,
                    LeftTop.Y + (index / Width) * FrameHeight,
                    FrameWidth,
                    FrameHeight);


            public Vector2 CalculateOriginForAnimation() => calculateOrigin(this);



        }



    }
}
