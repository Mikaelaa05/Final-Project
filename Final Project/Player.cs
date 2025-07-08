using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_Project
{
    internal class Player
    {
        Texture2D playerTexture;
        Rectangle playerDisplay;
        Rectangle playerSource;
        Color playerColor;
        float velocityY;



        //public int HitboxMarginX = 25;      // Horizontal padding
        //public int HitboxMarginTop = 15;    // Top padding
        //public int HitboxMarginBottom = 5;  // Bottom padding



        public Player(
            Texture2D playerTexture,
            Rectangle playerDisplay,
            Rectangle playerSource,
            Color playerColor)
        {
            this.playerTexture = playerTexture;
            this.playerDisplay = playerDisplay;
            this.playerSource = playerSource;
            this.playerColor = playerColor;
        }

        public Texture2D PlayerTexture
        {
            get => playerTexture;
        }
        public Rectangle PlayerDisplay
        {
            get => playerDisplay; 
        }
        public Rectangle PlayerSource
        {
            get => playerSource;
        }
        public Color PlayerColor
        {
            get => playerColor;
        }
        public float VelocityY { get => velocityY ;  }

        public void MoveVertical(int steps, int dir)
        {
            playerDisplay.Y += steps * dir;
        }

        public void MoveHorizontal(int steps, int dir)
        {
            playerDisplay.X += steps * dir;
        }

        public void TeleportY(int dest)
        {
            playerDisplay.Y = dest;
        }

        public void SetPosition(int x, int y)
        {
            playerDisplay.X = x;
            playerDisplay.Y = y;
        }

        public void SetSource(Rectangle newSource)
        {
            playerSource = newSource;
        }

        public Rectangle GetHitbox(int marginX, int marginTop, int marginBottom)
        {
            return new Rectangle(
                playerDisplay.X + marginX,
                playerDisplay.Y + marginTop,
                playerDisplay.Width - 2 * marginX,
                playerDisplay.Height - marginTop - marginBottom
            );
        }

        public void PlayerAnimator(int currentFrame, int startFrame, int endFrame)
        {
            int animationLength = endFrame - startFrame + 1;
            int textureX = playerTexture.Width / 8;
            int textureY = playerTexture.Height / 2;
            currentFrame = (currentFrame / 6) % animationLength;

            int frameX = (startFrame + currentFrame) % 8;
            int frameY = (startFrame + currentFrame) / 8;

            playerSource = new Rectangle(frameX * textureX, frameY * textureY, textureX, textureY);
        }

        

        public void ChangeVelocityY(float newVelocityY, bool setToggle = false)
        {
            if (setToggle)
            {
                velocityY = newVelocityY;
            }
            else
                velocityY += newVelocityY;
           

        }

    }
}