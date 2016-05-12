using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework;

namespace Resistance.Scene
{
    class LoadingScene : IScene
    {
        private Queue<Action> actionList;
        private Action finishAction;



        public LoadingScene()
        {
        }



        public LoadingScene(Queue<Action> actionList, Action a)
        {
            this.actionList = actionList;
            this.finishAction = a;
        }

        public void Initilize()
        {
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            for (int i = 0; i < 20 && actionList.Count != 0; i++)
            {
                var action = actionList.Dequeue();
                action();
                if (actionList.Count == 0)
                {
                    finishAction();

                }
            }

        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Game1.instance.spriteBatch.Begin(transformMatrix: Game1.instance.ScaleMatrix);
            Game1.instance.spriteBatch.Draw(Game1.instance.clearStdBackground, Vector2.Zero, Color.White);
            Game1.instance.spriteBatch.DrawString(Game1.instance.font, actionList.Count.ToString(), Vector2.Zero, Color.White);
            Game1.instance.spriteBatch.End();
        }

        public void DoneLoading()
        {
        }
    }
}
