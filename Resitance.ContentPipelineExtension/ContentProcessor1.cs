using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using Resistance.Configuration;
// TODO: replace these with the processor input and output types.
using TInput = System.Xml.Linq.XDocument;
using TOutput = Resistance.Configuration.GameConfiguration;
using System.Globalization;
using Microsoft.Xna.Framework;

namespace Resitance.ContentPipelineExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor(DisplayName = "Resistance Configuration Processor")]
    public class ContentProcessor1 : ContentProcessor<TInput, TOutput>
    {
        const String NAMESPACE = "http://resistance.res";

        public override TOutput Process(TInput input, ContentProcessorContext context)
        {

            //Debugger.Break();

            var root = input.Root;
            //System.Xml.Linq.XName f = new System.Xml.Linq.XName; 
            var world = root.Element(XName.Get("World", NAMESPACE));
            var worldWidth = world.Attribute("width");
            var worldHeight = world.Attribute("height");
            var EnemyShootspeed = world.Element(XName.Get("EnemyShotSpeed", NAMESPACE));

            var player = root.Element(XName.Get("Player", NAMESPACE));
            var maxLifePoints = player.Element(XName.Get("MaxLifePoints", NAMESPACE));
            var PlayerSpeed = player.Element(XName.Get("Speed", NAMESPACE));
            var PlayerShootSpeed = player.Element(XName.Get("ShootSpeed", NAMESPACE));
            var PlayerShootCount = player.Element(XName.Get("ShootCount", NAMESPACE));
            var maxBombSize = player.Element(XName.Get("MaxBombSize", NAMESPACE));
            var timeToMaxBombSize = player.Element(XName.Get("TimeToMaxDetonation", NAMESPACE));

            var humans = root.Element(XName.Get("Human", NAMESPACE));
            var humanNumberFirstLevel = humans.Attribute("numberFirstLevel");
            var humanRaisePerLevelMin = humans.Attribute("raisePerLevelMin");
            var humanRaisePerLevelMax = humans.Attribute("raisePerLevelMax");

            var enemyPredator = root.Element(XName.Get("EnemyPredator", NAMESPACE));
            var enemyPredatorNumberFirstLevel = enemyPredator.Attribute("numberFirstLevel");
            var enemyPredatroAppearsInLevel = enemyPredator.Attribute("appearsInLevel");
            var enemyPredatorMinAditionalPerLevel = enemyPredator.Attribute("minAditionalPerLevel");
            var enemyPredatorMaxAditionalPerLevel = enemyPredator.Attribute("maxAditionalPerLevel");
            var enemyPredatorSpeed = enemyPredator.Element(XName.Get("Speed", NAMESPACE));
            var enemyPredatorTargetting = enemyPredator.Element(XName.Get("Targeting", NAMESPACE));


            var enemyCollector = root.Element(XName.Get("EnemyCollector", NAMESPACE));
            var enemyCollectorNumberFirstLevel = enemyCollector.Attribute("numberFirstLevel");
            var enemyCollectorAppearsInLevel = enemyCollector.Attribute("appearsInLevel");
            var enemyCollectorMinAditionalPerLevel = enemyCollector.Attribute("minAditionalPerLevel");
            var enemyCollectorMaxAditionalPerLevel = enemyCollector.Attribute("maxAditionalPerLevel");
            var enemyCollectorSpeed = enemyCollector.Element(XName.Get("Speed", NAMESPACE));

            var enemyMine = root.Element(XName.Get("EnemyMine", NAMESPACE));
            var enemyMineNumberFirstLevel = enemyMine.Attribute("numberFirstLevel");
            var enemyMineAppearsInLevel = enemyMine.Attribute("appearsInLevel");
            var enemyMineMinAditionalPerLevel = enemyMine.Attribute("minAditionalPerLevel");
            var enemyMineMaxAditionalPerLevel = enemyMine.Attribute("maxAditionalPerLevel");
            var enemyMineSpeed = enemyMine.Element(XName.Get("Speed", NAMESPACE));

            var enemyDestroyer = root.Element(XName.Get("EnemyDestroyer", NAMESPACE));
            var enemyDestroyerAppearsInLevel = enemyDestroyer.Attribute("appearsInLevel");
            var enemyDestroyerMinTimeBetweenAppereance = enemyDestroyer.Attribute("minTimeBetweenAppereance");
            var enemyDestroyerProbabilityPerSeccond = enemyDestroyer.Attribute("probabilityPerSeccond");
            var enemyDestroyerRaiseProbabilityPerLevel = enemyDestroyer.Attribute("raiseProbabilityPerLevel");
            var enemyDestroyerSpeed = enemyDestroyer.Element(XName.Get("Speed", NAMESPACE));
            var enemyDestroyerDamageCapathity = enemyDestroyer.Element(XName.Get("DamageCapathyty", NAMESPACE));




            GameConfiguration g = new GameConfiguration();
            g.WorldHeight = int.Parse(worldHeight.Value, CultureInfo.InvariantCulture.NumberFormat);
            g.WorldWidth = int.Parse(worldWidth.Value, CultureInfo.InvariantCulture.NumberFormat);
            g.RaiseHumansPerLevelMax = int.Parse(humanRaisePerLevelMax.Value, CultureInfo.InvariantCulture.NumberFormat);
            g.RaiseHumansPerLevelMin = int.Parse(humanRaisePerLevelMin.Value, CultureInfo.InvariantCulture.NumberFormat);

            g.EnemyShotSpeed = float.Parse(EnemyShootspeed.Value, CultureInfo.InvariantCulture.NumberFormat);
            g.Level = 1;
            g.NoHumans = int.Parse(humanNumberFirstLevel.Value, CultureInfo.InvariantCulture.NumberFormat);
            g.NoPredator = int.Parse(enemyPredatroAppearsInLevel.Value, CultureInfo.InvariantCulture.NumberFormat) == 1 ? int.Parse(enemyPredatorNumberFirstLevel.Value, CultureInfo.InvariantCulture.NumberFormat) : 0;
            g.NoCollector = int.Parse(enemyCollectorAppearsInLevel.Value, CultureInfo.InvariantCulture.NumberFormat) == 1 ? int.Parse(enemyCollectorNumberFirstLevel.Value, CultureInfo.InvariantCulture.NumberFormat) : 0;
            g.NoMine = int.Parse(enemyMineAppearsInLevel.Value, CultureInfo.InvariantCulture.NumberFormat) == 1 ? int.Parse(enemyMineNumberFirstLevel.Value, CultureInfo.InvariantCulture.NumberFormat) : 0;
            g.EnemyTargetting = bool.Parse(enemyPredatorTargetting.Value);
            g.Player = new GameConfiguration.PlayerConfiguration()
            {
                Lifepoints = int.Parse(maxLifePoints.Value, CultureInfo.InvariantCulture.NumberFormat),
                MaxBombSizeWidth = float.Parse(maxBombSize.Element(XName.Get("X", NAMESPACE)).Value, CultureInfo.InvariantCulture.NumberFormat),
                MaxBombSizeHeight = float.Parse(maxBombSize.Element(XName.Get("Y", NAMESPACE)).Value, CultureInfo.InvariantCulture.NumberFormat),
                ShotCount = int.Parse(PlayerShootCount.Value, CultureInfo.InvariantCulture.NumberFormat),
                Speed = float.Parse(PlayerSpeed.Value, CultureInfo.InvariantCulture.NumberFormat),
                TimeTillMaxBombSize = float.Parse(timeToMaxBombSize.Value, CultureInfo.InvariantCulture.NumberFormat),
                ShootSpeed = float.Parse(PlayerShootSpeed.Value, CultureInfo.InvariantCulture.NumberFormat)
            };
            g.Collector = new GameConfiguration.EnemyConfiguration()
            {
                Speed = float.Parse(enemyCollectorSpeed.Value, CultureInfo.InvariantCulture.NumberFormat),
                AppearsInLevel = int.Parse(enemyCollectorAppearsInLevel.Value, CultureInfo.InvariantCulture.NumberFormat),
                MaxAditionalPerLevel = int.Parse(enemyCollectorMaxAditionalPerLevel.Value, CultureInfo.InvariantCulture.NumberFormat),
                MinAditionalPerLevel = int.Parse(enemyCollectorMinAditionalPerLevel.Value, CultureInfo.InvariantCulture.NumberFormat),
                NumberFirstLevel = int.Parse(enemyCollectorNumberFirstLevel.Value, CultureInfo.InvariantCulture.NumberFormat)
            };
            g.Predator = new GameConfiguration.EnemyPredatorConfiguration()
            {
                Speed = float.Parse(enemyPredatorSpeed.Value, CultureInfo.InvariantCulture.NumberFormat),
                AppearsInLevel = int.Parse(enemyPredatroAppearsInLevel.Value, CultureInfo.InvariantCulture.NumberFormat),
                MaxAditionalPerLevel = int.Parse(enemyPredatorMaxAditionalPerLevel.Value, CultureInfo.InvariantCulture.NumberFormat),
                MinAditionalPerLevel = int.Parse(enemyPredatorMinAditionalPerLevel.Value, CultureInfo.InvariantCulture.NumberFormat),
                NumberFirstLevel = int.Parse(enemyCollectorNumberFirstLevel.Value, CultureInfo.InvariantCulture.NumberFormat)
            };
            g.Mine = new GameConfiguration.EnemyConfiguration()
            {
                Speed = float.Parse(enemyMineSpeed.Value, CultureInfo.InvariantCulture.NumberFormat),
                AppearsInLevel = int.Parse(enemyMineAppearsInLevel.Value, CultureInfo.InvariantCulture.NumberFormat),
                MaxAditionalPerLevel = int.Parse(enemyMineMaxAditionalPerLevel.Value, CultureInfo.InvariantCulture.NumberFormat),
                MinAditionalPerLevel = int.Parse(enemyMineMinAditionalPerLevel.Value, CultureInfo.InvariantCulture.NumberFormat),
                NumberFirstLevel = int.Parse(enemyMineNumberFirstLevel.Value, CultureInfo.InvariantCulture.NumberFormat)
            };
            g.Destroyer = new GameConfiguration.EnemyDestroyerConfiguration()
            {
                Speed = float.Parse(enemyDestroyerSpeed.Value, CultureInfo.InvariantCulture.NumberFormat),
                NumberFirstLevel = 1,
                MinAditionalPerLevel = 0,
                MaxAditionalPerLevel = 0,
                AppearsInLevel = int.Parse(enemyDestroyerAppearsInLevel.Value, CultureInfo.InvariantCulture.NumberFormat),
                MinTimeBetweenAppereance = double.Parse(enemyDestroyerMinTimeBetweenAppereance.Value, CultureInfo.InvariantCulture),
                ProbabilityPerSeccond = float.Parse(enemyDestroyerProbabilityPerSeccond.Value, CultureInfo.InvariantCulture.NumberFormat),
                RaiseProbabilityPerLevel = float.Parse(enemyDestroyerRaiseProbabilityPerLevel.Value, CultureInfo.InvariantCulture.NumberFormat),
                DamageCapathyty = int.Parse(enemyDestroyerDamageCapathity.Value, CultureInfo.InvariantCulture.NumberFormat)
            };



            return g;

        }
    }
}