using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project
{
    internal class Background
    {
        Texture2D backgroundTexture;
        Rectangle backgroundRectangle;
        Color backgroundColor;

        public Background(Texture2D backgroundTexture, Rectangle backgroundRectangle, Color backgroundColor)
        {
            this.backgroundTexture = backgroundTexture;
            this.backgroundRectangle = backgroundRectangle;
            this.backgroundColor = backgroundColor;
        }

        public Texture2D BackgroundTexture { get => backgroundTexture; }
        public Rectangle BackgroundRectangle { get => backgroundRectangle; }
        public Color BackgroundColor { get => backgroundColor; }
    }
}