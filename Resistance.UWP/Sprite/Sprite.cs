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

    public abstract class CollidelbleSprite : Sprite<GameScene>
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

    public class Sprite<T> : IDrawableComponent, ISprite<T> where T : IScene
    {


        private Animation currentAnimation;
        private double frameTime;
        private String imageName;
        public Sprite(String imageName, T scene)
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
        public virtual Texture2D Image { get; set; }


        public Vector2 Origin { get; private set; }
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; } = Vector2.One;
        public T Scene { get; }
        public SpriteEffects SpriteEfekt { get; set; } = SpriteEffects.None;

        public bool Visible { get; set; }

        public virtual void Draw(GameTime gameTime)
        {
            if (CurrentAnimation == null)
                Visible = false;
            if (Visible)
            {
                if (Scene is GameScene)
                    Game1.instance.spriteBatch.Draw(Image, Position - (Scene as GameScene).ViewPort, CurrentAnimation[CurrentAnimationFrame], Color, 0f, Origin, Scale, SpriteEfekt, 0f);
                else
                    Game1.instance.spriteBatch.Draw(Image, Position, CurrentAnimation[CurrentAnimationFrame], Color, 0f, Origin, Scale, SpriteEfekt, 0f);
            }
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




       

    }
}
