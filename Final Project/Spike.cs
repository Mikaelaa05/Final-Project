using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project
{
    public class Spike
    {
        public Texture2D Texture { get; set; }
        public Rectangle Display { get; set; }
        public Rectangle Source { get; set; }
        public Color Color { get; set; }

        public Spike(Texture2D texture, Rectangle display, Rectangle source, Color color)
        {
            Texture = texture;
            Display = display;
            Source = source;
            Color = color;
        }
    }
}