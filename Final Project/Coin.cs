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

        private bool collected = false;
        public bool Collected => collected;

        public Coin(Texture2D texture, Vector2 position, int drawWidth, int drawHeight)
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
            if (collected) return;

            timer += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer >= interval)
            {
                currentFrame = (currentFrame + 1) % frameCount;
                timer = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (collected) return;

            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);
            Rectangle destRect = new Rectangle((int)position.X, (int)position.Y, drawWidth, drawHeight);
            spriteBatch.Draw(texture, destRect, sourceRect, Color.White);
        }

        public Rectangle GetCollisionBox()
        {
            return new Rectangle((int)position.X, (int)position.Y, drawWidth, drawHeight);
        }

        public void Collect()
        {
            collected = true;
        }
    }
}