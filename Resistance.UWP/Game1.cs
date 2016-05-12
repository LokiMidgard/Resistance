using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Resistance.Loading;
using Resistance.Musicplayer;
using Resistance.Scene;
using System;
using System.Collections.Generic;

namespace Resistance
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private const int VIEWPORT_WIDTH = 800;
        private const int VIEWPORT_HEIGHT = 480;
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;



        private GameScene.GameTombstone? tombstone = null;

        public static readonly Random random = new Random();

        public static Game1 instance;

        public SpriteFont font;

        public Texture2D clearStdBackground;

        private bool init;

        public Queue<Action> actionList = new Queue<Action>();

        float deltaFPSTime = 0;
        float fps = 0;


        IScene actualScene;




        public Game1()
        {
            instance = this;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            //graphics.PreferredBackBufferWidth = 800;
            //graphics.PreferredBackBufferHeight = 480;





        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            if (tombstone != null)
                SwitchToScene(new TitleScene(tombstone));
            else
                SwitchToScene(new TitleScene());
            TouchPanel.EnabledGestures = GestureType.Tap;

            this.IsFixedTimeStep = false;



            base.Initialize();

            init = true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("font");
            clearStdBackground = Content.Load<Texture2D>(@"Menue\ResTitelWP7_ohneTitle");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //we dont want to paint before we init;
            if (!init)
                return;
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            // The time since Update was called last
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            float fps = 1 / elapsed;
            deltaFPSTime += elapsed;
            if (deltaFPSTime > 1)
            {
                this.fps = fps;
                deltaFPSTime -= 1;
            }



            MusicManager.Update(gameTime);

            actualScene.Update(gameTime);


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //we dont want to paint before we init;
            if (!init)
                return;
            GraphicsDevice.Clear(Color.Black);


            var displayInformation = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();
            var width = Windows.UI.Xaml.Window.Current.Bounds.Width * displayInformation.RawPixelsPerViewPixel;
            var height = Windows.UI.Xaml.Window.Current.Bounds.Height * displayInformation.RawPixelsPerViewPixel;



            var scaleX = (float)(width / VIEWPORT_WIDTH);
            var scaleY = (float)(height / VIEWPORT_HEIGHT);

            if (scaleX < scaleY)
            {
                ScaleMatrix = Matrix.CreateScale(scaleX);
                GraphicsDevice.Viewport = new Viewport(0, (int)((height - VIEWPORT_HEIGHT * scaleX) / 2), (int)(VIEWPORT_WIDTH * scaleX), (int)(VIEWPORT_HEIGHT * scaleX));
            }
            else
            {
                ScaleMatrix = Matrix.CreateScale(scaleY);
                GraphicsDevice.Viewport = new Viewport((int)((width - VIEWPORT_WIDTH * scaleY) / 2), 0, (int)(VIEWPORT_WIDTH * scaleY), (int)(VIEWPORT_HEIGHT * scaleY));
            }

            actualScene.Draw(gameTime);

            spriteBatch.Begin(transformMatrix: ScaleMatrix);
            spriteBatch.DrawString(font, fps.ToString(), Vector2.Zero, Color.Magenta);
            spriteBatch.End();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SwitchToScene(IScene scene)
        {
            scene.Initilize();
            Action a = () =>
            {
                actualScene = scene;
                scene.DoneLoading();
            };
            if (actionList.Count > 0)
            {
                LoadingScene l = new LoadingScene(actionList, a);
                actualScene = l;
            }
            else
                a();
        }

        Dictionary<String, object> loaded = new Dictionary<string, object>();

        public Matrix ScaleMatrix { get; private set; }

        /// <summary>
        /// Tells the Game that it shuld Load every Content in the Loading Queue before Returning
        /// </summary>
        public void LoadContentImidetly()
        {
            while (actionList.Count != 0)
            {
                var action = actionList.Dequeue();
                action();
            }

        }

        /// <summary>
        ///         Tells the Game that this Contetn shuld be loaded, next Time when the scene Switchs Lods the Content.
        /// </summary>
        /// <typeparam name="T">Type of Content that will be Loaded</typeparam>
        /// <param name="name">The Name OF the Content, including the Folders</param>
        /// <param name="del">The Delegete that will be calld when the contetn is Load</param>
        public void QueuLoadContent<T>(String name, LoadedDelegate<T> del)
        {

            System.Action a = () =>
            {
                if (!loaded.ContainsKey(name))
                {
                    var x = Content.Load<T>(name);
                    loaded.Add(name, x);
                }
                //Thread.Sleep(500);
                del((T)loaded[name]);
            };

            actionList.Enqueue(a);


        }
    }
}
