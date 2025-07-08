using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
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
        private Texture2D mainMenu;

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

        private Enemy[] enemies;

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

        private bool isHit = false;
        private double hitTimer = 0;
        private const double hitDuration = 0.5; // seconds

        private int coinScore = 0;

        // --- HIT COUNTER FOR RESET ---
        private int hitCount = 0;
        private const int maxHits = 3;


        private int tileWidth = 43;
        private int tileHeight = 43;

        // Snow particle system
        private Texture2D whitePixel;
        private Vector2[] snowflakePositions;
        private float[] snowflakeSpeeds;
        private const int SnowflakeCount = 100;

        private int enemyCount;

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

            action = "idle"; // No enemies in this level
            GameState = "menu";
            gameFrame = 0;

            checkSave = false;
            checkLoad = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load raw assets only
            backgroundTexture = Content.Load<Texture2D>("snowice");
            horizontalTileTexture = Content.Load<Texture2D>("Platform 2");
            verticalTileTexture = Content.Load<Texture2D>("Platform 5");
            playerTexture = Content.Load<Texture2D>("mika");
            playerHitboxTexture = Content.Load<Texture2D>("hitbox");
            spikeTexture = Content.Load<Texture2D>("spikes");
            coinTexture = Content.Load<Texture2D>("coin");
            backgroundmusic = Content.Load<Song>("PekoraPek");
            jumpSound = Content.Load<SoundEffect>("jump");
            runSound = Content.Load<SoundEffect>("run");
            mainMenu = Content.Load<Texture2D>("easy");


            whitePixel = new Texture2D(GraphicsDevice, 1, 1);
            whitePixel.SetData(new[] { Color.White });
            TextureWidth = playerTexture.Width / 8;
            TextureHeight = playerTexture.Height / 3;

            //// Call Level1 to set up all level objects
            //Level2();
            Level1();

            // Snow particle setup
            snowflakePositions = new Vector2[SnowflakeCount];
            snowflakeSpeeds = new float[SnowflakeCount];
            Random rand = new Random();
            for (int i = 0; i < SnowflakeCount; i++)
            {
                snowflakePositions[i] = new Vector2(rand.Next(0, _graphics.PreferredBackBufferWidth), rand.Next(0, _graphics.PreferredBackBufferHeight));
                snowflakeSpeeds[i] = (float)(rand.NextDouble() * 2 + 1);
            }

            // Player
           
            Rectangle playerSource = new Rectangle(0, 0, TextureWidth, TextureHeight);
            Rectangle playerHitbox = playerSource;
            player = new Player(playerTexture, new Rectangle(100, Window.ClientBounds.Height - (25 * 6), TextureWidth + 60, TextureHeight + 45), playerSource, Color.White, playerHitbox);



            // Music
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(backgroundmusic);
            MediaPlayer.Volume = 0.1f;
        }

        public void Level1()
        {
            // Clear all lists to avoid duplication if Level1 is called again
            tiles.Clear();
            verticalTiles.Clear();
            floorSpikes.Clear();
            ceilingSpikes.Clear();
            sideWallSpikes.Clear();
            oppsideWallSpikes.Clear();
            spinningCoins.Clear();

            // Background
            background = new Background(
                backgroundTexture,
                new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                Color.White
            );

            // Coins
            spinningCoins.Add(new Coin(coinTexture, new Vector2(360, 475), 32, 32));
            spinningCoins.Add(new Coin(coinTexture, new Vector2(600, 475), 32, 32));
            spinningCoins.Add(new Coin(coinTexture, new Vector2(1480, 300), 32, 32));
            spinningCoins.Add(new Coin(coinTexture, new Vector2(680, 725), 32, 32));
            spinningCoins.Add(new Coin(coinTexture, new Vector2(95, 300), 32, 32));



            // Tiles and platforms
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

            // Spikes
            Rectangle spikeFloorSource = new Rectangle(0, 0, 87, 87);

            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(602, 174, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(645, 174, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(688, 174, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(1161, 174, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(1204, 174, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(1247, 174, tileWidth, tileHeight), spikeFloorSource, Color.White));    
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(946, 820, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(1333, 820, tileWidth, tileHeight), spikeFloorSource, Color.White));

            Rectangle spikeCeilingSource = new Rectangle(2 * 87, 0, 87, 87);
            ceilingSpikes.Add(new Spike(spikeTexture, new Rectangle(818, 258, tileWidth, tileHeight), spikeCeilingSource, Color.White));
            ceilingSpikes.Add(new Spike(spikeTexture, new Rectangle(860, 258, tileWidth, tileHeight), spikeCeilingSource, Color.White));
            ceilingSpikes.Add(new Spike(spikeTexture, new Rectangle(902, 258, tileWidth, tileHeight), spikeCeilingSource, Color.White));
            
            ceilingSpikes.Add(new Spike(spikeTexture, new Rectangle(1118, 688, tileWidth, tileHeight), spikeCeilingSource, Color.White));

            Rectangle sideWallSpikeSource = new Rectangle(1 * 87, 0, 87, 87);
            sideWallSpikes.Add(new Spike(spikeTexture, new Rectangle(516, 385, tileWidth, tileHeight), sideWallSpikeSource, Color.White));
            sideWallSpikes.Add(new Spike(spikeTexture, new Rectangle(516, 430, tileWidth, tileHeight), sideWallSpikeSource, Color.White));
            sideWallSpikes.Add(new Spike(spikeTexture, new Rectangle(516, 475, tileWidth, tileHeight), sideWallSpikeSource, Color.White));

            Rectangle oppsideWallSpikeSource = new Rectangle(3 * 87, 0, 87, 87);
            oppsideWallSpikes.Add(new Spike(spikeTexture, new Rectangle(688, 430, tileWidth, tileHeight), oppsideWallSpikeSource, Color.White));
            

            enemyCount = 3;
            enemies = new Enemy[enemyCount];

            //Enemies
            for (int i = 0; i < enemyCount; i++)
            {

                enemies[i] = new Enemy(Content.Load<Texture2D>("mika"), new Rectangle(100, 100, TextureWidth + 60, TextureHeight + 45), new Rectangle(0, 0, TextureWidth, TextureHeight), Color.DarkRed, new Rectangle(0,0,0,0), 0, 0, 0);
                enemies[i].UpdateHitbox();
            }
            enemies[0].setPath(0 + (43 * 15), 0 + (43 * 20), Window.ClientBounds.Height - (43 * 3) - 10);
            enemies[1].setPath(0 + (43 * 12), 0 + (43 * 20), Window.ClientBounds.Height - (43 * 8) - 10);
            enemies[2].setPath(0 + (43 * 6), 0 + (43 * 8), Window.ClientBounds.Height - (43 * 8) - 10);
        }

        public void Level2()
        {
            tiles.Clear();
            verticalTiles.Clear();
            floorSpikes.Clear();
            ceilingSpikes.Clear();
            sideWallSpikes.Clear();
            oppsideWallSpikes.Clear();
            spinningCoins.Clear();

            background = new Background(
                backgroundTexture,
                new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                Color.White
            );

            // Coins
            spinningCoins.Add(new Coin(coinTexture, new Vector2(95, 300), 32, 32));
            spinningCoins.Add(new Coin(coinTexture, new Vector2(1490, 750), 32, 32));
            spinningCoins.Add(new Coin(coinTexture, new Vector2(700, 100), 32, 32));
            spinningCoins.Add(new Coin(coinTexture, new Vector2(600, 330), 32, 32));

            spinningCoins.Add(new Coin(coinTexture, new Vector2(1110, 750), 32, 32));




            // Tiles and platforms
            Rectangle horizontalSource = new Rectangle(3 * 128, 0, 128, 128);
            Rectangle verticalSource = new Rectangle(3 * 128, 0, 128, 128);

            for (int i = 0; i <= 40; i++) // floor tiles
            {
                Rectangle dest = new Rectangle(i * tileWidth, 860, tileWidth, tileHeight);
                if (i == 28)
                    tiles.Add(new GameTiles(verticalTileTexture, dest, verticalSource, Color.White));
                else
                    tiles.Add(new GameTiles(horizontalTileTexture, dest, horizontalSource, Color.White));
            }

            for (int i = 1; i <= 10; i++)
            {
                Rectangle dest = new Rectangle(28 * tileWidth, 860 - (i * tileHeight), tileWidth, tileHeight);
                if (i <= 5)
                {
                    verticalTiles.Add(new GameTiles(verticalTileTexture, dest, verticalSource, Color.White));
                }
            }

            GameTiles fifthVertical = verticalTiles[4];
            for (int i = 1; i <= 11; i++)
            {
                Rectangle bridgeDest = new Rectangle(fifthVertical.TileDisplay.X - (i * tileWidth), fifthVertical.TileDisplay.Y, tileWidth, tileHeight);

                tiles.Add(new GameTiles(verticalTileTexture, bridgeDest, verticalSource, Color.White));
            }
            for (int i = 19; i <= 22; i++)
            {
                Rectangle bridgeDest = new Rectangle(fifthVertical.TileDisplay.X - (i * tileWidth), fifthVertical.TileDisplay.Y, tileWidth, tileHeight);

                tiles.Add(new GameTiles(verticalTileTexture, bridgeDest, verticalSource, Color.White));
            }

            for (int i = 5; i <= 8; i++)
            {
                Rectangle bridgeDest = new Rectangle(fifthVertical.TileDisplay.X + (i * tileWidth), fifthVertical.TileDisplay.Y, tileWidth, tileHeight);

                tiles.Add(new GameTiles(verticalTileTexture, bridgeDest, verticalSource, Color.White));
            }

            for (int i = 5; i <= 15; i++)
            {
                Rectangle dest = new Rectangle(10 * tileWidth, 860 - (i * tileHeight), tileWidth, tileHeight);
                verticalTiles.Add(new GameTiles(verticalTileTexture, dest, verticalSource, Color.White));
            }

            for (int i = 5; i <= 15; i++)
            {
                Rectangle dest = new Rectangle(22 * tileWidth, 860 - (i * tileHeight), tileWidth, tileHeight);
                verticalTiles.Add(new GameTiles(verticalTileTexture, dest, verticalSource, Color.White));
            }

            GameTiles tenthVertical = verticalTiles[10];
            for (int i = 7; i <= 10; i++)
            {
                Rectangle bridgeDest = new Rectangle(tenthVertical.TileDisplay.X - (i * tileWidth), tenthVertical.TileDisplay.Y, tileWidth, tileHeight);
                if (i == 10)
                    tiles.Add(new GameTiles(tenthVertical.Texture, bridgeDest, tenthVertical.TileSource, tenthVertical.TileColor));
                else
                    tiles.Add(new GameTiles(verticalTileTexture, bridgeDest, verticalSource, Color.White));
            }
            for (int i = 1; i <= 8; i++)
            {
                Rectangle bridgeDest = new Rectangle(tenthVertical.TileDisplay.X + (i * tileWidth), tenthVertical.TileDisplay.Y, tileWidth, tileHeight);

                tiles.Add(new GameTiles(verticalTileTexture, bridgeDest, verticalSource, Color.White));
            }
            for (int i = 19; i <= 26; i++)
            {
                Rectangle bridgeDest = new Rectangle(tenthVertical.TileDisplay.X + (i * tileWidth), tenthVertical.TileDisplay.Y, tileWidth, tileHeight);

                tiles.Add(new GameTiles(verticalTileTexture, bridgeDest, verticalSource, Color.White));
            }
            GameTiles fifteenthVertical = verticalTiles[15];
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

            // Spikes
            Rectangle spikeFloorSource = new Rectangle(0, 0, 87, 87);

            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(86, 820, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(645, 820, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(302, 174, tileWidth, tileHeight), spikeFloorSource, Color.White));


            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(1247, 174, tileWidth, tileHeight), spikeFloorSource, Color.White));
            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(1030, 820, tileWidth, tileHeight), spikeFloorSource, Color.White));


            floorSpikes.Add(new Spike(spikeTexture, new Rectangle(1333, 820, tileWidth, tileHeight), spikeFloorSource, Color.White));



            Rectangle sideWallSpikeSource = new Rectangle(1 * 87, 0, 87, 87);
            sideWallSpikes.Add(new Spike(spikeTexture, new Rectangle(988, 385, tileWidth, tileHeight), sideWallSpikeSource, Color.White));
            sideWallSpikes.Add(new Spike(spikeTexture, new Rectangle(172, 430, tileWidth, tileHeight), sideWallSpikeSource, Color.White));
            sideWallSpikes.Add(new Spike(spikeTexture, new Rectangle(988, 475, tileWidth, tileHeight), sideWallSpikeSource, Color.White));

            Rectangle oppsideWallSpikeSource = new Rectangle(3 * 87, 0, 87, 87);
            oppsideWallSpikes.Add(new Spike(spikeTexture, new Rectangle(904, 430, tileWidth, tileHeight), oppsideWallSpikeSource, Color.White));

            enemyCount = 4;
            enemies = new Enemy[enemyCount];

            //Enemies
            for (int i = 0; i < enemyCount; i++)
            {

                enemies[i] = new Enemy(Content.Load<Texture2D>("mika"), new Rectangle(100, 100, TextureWidth + 60, TextureHeight + 45), new Rectangle(0, 0, TextureWidth, TextureHeight), Color.DarkRed, new Rectangle(0,0,0,0), 0, 0, 0);
                enemies[i].UpdateHitbox();
            }
            enemies[0].setPath(0 + (43 * 8), 0 + (43 * 25), Window.ClientBounds.Height - (43 * 18) - 10);
            enemies[1].setPath(0 + (43 * 12), 0 + (43 * 20), Window.ClientBounds.Height - (43 * 18) - 10);
            enemies[2].setPath(0 + (43 * 6), 0 + (43 * 8), Window.ClientBounds.Height - (43 * 8) - 10);
            enemies[3].setPath(0 + (43 * 12), 0 + (43 * 15), Window.ClientBounds.Height - (43 * 13) - 10);
        }

        protected override void Update(GameTime gameTime)
        {
            Color defaultPlayerColor = Color.White;

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

                if (state.IsKeyDown(Keys.F))
                {
                    if(player.Attackframes == -1)
                        player.IncrementFrameCounter();


                }

                if (player.Attackframes > -1)
                {
                    action = "attack";
                    player.IncrementFrameCounter();
                    
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

                bool spikeCollision = false;
                foreach (var spike in allSpikes)
                {
                    // Use Hitbox for spike collision
                    if (player.PlayerHitbox.Intersects(spike.Hitbox))
                    {


                        spikeCollision = true;
                        // Apply knockback for visual feedback
                        player.ChangeVelocityY(-5, true);
                        player.MoveHorizontal(10, direction * -1);
                        player.UpdateHitbox();
                        action = "hit";
                        

                    }

                    if (spikeCollision && !isHit)
                    {
                        isHit = true;
                        hitTimer = 0;
                        hitCount++; // Increment hit counter
                                    // --- RESET IF HIT 3 TIMES ---
                        if (hitCount >= maxHits)
                        {
                            action = "death";
                            player.SetPosition(100, Window.ClientBounds.Height - (25 * 6));
                            player.ChangeVelocityY(0, true);
                            player.UpdateHitbox();

                            hitCount = 0;
                            isHit = false;
                            hitTimer = 0;

                        }
                        break;
                        
                    }
                }

                foreach (var coin in spinningCoins)
                {
                    if (!coin.Collected && player.PlayerHitbox.Intersects(coin.GetCollisionBox()))
                    {
                        coin.Collect();
                        coinScore++;
                        // Optionally: play a sound effect here
                    }
                }

                foreach (var e in enemies){
                    if (player.PlayerDisplay.Intersects(e.EnemyHitbox) && player.Attackframes > 0)
                        e.Death();
                    else if (player.PlayerHitbox.Intersects(e.EnemyHitbox) && e.Alive && !isHit)
                    {
                            isHit = true;
                            hitTimer = 0;
                            hitCount++;

                            player.ChangeVelocityY(-4, true); // Apply knockback
                            player.MoveHorizontal(10, direction * -1); // Move player away from spike
                            player.UpdateHitbox();
                            action = "hit";

                            if (hitCount >= maxHits)
                            {
                            // Reset player position to starting point
                            action = "death";
                                player.SetPosition(100, Window.ClientBounds.Height - (25 * 6));
                                player.ChangeVelocityY(0, true);
                                player.UpdateHitbox();
                                // Reset hit state and counter
                                hitCount = 0;
                                isHit = false;
                                hitTimer = 0;
                                // Optionally: add a sound or visual cue here
                            }
                            break;
                        
                    }
                }

                // --- HIT ANIMATION LOGIC ---
                if (isHit)
                {
                    hitTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    action = "hit";
                    if (hitTimer >= hitDuration)
                    {
                        isHit = false;
                        hitTimer = 0;
                    }
                }


                player.playerAnimation(action, gameFrame);
            }

            foreach(Enemy e in enemies)
            {
                e.EnemyPathing();
                e.UpdateHitbox();
                e.enemyAnimation(e.EnemyAction, gameFrame);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.P) && !checkSave) // Save
            {
                GameData playerPos = new GameData();
                playerPos.playerPos = player.PlayerDisplay;

                XmlSerializer saveData = new XmlSerializer(typeof(GameData));

                StreamWriter sw = new StreamWriter("player_save.txt");
                saveData.Serialize(sw, playerPos);

                GameData[] enemyPos = new GameData[enemyCount];
                for (int i = 0; i < enemyCount; i++)
                {
                    enemyPos[i] = new GameData();
                    enemyPos[i].enemyPos = enemies[i].EnemyDisplay;
                }

                XmlSerializer saveDataArray = new XmlSerializer(typeof(GameData[]));

                StreamWriter sw1 = new StreamWriter("enemy_save.txt");
                saveDataArray.Serialize(sw1, enemyPos);

                sw.Close();
                sw1.Close();

                checkSave = true;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.P))
            {
                checkSave = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.L) && !checkLoad) // Load
            {
                XmlSerializer loadData = new XmlSerializer(typeof(GameData));
                StreamReader sr = new StreamReader("player_save.txt");
                XmlSerializer loadDataArray = new XmlSerializer(typeof(GameData[]));
                StreamReader sr1 = new StreamReader("enemy_save.txt");

                GameData playerPos = (GameData)loadData.Deserialize(sr);
                player.SetPosition(playerPos.playerPos.X, playerPos.playerPos.Y);
                Console.WriteLine(playerPos.playerPos.X);
                Console.WriteLine(playerPos.playerPos.Y);

                GameData[] enemyPos = (GameData[])loadDataArray.Deserialize(sr1);
                for (int i = 0; i < enemyCount; i++)
                {
                    enemies[i].SetPosition(enemyPos[i].enemyPos.X, enemyPos[i].enemyPos.Y);
                    
                }
                sr.Close();
                sr1.Close();

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
                _spriteBatch.Draw(mainMenu, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White);


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



                foreach (var enemy in enemies)
                {
                   if(enemy.Alive)
                    _spriteBatch.Draw(enemy.EnemyTexture, enemy.EnemyDisplay, enemy.EnemySource, enemy.EnemyColor, 0, Vector2.Zero, enemy.Dir < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                }


                // --- PLAYER FLASHING EFFECT ---
                bool drawPlayer = true; // Default to drawing player
                if (isHit) // if player is hit, apply flashing effect
                {
                    // Flashing: skip drawing player every other 0.1s interval
                    double flashInterval = 0.1;
                    int flashPhase = (int)(hitTimer / flashInterval) % 2;
                    drawPlayer = flashPhase == 0;
                }

                if (drawPlayer)
                {
                    _spriteBatch.Draw(
                        playerTexture,
                        player.PlayerDisplay,
                        player.PlayerSource,
                        player.PlayerColor,
                        0,
                        Vector2.Zero,
                        direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        0
                    );
                }

                

                foreach (var coin in spinningCoins)
                    coin.Draw(_spriteBatch);

                DrawHealthBar();

            }

            for (int i = 0; i < SnowflakeCount; i++)
            {
                Rectangle snowRect = new Rectangle((int)snowflakePositions[i].X, (int)snowflakePositions[i].Y, 3, 3);
                _spriteBatch.Draw(whitePixel, snowRect, Color.White);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawHealthBar()
        {
            // Health bar position and size
            int barWidth = 200;
            int barHeight = 25;
            int barX = 30;
            int barY = 30;

            // Calculate health ratio
            float healthRatio = (float)(maxHits - hitCount) / maxHits;
            int healthWidth = (int)(barWidth * healthRatio);

            // Draw background (gray)
            _spriteBatch.Draw(whitePixel, new Rectangle(barX, barY, barWidth, barHeight), Color.Gray * 0.5f);
            // Draw health (red)
            _spriteBatch.Draw(whitePixel, new Rectangle(barX, barY, healthWidth, barHeight), Color.Red);
            // Draw border (black)
            int borderThickness = 2;
            _spriteBatch.Draw(whitePixel, new Rectangle(barX - borderThickness, barY - borderThickness, barWidth + borderThickness * 2, borderThickness), Color.Black); // Top
            _spriteBatch.Draw(whitePixel, new Rectangle(barX - borderThickness, barY + barHeight, barWidth + borderThickness * 2, borderThickness), Color.Black); // Bottom
            _spriteBatch.Draw(whitePixel, new Rectangle(barX - borderThickness, barY, borderThickness, barHeight), Color.Black); // Left
            _spriteBatch.Draw(whitePixel, new Rectangle(barX + barWidth, barY, borderThickness, barHeight), Color.Black); // Right
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
        

       

    }
}
