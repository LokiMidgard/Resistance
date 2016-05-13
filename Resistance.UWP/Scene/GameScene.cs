using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Resistance.Input;
using Resistance.Sprite;

using Microsoft.Xna.Framework;
using Resistance.LevelBackground;
using Resistance.Configuration;
using System.Runtime.Serialization;

namespace Resistance.Scene
{
    public class GameScene : IScene
    {


        public const int VIEWPORT_WIDTH = 800;
        public const int VIEWPORT_HEIGHT = 480;

        public const float SHOTSPEED_NORMAL = 48f;
        public const float SHOTSPEED_FAST = 64f;
        public const float SHOTSPEED_SLOW = 24f;


        public GameConfiguration configuration;// = new GameConfiguration();

        DesertBackground background;

        Hud hud;

        public Vector2 ViewPort;

        public GameInput input;

        public Dificulty difficulty;

        public Player player;

        public EnemyDestroyer destroyer;
        public List<AbstractEnemy> notDestroyedEnemys = new List<AbstractEnemy>();
        public List<Human> notKilledHumans = new List<Human>();
        public List<AbstractEnemy> allEnemys = new List<AbstractEnemy>();
        public List<Human> allHumans = new List<Human>();
        public List<EnemyPredator.Shot> allEnemyShots = new List<EnemyPredator.Shot>();

        public int score;
        public int scoreBeginLevel;
        private double bosCountdown;
        private double destroyerTime;

        public GameScene(GameTombstone t)
        {

            PrepareGame();

            this.difficulty = t.Difficulty;
            player = new Player(this);
            input = new GameInput();

            background = new DesertBackground(this);
            hud = new Hud(this);


            configuration.EnemyShotSpeed = t.EnemyShotSpeed;
            configuration.EnemyTargetting = t.EnemyTargetting;
            configuration.Level = t.Level;
            configuration.NoCollector = t.NoCollector;
            configuration.NoHumans = t.NoHumans;
            configuration.NoPredator = t.NoPredetor;
            configuration.NoMine = t.NoMine;
            score = t.Score;
            scoreBeginLevel = score;
            destroyer = new EnemyDestroyer(this);

            CreateNewEnemys(false);


        }

        public GameScene(Dificulty dificulty)
        {
            PrepareGame();
            this.difficulty = dificulty;
            player = new Player(this);
            input = new GameInput();

            background = new DesertBackground(this);
            hud = new Hud(this);
            destroyer = new EnemyDestroyer(this);
            CreateNewEnemys(false);


        }

        private void PrepareGame()
        {
            configuration = Game1.instance.Content.Load<GameConfiguration>(@"Configuration\GameConfig");
        }

        private void CreateNewEnemys(bool gameRunning)
        {
            var newEnemys = new List<AbstractEnemy>();
            var newHumans = new List<Human>();
            for (int i = allEnemys.Count(x => x is EnemyPredator); i < configuration.NoPredator; i++)
            {

                EnemyPredator e = new EnemyPredator(this);
                newEnemys.Add(e);
            }
            for (int i = allEnemys.Count(x => x is EnemyCollector); i < configuration.NoCollector; i++)
            {
                EnemyCollector e = new EnemyCollector(this);
                newEnemys.Add(e);
            }

            for (int i = allHumans.Count; i < configuration.NoHumans; i++)
            {
                Human e = new Human(this);
                newHumans.Add(e);
            }
            for (int i = newEnemys.Count(x => x is EnemyMine); i < configuration.NoMine; i++)
            {
                EnemyMine e = new EnemyMine(this);
                newEnemys.Add(e);
            }

            allHumans.AddRange(newHumans);
            notKilledHumans.Clear();
            notKilledHumans.AddRange(allHumans);


            allEnemys.AddRange(newEnemys);
            notDestroyedEnemys.Clear();
            notDestroyedEnemys.AddRange(allEnemys);

            foreach (var h in allHumans)
            {
                h.Initilize();
            }
            foreach (var h in allEnemys)
            {
                h.Initilize();
            }
            if (gameRunning)
            {
                Game1.instance.LoadContentImidetly();
            }
        }

        public void Initilize()
        {
            if (((int)difficulty + 1) * 2 >= Game1.random.Next(10) + 1)
                destroyerTime = (3 - (int)difficulty) << 10;
            destroyer.Initilize();
            foreach (var h in allHumans)
            {
                h.Initilize();
            }
            foreach (var h in allEnemys)
            {
                h.Initilize();
            }

            input.Initilize();

            player.Initilize();
            background.Initilize();
            hud.Initilize();
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {

            if (notKilledHumans.Count == 0)
            {
                GameOver();
                return;
            }
            if (notDestroyedEnemys.Count == 0)
            {
                NextLevel();
                return;
            }

            input.Update(gameTime);


            foreach (var h in notKilledHumans)
            {
                h.Update(gameTime);
            }
            List<AbstractEnemy> eToDelete = new List<AbstractEnemy>();
            foreach (var h in notDestroyedEnemys)
            {
                h.Update(gameTime);
                if (!h.Visible)
                    eToDelete.Add(h);
            }
            foreach (var e in eToDelete)
            {
                notDestroyedEnemys.Remove(e);
            }

            destroyer.Update(gameTime);

            foreach (var s in allEnemyShots)
            {
                s.Update(gameTime);
            }

            bosCountdown += gameTime.ElapsedGameTime.TotalSeconds;
            if ((4 - (int)difficulty * 2) < configuration.Level && destroyerTime > 0 && destroyerTime <= bosCountdown)
            {
                destroyer.ReEnter();
                bosCountdown = 0.0;
                if (((int)difficulty + 1) * 2 >= Game1.random.Next(10) + 1)
                {
                    destroyerTime = (3 - (int)difficulty) << 10;
                }
                else
                {
                    destroyerTime = 0;
                }
            }

            player.Update(gameTime);

            MoveEveryThing();


            background.Update(gameTime);
            hud.Update(gameTime);
        }

        private void MoveEveryThing()
        {
            if (player.Position.X < 0)
            {
                player.Position += new Vector2(configuration.WorldWidth, 0f);
            }
            else if (player.Position.X > configuration.WorldWidth)
            {
                player.Position -= new Vector2(configuration.WorldWidth, 0f);
            }

            ViewPort = (player.Position * new Vector2(1, 1)) - new Vector2(VIEWPORT_WIDTH / 2, VIEWPORT_HEIGHT / 2);
            ViewPort.Y = MathHelper.Clamp(ViewPort.Y, 0, configuration.WorldHeight - VIEWPORT_HEIGHT);

            foreach (var s in notDestroyedEnemys.Union(new AbstractEnemy[] { destroyer }))
            {
                if (s.Position.X < ViewPort.X - (configuration.WorldWidth >> 1))
                {
                    s.Position += new Vector2(configuration.WorldWidth, 0f);
                }
                else if (s.Position.X > ViewPort.X + (configuration.WorldWidth >> 1))
                {
                    s.Position -= new Vector2(configuration.WorldWidth, 0f);
                }
            }

            if (player.bomb.Position.X < ViewPort.X - (configuration.WorldWidth >> 1))
            {
                player.bomb.Position += new Vector2(configuration.WorldWidth, 0f);
            }
            else if (player.bomb.Position.X > ViewPort.X + (configuration.WorldWidth >> 1))
            {
                player.bomb.Position -= new Vector2(configuration.WorldWidth, 0f);
            }

            foreach (var s in player.allShots)
            {

                if (s.Position.X < ViewPort.X - (configuration.WorldWidth >> 1))
                {
                    s.Position+= new Vector2(configuration.WorldWidth, 0f);
                }
                else if (s.Position.X > ViewPort.X + (configuration.WorldWidth >> 1))
                {
                    s.Position -= new Vector2(configuration.WorldWidth, 0f);
                }
            }

            foreach (var s in allEnemyShots)
            {

                if (s.Position.X < ViewPort.X - (configuration.WorldWidth >> 1))
                {
                    s.Position += new Vector2(configuration.WorldWidth, 0f);


                }
                else if (s.Position.X > ViewPort.X + (configuration.WorldWidth >> 1))
                {
                    s.Position -= new Vector2(configuration.WorldWidth, 0f);


                }
            }

            foreach (var s in notKilledHumans)
            {

                if (s.Position.X < ViewPort.X - (configuration.WorldWidth >> 1))
                {
                    s.Position += new Vector2(configuration.WorldWidth, 0f);


                }
                else if (s.Position.X > ViewPort.X + (configuration.WorldWidth >> 1))
                {
                    s.Position -= new Vector2(configuration.WorldWidth, 0f);


                }
            }





        }

        public enum Dificulty
        {
            Easy = 0,
            Medium = 1,
            Hard = 2
        }

        private void NextLevel()
        {
            scoreBeginLevel = score;
            ++configuration.Level;
            // EnemyDestroyer.clearDestroyer();
            switch (difficulty)
            {
                case Dificulty.Easy:
                    configuration.NoHumans = notKilledHumans.Count + Game1.random.Next(3) + 3;
                    configuration.NoPredator += (Game1.random.Next(3) + 2);
                    configuration.NoCollector += (Game1.random.Next(3) + 2);
                    if (configuration.Level > 9)
                    {
                        configuration.EnemyTargetting = true;
                    }
                    if (configuration.Level > 3)
                    {
                        this.configuration.EnemyShotSpeed = SHOTSPEED_NORMAL;
                    }
                    if (configuration.Level > 14)
                    {
                        this.configuration.EnemyShotSpeed = SHOTSPEED_FAST;
                    }
                    if (configuration.Level > 2)
                    {
                        configuration.NoMine += (Game1.random.Next(3) + 1);
                    }
                    ;
                    break;
                case Dificulty.Medium:
                    configuration.NoHumans = notKilledHumans.Count + Game1.random.Next(4) + 2;
                    configuration.NoPredator += (Game1.random.Next(2) + 3);
                    configuration.NoCollector += (Game1.random.Next(2) + 3);
                    if (configuration.Level > 5)
                    {
                        configuration.EnemyTargetting = true;
                    }
                    if (configuration.Level > 2)
                    {
                        this.configuration.EnemyShotSpeed = SHOTSPEED_NORMAL;
                    }
                    if (configuration.Level > 9)
                    {
                        this.configuration.EnemyShotSpeed = SHOTSPEED_FAST;
                    }

                    if (configuration.Level > 1)
                    {
                        configuration.NoMine += (Game1.random.Next(4) + 2);
                    }
                    break;
                case Dificulty.Hard:
                    configuration.NoHumans = notKilledHumans.Count + Game1.random.Next(5) + 1;
                    configuration.NoPredator += (Game1.random.Next(1) + 4);
                    configuration.NoCollector += (Game1.random.Next(1) + 4);
                    if (configuration.Level > 3)
                    {
                        this.configuration.EnemyShotSpeed = SHOTSPEED_FAST;
                    }
                    if (configuration.Level > 1)
                    {
                        configuration.NoMine += (Game1.random.Next(2) + 4);
                    }
                    break;
            }
            CreateNewEnemys(true);


        }

        public void GameOver()
        {
            Game1.instance.SwitchToScene(new GameOverScene(score));
        }

        public void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            var batch = Game1.instance.spriteBatch;
            batch.Begin(transformMatrix: Game1.instance.ScaleMatrix);

            background.Draw(gameTime);

            player.Draw(gameTime);

            foreach (var s in allEnemyShots)
            {
                s.Draw(gameTime);
            }

            foreach (var h in notKilledHumans)
            {
                h.Draw(gameTime);
            }
            foreach (var h in notDestroyedEnemys)
            {
                h.Draw(gameTime);
            }

            destroyer.Draw(gameTime);

            input.Draw(gameTime);

            hud.Draw(gameTime);


            batch.End();
        }

        public void DoneLoading()
        {

        }



        public GameTombstone Tombstone => new GameScene.GameTombstone()
        {
            Level = configuration.Level,
            Difficulty = difficulty,
            EnemyShotSpeed = configuration.EnemyShotSpeed,
            EnemyTargetting = configuration.EnemyTargetting,
            NoCollector = configuration.NoCollector,
            NoHumans = configuration.NoHumans,
            NoMine = configuration.NoMine,
            NoPredetor = configuration.NoPredator,
            Score = scoreBeginLevel
        };

        [DataContract]
        public struct GameTombstone
        {
            [DataMember]
            public int Level { get; set; }
            [DataMember]
            public Dificulty Difficulty { get; set; }
            [DataMember]
            public float EnemyShotSpeed { get; set; }
            [DataMember]
            public bool EnemyTargetting { get; set; }
            [DataMember]
            public int NoCollector { get; set; }
            [DataMember]
            public int NoHumans { get; set; }
            [DataMember]
            public int NoPredetor { get; set; }
            [DataMember]
            public int Score { get; set; }
            [DataMember]
            public int NoMine { get; set; }
        }





    }
}
