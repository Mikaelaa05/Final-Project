using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Xml.Serialization;

namespace Final_Project
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private bool isFullScreen = false;

        private Background background;
        private Texture2D backgroundTexture;

        private Texture2D horizontalTileTexture;
        private Texture2D verticalTileTexture;

        private List<GameTiles> tiles = new List<GameTiles>();
        private List<GameTiles> verticalTiles = new List<GameTiles>();

        private Texture2D spikeTexture;
        private List<Spike> floorSpikes = new List<Spike>();
        private List<Spike> ceilingSpikes = new List<Spike>();
        private List<Spike> sideWallSpikes = new List<Spike>();
        private List<Spike> oppsideWallSpikes = new List<Spike>();

        private Player player;
        private Texture2D playerTexture;
        private Texture2D playerHitboxTexture;

        private int TextureWidth;
        private int TextureHeight;
        private string action;
        private int gameFrame;

        private Song backgroundmusic;
        private SoundEffect runSound;
        private SoundEffect jumpSound;

        private List<Coin> spinningCoins = new List<Coin>();
        private Texture2D coinTexture;

        private int direction;

        private int tileWidth = 43;
        private int tileHeight = 43;

        // Snow particle system
        private Texture2D whitePixel;
        private Vector2[] snowflakePositions;
        private float[] snowflakeSpeeds;
        private const int SnowflakeCount = 100;

        private string GameState;

        bool checkSave, checkLoad;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1590;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();

            action = "idle";
            GameState = "menu";
            gameFrame = 0;

            checkSave = false;
            checkLoad = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundTexture = Content.Load<Texture2D>("snowice");
            background = new Background(
                backgroundTexture,
                new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                Color.White
            );

            horizontalTileTexture = Content.Load<Texture2D>("Platform 2");
            verticalTileTexture = Content.Load<Texture2D>("Platform 5");

            Rectangle horizontalSource = new Rectangle(3 * 128, 0, 128, 128);
            Rectangle verticalSource = new Rectangle(3 * 128, 0, 128, 128);

            for (int i = 0; i <= 40; i++) // floor tiles
            {
                Rectangle dest = new Rectangle(i * tileWidth, 860, tileWidth, tileHeight);
                if (i == 11)
                    tiles.Add(new GameTiles(verticalTileTexture, dest, verticalSource, Color.White));
                else
                    tiles.Add(new GameTiles(horizontalTileTexture, dest, horizontalSource, Color.White));
            }

            for (int i = 1; i <= 15; i++)
            {
                Rectangle dest = new Rectangle(11 * tileWidth, 860 - (i * tileHeight), tileWidth, tileHeight);
                verticalTiles.Add(new GameTiles(verticalTileTexture, dest, verticalSource, Color.White));
            }

            GameTiles fifthVertical = verticalTiles[4];
            for (int i = 1; i <= 4; i++)
            {
                Rectangle bridgeDest = new Rectangle(fifthVertical.TileDisplay.X - (i * tileWidth), fifthVertical.TileDisplay.Y, tileWidth, tileHeight);
                tiles.Add(new GameTiles(verticalTileTexture, bridgeDest, verticalSource, Color.White));
            }

            for (int i = 1; i <= 20; i++)
            {
                Rectangle bridgeDest = new Rectangle(fifthVertical.TileDisplay.X + (i * tileWidth), fifthVertical.TileDisplay.Y, tileWidth, tileHeight);
                tiles.Add(new GameTiles(verticalTileTexture, bridgeDest, verticalSource, Color.White));
            }

            GameTiles tenthVertical = verticalTiles[9];
            for (int i = 7; i <= 11; i++)
            {
                Rectangle bridgeDest = new Rectangle(tenthVertical.TileDisplay.X - (i * tileWidth), tenthVertical.TileDisplay.Y, tileWidth, tileHeight);
                if (i == 10)
                    tiles.Add(new GameTiles(fifthVertical.Texture, bridgeDest, fifthVertical.TileSource, fifthVertical.TileColor));
                else
                    tiles.Add(new GameTiles(verticalTileTexture, bridgeDest, verticalSource, Color.White));
            }

            for (int i = 6; i <= 25; i++)
            {
                Rectangle bridgeDest = new Rectangle(tenthVertical.TileDisplay.X + (i * tileWidth), tenthVertical.TileDisplay.Y, tileWidth, tileHeight);
                tiles.Add(new GameTiles(verticalTileTexture, bridgeDest, verticalSource, Color.White));
            }

            GameTiles fifteenthVertical = verticalTiles[14];
            for (int i = 1; i <= 20; i++)
            {
                Rectangle bridgeDest = new Rectangle(fifteenthVertical.TileDisplay.X + (i * tileWidth), fifteenthVertical.TileDisplay.Y, tileWidth, tileHeight);
                tiles.Add(new GameTiles(verticalTileTexture, bridgeDest, verticalSource, Color.White));
            }

            for (int i = 1; i <= 4; i++)
            {
                Rectangle bridgeDest = new Rectangle(fifteenthVertical.TileDisplay.X - (i * tileWidth), fifteenthVertical.TileDisplay.Y, tileWidth, tileHeight);
                tiles.Add(new GameTiles(verticalTileTexture, bridgeDest, verticalSource, Color.White));
            }

            spikeTexture = Content.Load<Texture2D>("spikes");
            Rectangle spikeFloorSource = new Rectangle(0, 0, 87, 87);

            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(602, 174, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(645, 174, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(688, 174, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(1161, 174, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(1204, 174, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(1247, 174, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(1030, 820, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(1074, 820, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(1290, 820, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(1333, 820, tileWidth, tileHeight), spikeFloorSource, Color.White));

            Rectangle spikeCeilingSource = new Rectangle(2 * 87, 0, 87, 87);
            ceilingSpikes.Add(new Spike(spikeTexture, new Rectangle(818, 258, tileWidth, tileHeight), spikeCeilingSource, Color.White));
            ceilingSpikes.Add(new Spike(spikeTexture, new Rectangle(860, 258, tileWidth, tileHeight), spikeCeilingSource, Color.White));
            ceilingSpikes.Add(new Spike(spikeTexture, new Rectangle(902, 258, tileWidth, tileHeight), spikeCeilingSource, Color.White));
            ceilingSpikes.Add(new Spike(spikeTexture, new Rectangle(902, 688, tileWidth, tileHeight), spikeCeilingSource, Color.White));
            ceilingSpikes.Add(new Spike(spikeTexture, new Rectangle(946, 688, tileWidth, tileHeight), spikeCeilingSource, Color.White));
            ceilingSpikes.Add(new Spike(spikeTexture, new Rectangle(1160, 688, tileWidth, tileHeight), spikeCeilingSource, Color.White));
            ceilingSpikes.Add(new Spike(spikeTexture, new Rectangle(1204, 688, tileWidth, tileHeight), spikeCeilingSource, Color.White));

            Rectangle sideWallSpikeSource = new Rectangle(1 * 87, 0, 87, 87);
            sideWallSpikes.Add(new Spike(spikeTexture, new Rectangle(516, 385, tileWidth, tileHeight), sideWallSpikeSource, Color.White));
            sideWallSpikes.Add(new Spike(spikeTexture, new Rectangle(516, 430, tileWidth, tileHeight), sideWallSpikeSource, Color.White));
            sideWallSpikes.Add(new Spike(spikeTexture, new Rectangle(516, 475, tileWidth, tileHeight), sideWallSpikeSource, Color.White));

            Rectangle oppsideWallSpikeSource = new Rectangle(3 * 87, 0, 87, 87);
            oppsideWallSpikes.Add(new Spike(spikeTexture, new Rectangle(688, 430, tileWidth, tileHeight), oppsideWallSpikeSource, Color.White));

            // Snow particle setup
            whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            whitePixel.SetData(new[] { Color.White });

            snowflakePositions = new Vector2[SnowflakeCount];
            snowflakeSpeeds = new float[SnowflakeCount];
            Random rand = new Random();
            for (int i = 0; i < SnowflakeCount; i++)
            {
                snowflakePositions[i] = new Vector2(rand.Next(0, _graphics.PreferredBackBufferWidth), rand.Next(0, _graphics.PreferredBackBufferHeight));
                snowflakeSpeeds[i] = (float)(rand.NextDouble() * 2 + 1);
            }

            coinTexture = Content.Load<Texture2D>("coin");
            spinningCoins.Add(new Coin(coinTexture, new Vector2(360, 475), 64, 64));
            spinningCoins.Add(new Coin(coinTexture, new Vector2(600, 475), 64, 64));
            spinningCoins.Add(new Coin(coinTexture, new Vector2(1480, 300), 64, 64));
            spinningCoins.Add(new Coin(coinTexture, new Vector2(800, 725), 64, 64));
            spinningCoins.Add(new Coin(coinTexture, new Vector2(95, 300), 64, 64));

            playerTexture = Content.Load<Texture2D>("mika");
            playerHitboxTexture = Content.Load<Texture2D>("hitbox");
            TextureWidth = playerTexture.Width / 8;
            TextureHeight = playerTexture.Height / 2;
            Rectangle playerSource = new Rectangle(0, 0, TextureWidth, TextureHeight);
            Rectangle playerHitbox = playerSource; // Initialize hitbox with the same size as the source rectangle

            player = new Player(playerTexture, new Rectangle(100, Window.ClientBounds.Height - (25 * 6), TextureWidth + 60, TextureHeight + 45), playerSource, Color.White, playerHitbox);

            backgroundmusic = Content.Load<Song>("PekoraPek");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundmusic);
            MediaPlayer.Volume = 0.1f; // Set volume to 50%

          jumpSound = Content.Load<SoundEffect>("jump"); // Load jump sound effect
          runSound = Content.Load<SoundEffect>("run"); // Load walk sound effect


        }

        protected override void Update(GameTime gameTime)
        {
            if (GameState == "menu")
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    GameState = "play";
                }
            }
            else if (GameState == "play")
            {
                gameFrame++;
                if (gameFrame >= 999) gameFrame = 0;

                KeyboardState state = Keyboard.GetState();


                Rectangle originalplayer = player.PlayerDisplay;

                if (state.IsKeyDown(Keys.Escape)) Exit();

                if (state.IsKeyDown(Keys.F11) && !isFullScreen)
                {
                    _graphics.IsFullScreen = true;
                    _graphics.ApplyChanges();
                    isFullScreen = true;
                }
                else if (state.IsKeyDown(Keys.F11) && isFullScreen)
                {
                    _graphics.IsFullScreen = false;
                    _graphics.ApplyChanges();
                    isFullScreen = false;
                }

                

                foreach (var coin in spinningCoins)
                    coin.Update(gameTime);

                List<Spike> allSpikes = new List<Spike>();
                allSpikes.AddRange(floorSpikes);
                allSpikes.AddRange(ceilingSpikes);
                allSpikes.AddRange(sideWallSpikes);
                allSpikes.AddRange(oppsideWallSpikes);



                spinningCoins.RemoveAll(c => c.Collected);



                // Movement keys
                float moveSpeed = 4f;
                float jumpStrength = -10f;


                player.UpdateHitbox();

                // Move Left
                if (state.IsKeyDown(Keys.A))
                {
                    direction = -1;
                    PlayerMove((int)moveSpeed, direction);
                    action = "running";
                    if (gameFrame % 25 == 0 && IsOnGround())
                        // If jumping, play jump sound
                        runSound.Play();

                }
                else if (state.IsKeyDown(Keys.D))
                {
                    direction = 1;
                    PlayerMove((int)moveSpeed, direction);
                    action = "running";
                    if (gameFrame % 25 == 0 && IsOnGround())
                        runSound.Play();
                }
                else if (IsOnGround())
                {
                    action = "idle"; // If not moving, set action to idle
                }

                // Jump (only if grounded)
                if (state.IsKeyDown(Keys.Space) && IsOnGround())
                {
                    player.ChangeVelocityY(jumpStrength, true);
                    jumpSound.Play();
                }



                if (player.VelocityY > 2) //sets action to falling if velocity is going up
                {
                    action = "falling";
                }
                else if (player.VelocityY < -2) //sets action to jump if velolocity is going down.
                {
                    action = "jump";
                }



                // Stop falling below ground (simple ground collision)
                if (!IsOnGround() || player.VelocityY < 0)
                {
                    // Apply gravity
                    player.ChangeVelocityY(0.2f); // Gravity strength
                    player.MoveVertical((int)player.VelocityY, 1); // Move player vertically based on velocity
                    player.UpdateHitbox();

                    if (IsColliding(player.PlayerHitbox))
                    {
                        player.MoveVertical((int)player.VelocityY, -1);
                        player.ChangeVelocityY(1, true);
                    }


                }
                else
                {

                    while (IsOnGround())
                    {

                        player.MoveVertical(1, -1); // Move player up until not colliding with ground
                        player.UpdateHitbox();
                    }
                    player.ChangeVelocityY(0, true); // Reset vertical velocity
                                                     //if (action != "running")
                                                     //    action = "idle";
                }

                for (int i = 0; i < SnowflakeCount; i++)
                {
                    snowflakePositions[i].Y += snowflakeSpeeds[i];
                    if (snowflakePositions[i].Y > _graphics.PreferredBackBufferHeight)
                    {
                        snowflakePositions[i].Y = -5;
                        snowflakePositions[i].X = new Random().Next(0, _graphics.PreferredBackBufferWidth);
                    }
                }



                playerAnimation(action, gameFrame);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P) && !checkSave) // Save
            {
                GameData playerPos = new GameData();
                playerPos.playerPos = player.PlayerDisplay;

                //GameData[] enemyPos = new GameData[enemyLength];
                //for (int i = 0; i < enemyLength; i++)
                //{
                //    enemyPos[i] = new GameData();
                //    enemyPos[i].enemyPos = enemies[i].EnemyDisplay;
                //}

                XmlSerializer saveData = new XmlSerializer(typeof(GameData));

                StreamWriter sw = new StreamWriter("game_save.txt");
                saveData.Serialize(sw, playerPos);
                //saveData.Serialize(sw, enemyPos);

                sw.Close();

                checkSave = true;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.P))
            {
                checkSave = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.L) && !checkLoad) // Load
            {
                XmlSerializer loadData = new XmlSerializer(typeof(GameData));
                StreamReader sr = new StreamReader("game_save.txt");

                GameData playerPos = (GameData)loadData.Deserialize(sr);
                player.SetPosition(playerPos.playerPos.X, playerPos.playerPos.Y);
                Console.WriteLine(playerPos.playerPos.X);
                Console.WriteLine(playerPos.playerPos.Y);

                //GameData[] enemyPos = (GameData[])loadData.Deserialize(sr);
                //for (int i = 0; i < enemyLength; i++)
                //{
                //    enemies[i].SetPosition(enemyPos[i].enemyPos.X, enemyPos[i].enemyPos.Y);
                //    Console.WriteLine(enemyPos[i].enemyPos.X);
                //    Console.WriteLine(enemyPos[i].enemyPos.Y);
                //}
                sr.Close();

                player.ChangeVelocityY(0, true); // Reset vertical velocity after loading

                checkLoad = true;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.L))
            {
                checkLoad = false;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();

            _spriteBatch.Draw(background.BackgroundTexture, background.BackgroundRectangle, background.BackgroundColor);

            if (GameState == "menu")
            {
                _spriteBatch.DrawString(Content.Load<SpriteFont>("MenuFont"), "Mika is Peak!!!", new Vector2(25, 25), Color.Magenta);
                
               
            }
            else if (GameState == "play")
            {

                foreach (var tile in tiles)
                    _spriteBatch.Draw(tile.Texture, tile.TileDisplay, tile.TileSource, tile.TileColor);

                foreach (var t in verticalTiles)
                    _spriteBatch.Draw(t.Texture, t.TileDisplay, t.TileSource, t.TileColor);

                foreach (var s in floorSpikes)
                    _spriteBatch.Draw(s.Texture, s.Display, s.Source, s.Color);

                foreach (var s in ceilingSpikes)
                    _spriteBatch.Draw(s.Texture, s.Display, s.Source, s.Color);

                foreach (var s in sideWallSpikes)
                    _spriteBatch.Draw(s.Texture, s.Display, s.Source, s.Color);

                foreach (var s in oppsideWallSpikes)
                    _spriteBatch.Draw(s.Texture, s.Display, s.Source, s.Color);


                _spriteBatch.Draw(playerTexture, player.PlayerDisplay, player.PlayerSource, player.PlayerColor, 0, Vector2.Zero, direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0); // for player
                _spriteBatch.Draw(playerHitboxTexture, player.PlayerHitbox, player.HitboxSource, Color.Red * 0.5f); // for player hitbox

                foreach (var coin in spinningCoins)
                    coin.Draw(_spriteBatch);

            }

            for (int i = 0; i < SnowflakeCount; i++)
            {
                Rectangle snowRect = new Rectangle((int)snowflakePositions[i].X, (int)snowflakePositions[i].Y, 3, 3);
                _spriteBatch.Draw(whitePixel, snowRect, Color.White);
            }

  

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private bool IsColliding(Rectangle playerRect)
        {
            player.UpdateHitbox();
            Rectangle hitbox = player.PlayerHitbox;

            foreach (var tile in tiles)
            {
                if (tile != null && hitbox.Intersects(tile.TileDisplay))
                {
                    return true;
                }
            }

            foreach (var tile in verticalTiles)
            {
                if (tile != null && hitbox.Intersects(tile.TileDisplay))
                {
                    return true;
                }
            }
            return false;
        }


        private bool IsOnGround()
        {
            player.UpdateHitbox();
            Rectangle hitbox = player.PlayerHitbox;
            Rectangle onePixelLower = new Rectangle(hitbox.X, hitbox.Y + 1, hitbox.Width, hitbox.Height);
            foreach (var tile in tiles)
            {
                if (onePixelLower.Y <= tile.TileDisplay.Y - tile.Texture.Height / 2)
                {
                    if (tile != null && onePixelLower.Intersects(tile.TileDisplay))
                    {
                        return true;
                    }
                }
            }

            foreach (var tile in verticalTiles)
            {
                if (tile != null && onePixelLower.Intersects(tile.TileDisplay))
                {
                    return true;
                }
            }
            return false;
        }

        private void PlayerMove(int steps, int dir)
        {
            player.MoveHorizontal(steps, dir);
            player.UpdateHitbox();
            if (IsColliding(player.PlayerHitbox))
            {
                player.MoveHorizontal(-steps, dir); 

            }
        }
        

        private void playerAnimation(string action, int curframe)
        {
            
            if (action == "idle")
            {
                player.PlayerAnimator(curframe, 0, 3);
            }
            else if (action == "running")
            {
                player.PlayerAnimator(curframe, 4, 7);
            }
            else if (action == "jump")
            {
                player.PlayerAnimator(curframe, 8, 8);
            }
            else if (action == "falling")
            {
                player.PlayerAnimator(curframe, 9, 9);
            }
            else
            {
                player.PlayerAnimator(curframe, 0, 3); // Default to idle if action is unknown

            }
        }

    }
}
