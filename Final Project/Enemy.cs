using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Net.Http;

namespace Final_Project
{
    internal class Enemy
    {
        Texture2D enemyTexture;
        Rectangle enemyDisplay;
        Rectangle enemySource;
        Rectangle enemyHitbox;
        Rectangle hitboxSource;
        Color enemyColor;
        float velocityY;
        string enemyAction;

        int startpos;
        int endpos;
        int ylevel;
        int dir;

        bool alive;

        public Enemy(
            Texture2D enemyTexture,
            Rectangle enemyDisplay,
            Rectangle enemySource,
            Color enemyColor, Rectangle enemyHitbox, int startpos, int endpos, int ylevel)
        {
            this.enemyTexture = enemyTexture;
            this.enemyDisplay = enemyDisplay;
            this.enemySource = enemySource;
            this.enemyColor = enemyColor;
            this.enemyHitbox = enemyHitbox;
            hitboxSource = new Rectangle(0, 0, 32, 32);
            this.startpos = startpos;
            this.endpos = endpos;
            this.ylevel = ylevel;

            dir = 1;
            enemyAction = "idle";
            alive = true;
        }

        public Texture2D EnemyTexture
        {
            get => enemyTexture;
        }
        public Rectangle EnemyDisplay
        {
            get => enemyDisplay; 
        }
        public Rectangle EnemySource
        {
            get => enemySource;
        }
        public Color EnemyColor
        {
            get => enemyColor;
        }

        public Rectangle EnemyHitbox
        {
            get => enemyHitbox;
        }

        public Rectangle HitboxSource
        {
            get => hitboxSource;
        }
        public float VelocityY { get => velocityY ;  }

        public int Dir        
        {
            get => dir;
        }

        public string EnemyAction
        {
            get => enemyAction;
        }

        public void MoveVertical(int steps, int dir)
        {
            enemyDisplay.Y += steps * dir;
        }

        public void MoveHorizontal(int steps, int dir)
        {
            enemyDisplay.X += steps * dir;
        }

        public void TeleportY(int dest)
        {
            enemyDisplay.Y = dest;
        }

        public void SetPosition(int x, int y)
        {
            enemyDisplay.X = x;
            enemyDisplay.Y = y;
        }

        public void SetSource(Rectangle newSource)
        {
            enemySource = newSource;
        }

      
        public Rectangle GetHitbox(int marginX, int marginTop, int marginBottom)
        { 
            return new Rectangle(
                enemyDisplay.X + marginX,
                enemyDisplay.Y + marginTop,
                enemyDisplay.Width - 2 * marginX,
                enemyDisplay.Height - marginTop - marginBottom
            );
        }

        public bool Alive
        {
            get => alive;
        }

        public void EnemyAnimator(int currentFrame, int startFrame, int endFrame)
        {
            int animationLength = endFrame - startFrame + 1;
            int textureX = enemyTexture.Width / 8;
            int textureY = enemyTexture.Height / 3;
            currentFrame = (currentFrame / 6) % animationLength;

            int frameX = (startFrame + currentFrame) % 8;
            int frameY = (startFrame + currentFrame) / 8;

            enemySource = new Rectangle(frameX * textureX, frameY * textureY, textureX, textureY);
        }


        public void setPath (int startpos, int endpos, int ylevel)
        {
            this.startpos = startpos;
            this.endpos = endpos;
            this.ylevel = ylevel;
            SetPosition(startpos, ylevel);
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

        public void EnemyPathing() 
        {
            MoveHorizontal(1, dir);
            enemyAction = "running";

            if(dir == 1 && enemyDisplay.X >= endpos)
            {
                dir = -1;

            } else if (dir == -1 && enemyDisplay.X <= startpos) 
            {
                dir = 1;
            }
        }

        public void Death()
        {
            alive = false;
            enemyAction = "dead";
            enemyDisplay.X = -1000; // Move off-screen
            enemyDisplay.Y = -1000; // Move off-screen
        }
        public void enemyAnimation(string action, int curframe)
        {

            if (action == "idle")
            {
                EnemyAnimator(curframe, 0, 3);
            }
            else if (action == "running")
            {
                EnemyAnimator(curframe, 4, 7);
            }
            else if (action == "jump")
            {
                EnemyAnimator(curframe, 8, 8);
            }
            else if (action == "falling")
            {
                EnemyAnimator(curframe, 9, 9);
            }
            else
            {
                EnemyAnimator(curframe, 0, 3); // Default to idle if action is unknown

            }
        }

        public void UpdateHitbox()
        {
            enemyHitbox = new Rectangle (enemyDisplay.X + 55, enemyDisplay.Y + 15, (int)((float)enemyDisplay.Width * 0.3), (int)((float)enemyDisplay.Height * 0.8));
        }

    }
}