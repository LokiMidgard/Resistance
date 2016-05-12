
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Resistance.Configuration
{
    public class GameConfiguration
    {
        private float enemyShotSpeed = 48f;
        private int level;
        private int noHumans;
        private int noPredator;
        private int noCollector;
        private int noMine;
        private bool enemyTargetting;
        private PlayerConfiguration player = new PlayerConfiguration();
        private int test;
        private EnemyConfiguration collector = new EnemyConfiguration();
        private EnemyPredatorConfiguration predator = new EnemyPredatorConfiguration();
        private EnemyConfiguration mine = new EnemyConfiguration();
        private EnemyDestroyerConfiguration destroyer = new EnemyDestroyerConfiguration();


        public int WorldWidth { get; set; }
        public int WorldHeight { get; set; }


        public int RaiseHumansPerLevelMin { get; set; }
        public int RaiseHumansPerLevelMax { get; set; }


        public float EnemyShotSpeed
        {
            get { return enemyShotSpeed; }
            set { enemyShotSpeed = value; }
        }

        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public int NoHumans
        {
            get { return noHumans; }
            set { noHumans = value; }
        }

        public int NoPredator
        {
            get { return noPredator; }
            set { noPredator = value; }
        }

        public int NoCollector
        {
            get { return noCollector; }
            set { noCollector = value; }
        }

        public int NoMine
        {
            get { return noMine; }
            set { noMine = value; }
        }

        public bool EnemyTargetting
        {
            get { return enemyTargetting; }
            set { enemyTargetting = value; }
        }

        public PlayerConfiguration Player
        {
            get { return player; }
            set { player = value; }
        }

        public EnemyConfiguration Collector
        {
            get { return collector; }
            set { collector = value; }
        }

        public EnemyPredatorConfiguration Predator
        {
            get { return predator; }
            set { predator = value; }
        }

        public EnemyConfiguration Mine
        {
            get { return mine; }
            set
            {
                mine = value;
                mine.Speed = 80;
            }
        }

        public EnemyDestroyerConfiguration Destroyer
        {
            get { return destroyer; }
            set
            {
                destroyer = value;
                destroyer.Speed = 260f;
            }
        }

        public class PlayerConfiguration
        {
            private int shotCount = 10;
            private float speed = 64f;
            private int lifepoints = 5;
            private float timeTillMaxBombSize = 2f;

            public PlayerConfiguration()
            {
            }

            public int ShotCount
            {
                get { return shotCount; }
                set { shotCount = value; }
            }
            public float Speed
            {
                get { return speed; }
                set { speed = value; }
            }
            public int Lifepoints
            {
                get { return lifepoints; }
                set { lifepoints = value; }
            }

            public float MaxBombSizeWidth { get; set; } = 500;
            public float MaxBombSizeHeight { get; set; } = 500;

            public float TimeTillMaxBombSize
            {
                get { return timeTillMaxBombSize; }
                set { timeTillMaxBombSize = value; }
            }

            public float ShootSpeed { get; set; }
        }

        public class EnemyConfiguration
        {
            private float speed = 16f;

            public int NumberFirstLevel { get; set; }
            public int AppearsInLevel { get; set; }
            public int MinAditionalPerLevel { get; set; }
            public int MaxAditionalPerLevel { get; set; }

            public EnemyConfiguration()
            {
            }

            public float Speed
            {
                get { return speed; }
                set { speed = value; }
            }
        }

        public class EnemyPredatorConfiguration : EnemyConfiguration
        {
            private int numberOfShots = 3;

            public EnemyPredatorConfiguration()
            {
            }

            public int NumberOfShots
            {
                get { return numberOfShots; }
                set { numberOfShots = value; }
            }
        }

        public class EnemyDestroyerConfiguration : EnemyConfiguration
        {
            public double MinTimeBetweenAppereance { get; set; }
            public float ProbabilityPerSeccond { get; set; }
            public float RaiseProbabilityPerLevel { get; set; }
            public int DamageCapathyty { get; set; }
        }

    }
}
