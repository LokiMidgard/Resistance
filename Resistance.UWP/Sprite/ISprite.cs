using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Resistance.Scene;

namespace Resistance.Sprite
{
    public interface ISprite<out T> where T : IScene
    {
        Color Color { get; set; }
        Animation CurrentAnimation { get; set; }
        int CurrentAnimationFrame { get; set; }
        Texture2D Image { get; set; }
        Vector2 Origin { get; }
        Vector2 Position { get; set; }
        Vector2 Scale { get; set; }
        T Scene { get; }
        SpriteEffects SpriteEfekt { get; set; }
        bool Visible { get; set; }

        void Draw(GameTime gameTime);
        void Initilize();
        void Update(GameTime gameTime);
    }
}