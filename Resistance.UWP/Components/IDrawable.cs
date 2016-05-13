using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resistance.Components
{
    public interface IDrawable
    {
        bool Visible { get; }

        void Draw(GameTime gameTime);
    }
}
