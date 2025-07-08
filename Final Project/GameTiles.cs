using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project
{
    public class GameTiles
    {
        public Texture2D Texture { get; }
        public Rectangle TileDisplay { get; }
        public Rectangle TileSource { get; }
        public Color TileColor { get; }

        public GameTiles(Texture2D texture, Rectangle display, Rectangle source, Color color)
        {
            Texture = texture;
            TileDisplay = display;
            TileSource = source;
            TileColor = color;
 
        }
    }
}
