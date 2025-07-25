﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        int attackframes;


        public Player(
            Texture2D playerTexture,
            Rectangle playerDisplay,
            Rectangle playerSource,
            Color playerColor, Rectangle playerHitbox)
        {
            this.playerTexture = playerTexture;
            this.playerDisplay = playerDisplay;
            this.playerSource = playerSource;
            this.playerColor = playerColor;
            this.playerHitbox = playerHitbox;
            hitboxSource = new Rectangle(0, 0, 32, 32);
            attackframes = -1;
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

        public Rectangle PlayerHitbox
        {
            get => playerHitbox;
        }

        public Rectangle HitboxSource
        {
            get => hitboxSource;
        }

        public int Attackframes
        {
            get => attackframes;
        }
        public float VelocityY { get => velocityY ;  }

        public void IncrementFrameCounter()
        {
            attackframes++;
            if(attackframes >= 30)
            {
                attackframes = -1; // Reset after 6 frames
            }
        }
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

        public void PlayerAnimator(int currentFrame, int startFrame, int endFrame = -1)
        {
            if (endFrame == -1)
            {
                endFrame = startFrame;
            }

            int animationLength = endFrame - startFrame + 1;
            int textureX = playerTexture.Width / 8;
            int textureY = playerTexture.Height / 4;
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

        public void playerAnimation(string action, int curframe)
        {

            if (action == "idle")
            {
                PlayerAnimator(curframe, 0, 3);
            }
            else if (action == "sprint")
            {
                PlayerAnimator(curframe, 24, 27);
            }
            else if (action == "running")
            {
                PlayerAnimator(curframe, 4, 7);
            }
            else if (action == "jump")
            {
                PlayerAnimator(curframe, 8);
            }
            else if (action == "hit")
            {
                PlayerAnimator(curframe, 16, 19);
            }
            else if (action == "death")
            {
                PlayerAnimator(curframe, 20, 23);
            }
            else if (action == "falling")
            {
                PlayerAnimator(curframe, 9);
            }
            else if (action == "attack") {

                PlayerAnimator(curframe, 10 + (int)attackframes / 5);
            } else
            {
                PlayerAnimator(curframe, 0, 3); // Default to idle if action is unknown

            }
        }

        public void UpdateHitbox()
        {
            playerHitbox = new Rectangle (playerDisplay.X + 55, playerDisplay.Y + 15, (int)((float)playerDisplay.Width * 0.3), (int)((float)playerDisplay.Height * 0.8));
        }

    }
}