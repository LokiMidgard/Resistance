using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resistance.Sprite
{
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

        public Animation(Point leftTop, int width, int heigth, int frameCount, int frameWidth, int frameHeighr, double animationSpeed, Func<Animation, Vector2> calculateOrigin = null, Animation nextAnimation = null, bool loop = true) : this(leftTop, width, heigth, frameWidth, frameHeighr, animationSpeed, calculateOrigin, nextAnimation, loop)
        { this.frameCount = frameCount; }

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
