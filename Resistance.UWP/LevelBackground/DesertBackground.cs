using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Resistance.Components;
using Microsoft.Xna.Framework.Graphics;

using Microsoft.Xna.Framework;
using Resistance.Scene;
using Resistance.Sprite;

namespace Resistance.LevelBackground
{
    class DesertBackground : IDrawableComponent
    {


        Texture2D[] cloud = new Texture2D[5];
        Vector2 cloudCoordinats;


        Texture2D gradient;
        float gradientVerticalOffset;

        Texture2D[] hill = new Texture2D[4];
        Vector2[] hillCoordinats = new Vector2[12];

        Texture2D[] mountain = new Texture2D[2];
        Vector2[] mountainCoordinats = new Vector2[6];

        Texture2D star;
        Vector2[] starCoordinats = new Vector2[4];


        GameScene scene;

        City[] cytiys = new City[5];

        public DesertBackground(GameScene scene)
        {
            this.scene = scene;

            for (int i = 0; i < cytiys.Length; ++i)
            {
                cytiys[i] = City.create(scene);
            }
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {




            float tmp1 = scene.ViewPort.X * 0.9f; // viewport *0,9;


            float pre = (scene.configuration.WorldHeight - (scene.ViewPort.Y + GameScene.VIEWPORT_HEIGHT));

            starCoordinats[0] = new Vector2(tmp1 + -400, (scene.ViewPort.Y + (GameScene.VIEWPORT_HEIGHT >> 1) - 480) * 9 / 10 + 180);
            starCoordinats[1] = new Vector2(tmp1 + 0, (scene.ViewPort.Y + (GameScene.VIEWPORT_HEIGHT >> 1) - 480) * 9 / 10 + 180);
            starCoordinats[2] = new Vector2(tmp1 + 400, (scene.ViewPort.Y + (GameScene.VIEWPORT_HEIGHT >> 1) - 480) * 9 / 10 + 180);
            starCoordinats[3] = new Vector2(tmp1 + 800, (scene.ViewPort.Y + (GameScene.VIEWPORT_HEIGHT >> 1) - 480) * 9 / 10 + 180);


            gradientVerticalOffset = (float)((scene.configuration.WorldHeight - gradient.Bounds.Height) - (pre * 0.8)); //(((pre * 16) + (pre * 4) + pre) / 32) + 350;


            tmp1 = scene.ViewPort.X * 0.6f; // viewport*6
            float tmp2 = (float)((scene.configuration.WorldHeight - this.mountain[0].Bounds.Height) - (pre * 0.6)); //(((pre * 16) + (pre * 4) + pre) / 32) + 350;

            mountainCoordinats[0] = new Vector2(tmp1 + -1600, tmp2);
            mountainCoordinats[1] = new Vector2(tmp1 + -800, tmp2);
            mountainCoordinats[2] = new Vector2(tmp1 + 0, tmp2);
            mountainCoordinats[3] = new Vector2(tmp1 + 800, tmp2);
            mountainCoordinats[4] = new Vector2(tmp1 + 1600, tmp2);
            mountainCoordinats[5] = new Vector2(tmp1 + 2400, tmp2);


            tmp1 = scene.ViewPort.X * 0.4f;

            for (int i = 0; i < cytiys.Length; ++i)
            {

                var c = cytiys[i];
                //c.Position = new Vector2((scene.ViewPort.X * c.paralxSpeed) + c.OriginalPosition.X, ((pre) * c.paralxSpeed) + c.OriginalPosition.Y);
                c.Position = new Vector2((scene.ViewPort.X * c.paralxSpeed) + c.OriginalPosition.X, (float)((c.OriginalPosition.Y) - (pre * c.paralxSpeed)));


            }

            tmp1 = scene.ViewPort.X * 0.2f;
            tmp2 = (float)((scene.configuration.WorldHeight - this.hill[0].Bounds.Height) - (pre * 0.2)); //(((pre * 16) + (pre * 4) + pre) / 32) + 350;

            hillCoordinats[0] = new Vector2((tmp1 + -3200), tmp2);
            hillCoordinats[1] = new Vector2((tmp1 + -2400), tmp2);
            hillCoordinats[2] = new Vector2((tmp1 + -1600), tmp2);
            hillCoordinats[3] = new Vector2((tmp1 + -800), tmp2);
            hillCoordinats[4] = new Vector2((tmp1 + 0), tmp2);
            hillCoordinats[5] = new Vector2((tmp1 + 800), tmp2);
            hillCoordinats[6] = new Vector2((tmp1 + 1600), tmp2);
            hillCoordinats[7] = new Vector2((tmp1 + 2400), tmp2);
            hillCoordinats[8] = new Vector2((tmp1 + 3200), tmp2);
            hillCoordinats[9] = new Vector2((tmp1 + 4000), tmp2);
            hillCoordinats[10] = new Vector2((tmp1 + 4800), tmp2);
            hillCoordinats[11] = new Vector2((tmp1 + 5200), tmp2);
        }

        public void Initilize()
        {
            for (int i = 0; i < cytiys.Length; i++)
            {
                int j = i;
                cytiys[i].Initilize();
            }

            for (int i = 0; i < cloud.Length; i++)
            {
                int j = i;
                Game1.instance.QueuLoadContent("cloud" + (i + 1), (Texture2D t) => cloud[j] = t);
            }

            for (int i = 0; i < hill.Length; i++)
            {
                int j = i;
                Game1.instance.QueuLoadContent("hills" + (j + 1), (Texture2D t) => hill[j] = t);
            }

            for (int i = 0; i < mountain.Length; i++)
            {
                int j = i;
                Game1.instance.QueuLoadContent("mountains" + (i + 1), (Texture2D t) => mountain[j] = t);
            }
            Game1.instance.QueuLoadContent("gradient", (Texture2D t) => gradient = t);
            Game1.instance.QueuLoadContent("stars", (Texture2D t) => star = t);
            cytiys = cytiys.OrderByDescending(c => c.paralxSpeed).ToArray();

        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            var batch = Game1.instance.spriteBatch;

            for (int y = 0; y < GameScene.VIEWPORT_HEIGHT + star.Bounds.Height; y += star.Bounds.Height)
            {

                for (int i = 0; i < starCoordinats.Length; i++)
                {
                    batch.Draw(star, starCoordinats[i] - scene.ViewPort + new Vector2(0, y), Color.White);
                }
            }
            batch.Draw(gradient, new Rectangle(0, (int)(gradientVerticalOffset - scene.ViewPort.Y), GameScene.VIEWPORT_WIDTH, 480), Color.White);

            for (int i = 0; i < mountainCoordinats.Length; i += 2)
            {
                batch.Draw(mountain[0], mountainCoordinats[i] - scene.ViewPort, Color.White);
                batch.Draw(mountain[1], mountainCoordinats[i + 1] - scene.ViewPort, Color.White);
            }


            for (int i = 0; i < cytiys.Length; i += 2)
            {
                cytiys[i].Draw(gameTime);
            }


            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 4; j++)
                {

                    batch.Draw(hill[j], hillCoordinats[i * 4 + j] - scene.ViewPort, Color.White);

                }
            }

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
