using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Resistance.Scene;

namespace Resistance.Sprite
{
    class AnimatedMenueEntry : Sprite
    {
        public AnimatedMenueEntry(string imageName, GameScene scene) : base(imageName, scene)
        {
        }

        public override Texture2D Image { get; set; }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
