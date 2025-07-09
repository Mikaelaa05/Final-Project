using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project
{
    public class Checkpoint
    {
        Texture2D checkpointTexture;
        Rectangle checkpointDisplay;
        Rectangle checkpointSource;
        Color checkpointColor;
        int collected;

        public Checkpoint(Texture2D checkpointTexture, Rectangle checkpointDisplay, Rectangle checkpointSource, Color checkpointColor)
        {
            this.checkpointTexture = checkpointTexture;
            this.checkpointDisplay = checkpointDisplay;
            this.checkpointSource = checkpointSource;
            this.checkpointColor = checkpointColor;
            collected = 0;
            
        }

        public Texture2D CheckpointTexture { get => checkpointTexture; }
        public Rectangle CheckpointDisplay { get => checkpointDisplay; }
        public Rectangle CheckpointSource { get => checkpointSource; }

        public Color CheckpointColor { get => checkpointColor; }

        public void collect()
        {
            collected = 1;
        }



        public void checkCP()
        {
            int frame = 1;
            if (collected == 1)
            {
                frame = 0;
            }
            checkpointSource = new Rectangle(frame * (checkpointTexture.Width / 2),0,checkpointTexture.Width / 2, checkpointTexture.Height);

        }


    }
}
