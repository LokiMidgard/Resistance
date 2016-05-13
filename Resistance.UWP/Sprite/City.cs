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
    class City : IDrawableComponent
    {
        private static Vector2 originCity1;
        private static Vector2 originCity2;
        private static Vector2 originCity3;

        public float ParalxSpeed { get; }
        public float Scalewidth { get; }
        private City lowerCity;
        private City higherCity;


        private static Texture2D city1;
        private static Texture2D city2;
        private static Texture2D city3;

        private static bool loaded;




        public void Initilize()
        {
            if (!loaded)
            {
                Game1.instance.QueuLoadContent("city1", (Texture2D t) =>
                {
                    city1 = t;
                    originCity1 = new Vector2(city1.Bounds.Width / 2, city1.Bounds.Height);
                });
                Game1.instance.QueuLoadContent("city2", (Texture2D t) =>
                {
                    city2 = t;
                    originCity2 = new Vector2(city2.Bounds.Width / 2, city2.Bounds.Height);
                });
                Game1.instance.QueuLoadContent("city3", (Texture2D t) =>
                {
                    city3 = t;
                    originCity3 = new Vector2(city3.Bounds.Width / 2, city3.Bounds.Height);
                });
                loaded = true;
                if (lowerCity != null && higherCity != null)
                {
                    lowerCity.Initilize();
                    higherCity.Initilize();
                }
            }
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            switch (image)
            {
                case CityNumber.City1:
                    Game1.instance.spriteBatch.Draw(city1, Position - scene.ViewPort, null, Color.White, 0f, originCity1, 1f, SpriteEffects.None, 0f);
                    break;
                case CityNumber.City2:
                    Game1.instance.spriteBatch.Draw(city2, Position - scene.ViewPort, null, Color.White, 0f, originCity2, 1f, SpriteEffects.None, 0f);
                    break;
                case CityNumber.City3:
                    Game1.instance.spriteBatch.Draw(city3, Position - scene.ViewPort, null, Color.White, 0f, originCity3, 1f, SpriteEffects.None, 0f);
                    break;

            }
            if (lowerCity != null && higherCity != null)
            {
                lowerCity.Draw(gameTime);
                higherCity.Draw(gameTime);
            }
        }



        public bool Visible { get; set; }


        public City(CityNumber image, float paralaxSpeed, GameScene scene)
        {
            if (paralaxSpeed <= 0f)
                throw new ArgumentException("Must be greater than 0", nameof(paralaxSpeed));
            this.image = CityNumber.None;
            this.ParalxSpeed = paralaxSpeed;
            this.scene = scene;
            lowerCity = new City(image, scene);
            higherCity = new City(image, scene);
            Scalewidth = ((ParalxSpeed * (float)GameScene.VIEWPORT_WIDTH + scene.configuration.WorldWidth * 0.35f));

            var posiblePositions = (int)(scene.configuration.WorldWidth - Scalewidth * 2);
            if (posiblePositions <= 0)
                throw new Exception($"ShouldNotHeappen \n\tScalewidth: {Scalewidth}\n\tscene.configuration.WorldWidth:{scene.configuration.WorldWidth}\n\tParalxSpeed{ ParalxSpeed }");
            OriginalPosition = new Vector2(Game1.random.Next(posiblePositions) + Scalewidth + GameScene.VIEWPORT_WIDTH, scene.configuration.WorldHeight - 10f - (1 - ParalxSpeed) * 10f);
            Visible = true;
            lowerCity.Visible = true;
            higherCity.Visible = true;
        }

        public City(CityNumber image, GameScene scene)
        {
            OriginalPosition = new Vector2(Game1.random.Next((int)(scene.configuration.WorldWidth - Scalewidth)), scene.configuration.WorldHeight - 10f - (1 - ParalxSpeed) * 70f);
            this.image = image;
            this.scene = scene;
        }

        /**
         * Erzeugt eine Stadt mit einem der Zufälligen 3 Bilder
         * @return
         */
        public static City create(GameScene scene)
        {
            CityNumber image = CityNumber.None;

            int r = Game1.random.Next(3);
            switch (r)
            {

                case 0:
                    image = CityNumber.City1;
                    break;
                case 1:
                    image = CityNumber.City2;
                    break;
                case 2:
                    image = CityNumber.City3;
                    break;
            }

            return new City(image, (float)(Game1.random.NextDouble() * 0.3 + 0.2), scene);
        }



        private Vector2 position;
        private CityNumber image;
        private int p;
        private GameScene scene;

        public Vector2 OriginalPosition { get; set; }

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                if (lowerCity != null && higherCity != null)
                {
                    lowerCity.Position = new Vector2(position.X - Scalewidth, position.Y);
                    higherCity.Position = new Vector2(position.X + Scalewidth, position.Y);
                }
            }
        }





        public void Update(GameTime gameTime)
        {
        }

        public enum CityNumber
        {
            None,
            City1,
            City2,
            City3
        }
    }
}
