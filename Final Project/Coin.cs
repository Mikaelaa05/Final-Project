using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project
{
    public class Coin
    {
        private Texture2D texture;
        private Vector2 position;
        private int frameWidth;
        private int frameHeight;
        private int frameCount = 6;
        private int currentFrame = 0;
        private double timer = 0;
        private double interval = 100; // milliseconds per frame

        private int drawWidth;
        private int drawHeight;

        public bool Collected { get; internal set; }

        public Coin(Texture2D texture, Vector2 position, int v, int v1)
        {
            this.texture = texture;
            this.position = position;
            this.drawWidth = drawWidth;
            this.drawHeight = drawHeight;
            frameWidth = texture.Width / frameCount;
            frameHeight = texture.Height;
        }

        public void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalMilliseconds; // Update the timer with elapsed time
            if (timer >= interval) // Check if enough time has passed to switch frames
            {
                currentFrame = (currentFrame + 1) % frameCount; // Move to the next frame, looping back to the start
                timer = 0; // Reset the timer
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight); // Calculate the source rectangle for the current frame
            Rectangle destRect = new Rectangle((int)position.X, (int)position.Y, drawWidth, drawHeight);
            spriteBatch.Draw(texture, position, sourceRect, Color.White); // Draw the coin using the current frame's source rectangle
        }
    }
}
