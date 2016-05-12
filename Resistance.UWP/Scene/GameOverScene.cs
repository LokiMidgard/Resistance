using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework;

namespace Resistance.Scene
{
    class GameOverScene : IScene
    {



        

        Texture2D texture;
        private int score;

        public GameOverScene(int score)
        {
            // TODO: Complete member initialization
            this.score = score;
        }
        public void Initilize()
        {
            Game1.instance.QueuLoadContent(@"Menue\ResTitelWP7_ohneGuy", (Texture2D t) => texture = t);
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {

            Game1.instance.spriteBatch.Begin(transformMatrix: Game1.instance.ScaleMatrix);
            Game1.instance.spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            Game1.instance.spriteBatch.DrawString(Game1.instance.font, score.ToString(),new Vector2(240,400), Color.White);
            
            Game1.instance.spriteBatch.End();
        }

        public void DoneLoading()
        {
        }
    }
}
