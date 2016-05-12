using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework;

namespace Resistance.Scene
{
    class HavyLoadingScene : IScene
    {

        List<Texture2D> texList = new List<Texture2D>();

        public void Initilize()
        {
            String[] a = new String[] { "city1", "city3", "city2", "cloud1", "cloud2", "cloud3", "cloud4", "cloud5", "gradient", "hills1", "hills2", "hills3", "hills4", "mountains2", "mountains1", "stars" };

            foreach (var s in a)
            {
                Game1.instance.QueuLoadContent(s, (Texture2D t) => { texList.Add(t); });
            }
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Game1.instance.spriteBatch.Begin(transformMatrix: Game1.instance.ScaleMatrix);

            foreach (var t in texList)
            {
                Game1.instance.spriteBatch.Draw(t, Vector2.Zero, Color.White);
            }
            Game1.instance.spriteBatch.End();
        }

        public void DoneLoading()
        {
        }
    }
}
