using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Resistance.Components;
using Microsoft.Xna.Framework.Graphics;

using Resistance.Scene;
using Microsoft.Xna.Framework;

namespace Resistance.Sprite
{
    class Hud : IDrawableComponent
    {
        Texture2D point;
        Texture2D bombIcon;
        Texture2D bombEmptyIcon;

        int rardarHeight = 96;
        int radarWidth = 800;

        private int live;
        private int maxLive;

        private String score = "";

        GameScene scene;

        Dictionary<Vector2, Color> radarDots = new Dictionary<Vector2, Color>();

        public Hud(GameScene scene)
        {
            this.scene = scene;
        }


        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {

            maxLive = 300;
            live = scene.player.lifePoints * 300 / scene.configuration.Player.Lifepoints;
            score = scene.score.ToString();
            Vector2 scalirungsvector = new Vector2((float)radarWidth / (float)scene.configuration.WorldWidth, (float)rardarHeight / (float)scene.configuration.WorldHeight);
            radarDots.Clear();

            Vector2 playerVector = scene.player.Position * scalirungsvector;

            Vector2 mittelVector = new Vector2(this.radarWidth / 2, rardarHeight / 2);

            Vector2 deltaVector = mittelVector - playerVector;

            deltaVector *= new Vector2(1, 0);

            foreach (var item in scene.notDestroyedEnemys)
            {

                Vector2 v = item.Position * scalirungsvector;
                radarDots[v + deltaVector] = Color.Violet;
            }

            foreach (var shot in from s in scene.allEnemyShots where s.Visible select s)
            {
                radarDots[shot.Position * scalirungsvector + deltaVector] = Color.WhiteSmoke;
            }

            foreach (var item in scene.notKilledHumans)
            {

                Vector2 v = item.Position * scalirungsvector;
                radarDots[v + deltaVector] = item.IsCaptured ? Color.Red : Color.Green;

            }

            Vector2 destroyserVector = scene.destroyer.Position * scalirungsvector;
            radarDots[destroyserVector + deltaVector] = Color.Magenta;


            radarDots[playerVector + deltaVector] = Color.Yellow;




        }

        public void Initilize()
        {
            Game1.instance.QueuLoadContent("Point", (Texture2D t) => point = t);
            Game1.instance.QueuLoadContent("bombIcon", (Texture2D t) => bombIcon = t);
            Game1.instance.QueuLoadContent("bombEmptyIcon", (Texture2D t) => bombEmptyIcon = t);
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {

            Color c = new Color(15, 15, 15, 100);

            var batch = Game1.instance.spriteBatch;

            batch.Draw(point, new Rectangle(0, 0, radarWidth, rardarHeight), c);

            foreach (var item in radarDots)
            {
                batch.Draw(point, item.Key, item.Value);
            }


            batch.Draw(point, new Rectangle(4, 480 - maxLive - 6, 22, maxLive + 2), c);
            batch.Draw(point, new Rectangle(5, 480 - maxLive + (maxLive - live) - 5, 20, live), Color.Green);
            batch.DrawString(Game1.instance.font, score, new Vector2(5, 5), Color.Green);

        }

        public int DrawOrder
        {
            get;
            set;
        }

        public event EventHandler<EventArgs> DrawOrderChanged;

        public bool Visible
        {
            get;
            set;
        }

        public event EventHandler<EventArgs> VisibleChanged;
    }
}
