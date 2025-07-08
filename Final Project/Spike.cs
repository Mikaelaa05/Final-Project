using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project
{
    public class Spike
    {
        public Texture2D Texture { get;  }
        public Rectangle Display { get;  }
        public Rectangle Source { get;  }
        public Color Color { get;  }

        public Spike(Texture2D texture, Rectangle display, Rectangle source, Color color)
        {
            Texture = texture;
            Display = display;
            Source = source;
            Color = color;
        }
    }
}