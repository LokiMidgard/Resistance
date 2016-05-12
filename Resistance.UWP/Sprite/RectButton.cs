using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Resistance.Components;
using Resistance.UWP;

namespace Resistance.Sprite
{



    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class RectButton : IDrawableComponent
    {
        private Rectangle rec;

        private Texture2D tex;

        SpriteBatch batch = Game1.instance.spriteBatch;

        Color c;
        private readonly string buttonGraphicName;

        public RectButton(Rectangle rec, string graphic = null)
        {
            this.rec = rec;
            this.buttonGraphicName = graphic;

        }






        public void Draw(GameTime gameTime)
        {


            var color = new Color(255, 255, 255, 80);
            color = c;

            batch.Draw(tex, new Rectangle(rec.Left, rec.Top, 5, 5), new Rectangle(0, 0, 5, 5), color);
            batch.Draw(tex, new Rectangle(rec.Right - 5, rec.Top, 5, 5), new Rectangle(6, 0, 5, 5), color);
            batch.Draw(tex, new Rectangle(rec.Left, rec.Bottom - 5, 5, 5), new Rectangle(0, 6, 5, 5), color);
            batch.Draw(tex, new Rectangle(rec.Right - 5, rec.Bottom - 5, 5, 5), new Rectangle(6, 6, 5, 5), color);

            batch.Draw(tex, new Rectangle(rec.Left + 5, rec.Top, rec.Width - 10, 5), new Rectangle(5, 0, 1, 5), color);
            batch.Draw(tex, new Rectangle(rec.Left + 5, rec.Bottom - 5, rec.Width - 10, 5), new Rectangle(5, 6, 1, 5), color);

            batch.Draw(tex, new Rectangle(rec.Left, rec.Top + 5, 5, rec.Height - 10), new Rectangle(0, 5, 5, 1), color);
            batch.Draw(tex, new Rectangle(rec.Right - 5, rec.Top + 5, 5, rec.Height - 10), new Rectangle(6, 5, 5, 1), color);

            batch.Draw(tex, new Rectangle(rec.Left + 5, rec.Top + 5, rec.Width - 10, rec.Height - 10), new Rectangle(5, 5, 1, 1), color);

            if (ButtonTexture != null)
            {
                var centerButtonTexture = new Vector2(ButtonTexture.Width / 2f, ButtonTexture.Height / 2f);
                batch.Draw(ButtonTexture, rec.Center.ToVector2() - centerButtonTexture, Color.White);
            }

        }



        public void Update(GameTime gameTime)
        {
            var touchPoints = Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState();

            c = Color.White;

            foreach (var t in touchPoints)
            {
                if (rec.Contains(new Point((int)t.Position.X, (int)t.Position.Y)))
                    c = Color.Beige;

            }
        }

        public void Initilize()
        {
            Game1.instance.QueuLoadContent("ButtonOutline", (Texture2D t) => tex = t);
            if (!string.IsNullOrWhiteSpace(this.buttonGraphicName))
                Game1.instance.QueuLoadContent(this.buttonGraphicName, (Texture2D t) => ButtonTexture = t);
        }


        public int DrawOrder
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<EventArgs> DrawOrderChanged;

        public bool Visible
        {
            get { throw new NotImplementedException(); }
        }

        public Texture2D ButtonTexture { get; private set; }

        public event EventHandler<EventArgs> VisibleChanged;
    }
}
