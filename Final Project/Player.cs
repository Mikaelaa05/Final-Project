using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Final_Project
{
    internal class Player
    {
        Texture2D playerTexture;
        Rectangle playerDisplay;
        Rectangle playerSource;
        Rectangle playerHitbox;
        Rectangle hitboxSource;
        Color playerColor;
        float velocityY;

        // Knockback variables
        public Vector2 knockbackVelocity = Vector2.Zero;
        public float knockbackDuration = 0f;
        public Vector2 position;
        public bool IsKnockbackActive;
        public const float knockbackTime = 0.2f; // duration in seconds
        public const float knockbackForce = 5f;


        //public int HitboxMarginX = 25;      // Horizontal padding
        //public int HitboxMarginTop = 15;    // Top padding
        //public int HitboxMarginBottom = 5;  // Bottom padding



        public Player(
            Texture2D playerTexture,
            Rectangle playerDisplay,
            Rectangle playerSource,
            Color playerColor, Rectangle playerHitbox)
        {
            this.playerTexture = playerTexture;
            this.PlayerDisplay = playerDisplay;
            this.playerSource = playerSource;
            this.playerColor = playerColor;
            this.playerHitbox = playerHitbox;
            hitboxSource = new Rectangle(0, 0, 32, 32);
        }

        public Texture2D PlayerTexture
        {
            get => playerTexture;
        }
        public Rectangle PlayerDisplay
        {
            get => playerDisplay;
            set => playerDisplay = value;
        }
        public Rectangle PlayerSource
        {
            get => playerSource;
        }
        public Color PlayerColor { get; set; } = Color.White;

        public Rectangle PlayerHitbox
        {
            get => playerHitbox;
        }

        public Rectangle HitboxSource
        {
            get => hitboxSource;
        }
        public float VelocityY { get => velocityY; }

        public void MoveVertical(int steps, int dir)
        {
            PlayerDisplay = new Rectangle(PlayerDisplay.X, PlayerDisplay.Y + steps * dir, PlayerDisplay.Width, PlayerDisplay.Height);
        }

        public void MoveHorizontal(int steps, int dir)
        {
            PlayerDisplay = new Rectangle(PlayerDisplay.X + steps * dir, PlayerDisplay.Y, PlayerDisplay.Width, PlayerDisplay.Height);
        }

        public void TeleportY(int dest)
        {
            PlayerDisplay = new Rectangle(PlayerDisplay.X, dest, PlayerDisplay.Width, PlayerDisplay.Height);
        }

        public void SetPosition(int x, int y)
        {
            PlayerDisplay = new Rectangle(x, y, PlayerDisplay.Width, PlayerDisplay.Height);
        }

        public void SetSource(Rectangle newSource)
        {
            playerSource = newSource;
        }

        public Rectangle GetHitbox(int marginX, int marginTop, int marginBottom)
        {
            return new Rectangle(
                PlayerDisplay.X + marginX,
                PlayerDisplay.Y + marginTop,
                PlayerDisplay.Width - 2 * marginX,
                PlayerDisplay.Height - marginTop - marginBottom
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

        public void UpdateHitbox()
        {
            playerHitbox = new Rectangle(PlayerDisplay.X + 55, PlayerDisplay.Y + 15, (int)((float)PlayerDisplay.Width * 0.3), (int)((float)PlayerDisplay.Height * 0.8));
        }

        public void SetColor(Color color)
        {
            playerColor = color;
        }

    }
}