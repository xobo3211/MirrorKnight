using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SureDroid;
using System.IO;
using System.Text.RegularExpressions;

namespace MirrorKnight
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D placeHc, loading, pMB, pMBO, pMBF;
        public static Texture2D enemyBullet, reflectedBullet;
        List<string> lines;
        string[,] tilesRead;
        Dictionary<String, Rectangle> tiles = new Dictionary<string, Rectangle>();
        Text text;

        bool pauseMenu, pauseOptionsBool, mainMenuBool;
        Rectangle pauseOptionsButton, pauseMusicButton, pauseSfxButton, pauseExitButton, pauseMenuRect, mainMenuRect, mainMenuStart;

        public static Dictionary<string, Dictionary<String, Texture2D>> sprites;
        KeyboardState oldKB;
        MouseState oldM;
        GamePadState oldGP;


        bool usingController = false, usingKeyboard = true;

        public static Player p;
        Map m;

        int x, y;       //Contains the current room the player is in.
        
        public static List<Projectile> projectiles;                     //Contains list of all active projectiles
        public static List<LivingEntity> enemies;                       //Contains list of all living enemies in a room
        public static List<Entity> entities;                            //Contains list of all non-living entities in a room

        static int tileSize = 60, verticalOffset = 200;

        Sprite[] hearts;
        Sprite crossheir;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Useful.set(this);
            this.Window.AllowUserResizing = false;
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();
        }




        /// <summary>
        /// Classifies it by the first underscore name, and then into a dictionary with all names.
        /// </summary>
        Regex nameReg = new Regex(@"^([a-z]+)");
        private void loadTiles()
        {
            DirectoryInfo d = new DirectoryInfo(@"Content/Textures");
            FileInfo[] Files = d.GetFiles(); //Getting Text files
            sprites = new Dictionary<string, Dictionary<string, Texture2D>>();
            foreach (FileInfo file in Files)
            {
                string op = nameReg.Match(file.Name).Groups[0].Value;
                string name = file.Name.Substring(0, file.Name.Length - 4);
                if(!sprites.ContainsKey(op))
                    sprites.Add(op, new Dictionary<string, Texture2D>());
                sprites[op].Add(name, Useful.getTexture(@"textures\" + name));
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //IsMouseVisible = true;

            mainMenuBool = true;
            mainMenuRect = new Rectangle(0, 0, Useful.getWWidth(), Useful.getWHeight());
            mainMenuStart = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 -100), 400, 60);

            crossheir = new Sprite();

            //Sprite[] hearts = new Sprite[];
            //for (int i = 0; 0 < 3; i++)
            //{
            //    hearts[i] = new Sprite(1000 - (50 * i), 100);
            //    hearts[i].addTexture("textures/ui_heart_full");
            //}
            p = new Player();
            p.body.setUpdate((sprite)=>
            {
                Console.WriteLine(sprite.getPos());
            });
            oldGP = GamePad.GetState(PlayerIndex.One);
            oldKB = Keyboard.GetState();
            oldM = Mouse.GetState();
            lines = new List<string>();
            projectiles = new List<Projectile>();
            enemies = new List<LivingEntity>();
            entities = new List<Entity>();

            tilesRead = new string[18, 10];

            text = new Text("Testing Boxy");
            text.visable = false;

            //ReadFileAsStrings("presetRooms/testroom.txt");

            pauseMenuRect = new Rectangle(Useful.getWWidth()/2-Useful.getWWidth()/4, Useful.getWHeight() / 2 - Useful.getWHeight() / 4, Useful.getWWidth() / 2, Useful.getWHeight() / 2);
            pauseOptionsButton = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 - 50), 400, 60);
            pauseExitButton = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 + 50), 400, 60);

            pauseMusicButton = new Rectangle(Useful.getWWidth()/2 - 200, (Useful.getWHeight() / 2 )-150, 60, 60);
            pauseSfxButton = new Rectangle(Useful.getWWidth()/2 + 140, (Useful.getWHeight() / 2) - 150, 60, 60);

            base.Initialize(); 
        } 

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            placeHc = Content.Load<Texture2D>("pc");
            crossheir.addTexture("crosshair");
            crossheir.setSize(100, 100);
            crossheir.centerOrigin();
            pMBO = Content.Load<Texture2D>("mNoteOn");
            pMBF = Content.Load<Texture2D>("mNoteOn (1)");
            pMB = pMBF;
            //crossHair = Content.Load<Texture2D>("crosshair");
            enemyBullet = Content.Load<Texture2D>("enemyBullet");
            reflectedBullet = Content.Load<Texture2D>("playerBullet");
            //pauseMenuRect.addTexture(placeHc);
            //pauseMenuRect.setSize(Useful.getWWidth() / 2, Useful.getWHeight() / 2);
            //pauseMenuRect.depth = 100;
            Text.setDefaultFont("font");

            text.center();

            loadTiles();
            p.load();
            p.body.setScale(3);
            p.body.setTimeFrame(1 / 8f);
            p.body.setPos(new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2));

            m = new Map(Map.Floor.GARDEN);

            x = m.GetDimensions().X / 2;
            y = m.GetDimensions().Y / 2;

            m.GetRoom(x, y).EnterRoom(Content, p);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        //private GameTime mainMenuTransition(GameTime gameTime)
        //{

        //}
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here

            KeyboardState kb = Keyboard.GetState();
            MouseState m = Mouse.GetState();
            GamePadState gp = GamePad.GetState(PlayerIndex.One);
            if (mainMenuBool == true)
            {
                mainMenuStart = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 - 200), 400, 100);
                pauseOptionsButton = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 - 50), 400, 60);
                pauseExitButton = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 + 50), 400, 60);

                //pauseMusicButton = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2) - 150, 60, 60);
                //pauseSfxButton = new Rectangle(Useful.getWWidth() / 2 + 140, (Useful.getWHeight() / 2) - 150, 60, 60);
                if (m.LeftButton == ButtonState.Pressed)
                {
                    if (crossheir.getRectangle().Intersects(mainMenuStart))
                    {
                        mainMenuBool = false;
                        pauseMenuRect = new Rectangle();
                        pauseOptionsButton = new Rectangle();
                        pauseExitButton = new Rectangle();
                        mainMenuStart = new Rectangle();
                        mainMenuRect = new Rectangle();
                        pauseMusicButton = new Rectangle();
                        pauseSfxButton = new Rectangle();
                        //mainMenuTransition(gameTime);
                    }
                    if (crossheir.getRectangle().Intersects(pauseMusicButton))
                    {
                        
                    }
                    if (crossheir.getRectangle().Intersects(pauseSfxButton))
                    {

                    }
                    if (crossheir.getRectangle().Intersects(pauseOptionsButton))
                    {
                        pauseOptionsBool = true;
                    }
                    if (crossheir.getRectangle().Intersects(pauseExitButton))
                    {
                        this.Exit();
                    }
                }

            }
            if (mainMenuBool == false)
            {
                if (pauseMenu == true)
                {
                    if (m.LeftButton == ButtonState.Pressed)
                    {
                        if (crossheir.getRectangle().Intersects(pauseMusicButton))
                        {

                        }
                        if (crossheir.getRectangle().Intersects(pauseSfxButton))
                        {

                        }
                        if (crossheir.getRectangle().Intersects(pauseOptionsButton))
                        {
                            pauseOptionsBool = true;
                        }
                        if (crossheir.getRectangle().Intersects(pauseExitButton))
                        {
                            this.Exit();
                        }
                    }

                    if (kb.IsKeyDown(Keys.Tab) && oldKB.IsKeyUp(Keys.Tab))
                    {
                        pauseMenu = false;
                        pauseMenuRect = new Rectangle();
                        pauseOptionsButton = new Rectangle();
                        pauseExitButton = new Rectangle();

                        pauseMusicButton = new Rectangle();
                        pauseSfxButton = new Rectangle();
                    }
                }
                else if (pauseMenu == false)
                {
                    ////////////////////////////////////////////////////////////////Player movement and aiming logic

                    Vector2 playerMoveVec = Vector2.Zero;
                    Vector2 playerAimVec = Vector2.Zero;


                    if (kb.IsKeyDown(Keys.Tab) && oldKB.IsKeyUp(Keys.Tab))
                    {
                        pauseMenu = true;

                    }
                    if (pauseMenu == true)
                    {

                        pauseMenuRect = new Rectangle(Useful.getWWidth() / 2 - Useful.getWWidth() / 4, Useful.getWHeight() / 2 - Useful.getWHeight() / 4, Useful.getWWidth() / 2, Useful.getWHeight() / 2);
                        pauseOptionsButton = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 - 50), 400, 60);
                        pauseExitButton = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 + 50), 400, 60);

                        pauseMusicButton = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2) - 150, 60, 60);
                        pauseSfxButton = new Rectangle(Useful.getWWidth() / 2 + 140, (Useful.getWHeight() / 2) - 150, 60, 60);

                    }



                    if (usingKeyboard)
                    {
                        if (gp.ThumbSticks.Left != Vector2.Zero || gp.ThumbSticks.Right != Vector2.Zero)
                        {
                            usingKeyboard = false;
                            usingController = true;
                        }
                        if (kb.IsKeyDown(Keys.W) && p.body.getY() > 100)
                        {
                            playerMoveVec.Y = -1;
                        }
                        else if (kb.IsKeyDown(Keys.S) && p.body.getY() < Useful.getWHeight()-100-p.body.getHeight())
                        {
                            playerMoveVec.Y = 1;
                        }
                        if (kb.IsKeyDown(Keys.A))
                        {
                            playerMoveVec.X = -1;
                        }
                        else if (kb.IsKeyDown(Keys.D))
                        {
                            playerMoveVec.X = 1;
                        }

                        playerAimVec = new Vector2(m.X, m.Y) - p.body.getOriginPos();

                        if (m.LeftButton == ButtonState.Pressed && oldM.LeftButton == ButtonState.Released)
                        {
                            p.Attack(playerAimVec);
                        }

                    }
                    else if (usingController)
                    {
                        if (Keyboard.GetState().GetPressedKeys().Length > 0 || oldM.X != m.X)
                        {
                            usingController = false;
                            usingKeyboard = true;
                        }
                        if (gp.ThumbSticks.Left != Vector2.Zero)
                        {
                            playerMoveVec = gp.ThumbSticks.Left;
                        }
                        if (gp.ThumbSticks.Right != Vector2.Zero)
                        {
                            playerAimVec = gp.ThumbSticks.Right;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: No controller-type set");
                    }

                    if (playerMoveVec != Vector2.Zero)
                    {
                        playerMoveVec.Normalize();
                        p.body.translate(playerMoveVec * p.GetSpeed());
                    }

                    ///////////////////////////////////////////////////////////////////////////////Game Object update logic

                    for (int i = 0; i < entities.Count; i++)
                    {
                        entities[i].Update();
                    }

                    ///////////////////////////////////////////////////////////////////////////////Projectile logic

                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        projectiles[i].Update();
                    }

                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        Vector2 pos = projectiles[i].body.getPos();
                        if (pos.X < 0 || pos.X > graphics.PreferredBackBufferWidth || pos.Y < 0 || pos.Y > graphics.PreferredBackBufferHeight)
                        {
                            projectiles[i].Dispose();
                            projectiles.Remove(projectiles[i]);
                        }
                        else
                        {
                            for (int a = 0; a < enemies.Count; a++)
                            {
                                if (projectiles[i].CanHurtEnemies() && enemies[a].body.intersects(projectiles[i].body))
                                {
                                    projectiles[i].Dispose();
                                    projectiles.Remove(projectiles[i]);
                                    enemies.Remove(enemies[a]);
                                }
                            }
                        }
                    }
                }

            }


            crossheir.setPos(m.X, m.Y);

            oldGP = gp;
            oldKB = kb;
            oldM = m;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, null, null);
            if (mainMenuBool == true)
            {
                spriteBatch.Draw(placeHc, mainMenuRect, Color.White);
                spriteBatch.Draw(placeHc, mainMenuStart, Color.White);
                spriteBatch.Draw(pMB, pauseMusicButton, Color.White);
                spriteBatch.Draw(placeHc, pauseSfxButton, Color.White);
                spriteBatch.Draw(placeHc, pauseOptionsButton, Color.White);
                spriteBatch.Draw(placeHc, pauseExitButton, Color.White);

            }
            else if (mainMenuBool == false)
            {
                m.GetRoom(x, y).Draw(spriteBatch, 0, (GraphicsDevice.Viewport.Height - m.GetRoom(x, y).Height * tileSize) / 2, tileSize);   //Draws room with offset x, y and tile size of tileSize
                spriteBatch.Draw(placeHc, pauseMenuRect, Color.White);
                spriteBatch.Draw(pMB, pauseMusicButton, Color.White);
                spriteBatch.Draw(placeHc, pauseSfxButton, Color.White);
                spriteBatch.Draw(placeHc, pauseOptionsButton, Color.White);
                spriteBatch.Draw(placeHc, pauseExitButton, Color.White);


                for (int i = 0; i < p.getHP(); i++)
                {
                    //hearts[i] = new Sprite((1000- 100*i) + (50 * i), 100);
                }

            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static Vector2 TileToPixelCoords(int x, int y)
        {
            x = tileSize * x;
            y = verticalOffset + (y * tileSize);

            return new Vector2(x, y);
        }

    }

}
