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

        Rectangle leftDoor, topDoor, rightDoor, bottomDoor;     //Contains hitboxes for the exits from rooms
        int doorSize = 50;                                      //Contains width of the doors
        bool enteringRoom = false;                              //Prevents player from interacting with door immediately after entering a room
        int doorTimerMax = 60, doorTimer = 0;                   //Timer to control the enteringRoom boolean.


        public static Dictionary<string, Dictionary<String, Texture2D>> sprites;
        KeyboardState oldKB;
        MouseState oldM;
        GamePadState oldGP;


        bool usingController = false, usingKeyboard = true;

        Player p;
        Map map;

        int x, y;       //Contains the current room the player is in.
        
        public static List<Projectile> projectiles;                     //Contains list of all active projectiles
        public static List<LivingEntity> enemies;                       //Contains list of all living enemies in a room
        public static List<Entity> entities;                            //Contains list of all non-living entities in a room

        static int tileSize = 60, verticalOffset = 200;

        Sprite[] hearts;
        Sprite crosshair;

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

            crosshair = new Sprite();

            //Sprite[] hearts = new Sprite[];
            //for (int i = 0; 0 < 3; i++)
            //{
            //    hearts[i] = new Sprite(1000 - (50 * i), 100);
            //    hearts[i].addTexture("textures/ui_heart_full");
            //}
            p = new Player();
            
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


            //Creates the bounding boxes for the doors
            leftDoor = new Rectangle(0, Useful.getWHeight()/2 - doorSize / 2, 10, doorSize);
            rightDoor = new Rectangle(Useful.getWWidth() - 10, Useful.getWHeight() / 2 - doorSize / 2, 10, doorSize);

            topDoor = new Rectangle(Useful.getWWidth()/2 - doorSize / 2, verticalOffset/2, doorSize, 10);
            bottomDoor = new Rectangle(Useful.getWWidth() / 2 - doorSize / 2, Useful.getWHeight() - 10 - verticalOffset/2, doorSize, 10);


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
            crosshair.addTexture("crosshair");
            crosshair.setSize(100, 100);
            crosshair.centerOrigin();
            pMBO = Content.Load<Texture2D>("mNoteOn"); //pause button music note on texture
            pMBF = Content.Load<Texture2D>("mNoteOn (1)"); //pause button music note off texture
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

            map = new Map(Map.Floor.GARDEN);

            x = map.GetDimensions().X / 2;
            y = map.GetDimensions().Y / 2;

            map.GetRoom(x, y).EnterRoom(Content, p);
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
                    if (crosshair.getRectangle().Intersects(mainMenuStart))
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
                    if (crosshair.getRectangle().Intersects(pauseMusicButton))
                    {
                        
                    }
                    if (crosshair.getRectangle().Intersects(pauseSfxButton))
                    {

                    }
                    if (crosshair.getRectangle().Intersects(pauseOptionsButton))
                    {
                        pauseOptionsBool = true;
                    }
                    if (crosshair.getRectangle().Intersects(pauseExitButton))
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
                        if (crosshair.getRectangle().Intersects(pauseMusicButton))
                        {

                        }
                        if (crosshair.getRectangle().Intersects(pauseSfxButton))
                        {

                        }
                        if (crosshair.getRectangle().Intersects(pauseOptionsButton))
                        {
                            pauseOptionsBool = true;
                        }
                        if (crosshair.getRectangle().Intersects(pauseExitButton))
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
                            playerMoveVec.Y *= -1;
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
                        playerMoveVec *= p.GetSpeed();

                        if(checkNewPos(p, playerMoveVec))
                            p.body.translate(playerMoveVec);
                    }

                    ///////////////////////////////////////////////////////////////////////////////Game Object update logic

                    for (int i = 0; i < entities.Count; i++)
                    {
                        entities[i].Update();
                    }
                    for(int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].Update();
                    }

                    ///////////////////////////////////////////////////////////////////////////////Projectile logic

                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        projectiles[i].Update();
                    }

                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        Vector2 pos = projectiles[i].body.getPos();
                        //Detects if projectile is going offscreen and if so, removes it
                        if (pos.X < 0 || pos.X > graphics.PreferredBackBufferWidth || pos.Y < 0 || pos.Y > graphics.PreferredBackBufferHeight)
                        {
                            projectiles[i].Dispose();
                            projectiles.Remove(projectiles[i]);
                        }
                        else if (isTileShootableThrough(projectiles[i].body.getOriginPos()) == false)
                        {
                            projectiles[i].Dispose();
                            projectiles.Remove(projectiles[i]);
                        }

                        else //Detects if projectile is currently hitting an enemy and if it is a reflected projectile.
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

                    //Movement from room to room logic

                    //If player enters the hitbox for a door
                    if (enteringRoom == false && (p.Intersects(leftDoor) || p.Intersects(rightDoor) || p.Intersects(topDoor) || p.Intersects(bottomDoor)))
                    {
                        if (enemies.Count == 0)  //And if all enemies are dead
                        {
                            //Checks each door hitbox, whether or not the room in that direction exists, and if the player is moving towards that door
                            //If so, moves the player to that room
                            if (p.Intersects(leftDoor) && map.CheckRoom(x - 1, y) && playerMoveVec.X < 0)  
                            {
                                x--;
                                p.body.setPos(rightDoor.X - rightDoor.Width * 2 - p.body.getWidth() / 2, rightDoor.Y + rightDoor.Height / 2 - p.body.getHeight() / 2);
                            }
                            else if (p.Intersects(rightDoor) && map.CheckRoom(x + 1, y) && playerMoveVec.X > 0)
                            {
                                x++;
                                p.body.setPos(leftDoor.X + leftDoor.Width * 2 + p.body.getWidth() / 2, leftDoor.Y + leftDoor.Height / 2 - p.body.getHeight() / 2);
                                
                            }
                            else if (p.Intersects(topDoor) && map.CheckRoom(x, y - 1) && playerMoveVec.Y < 0)
                            {
                                y--;
                                p.body.setPos(bottomDoor.X + bottomDoor.Width / 2 - p.body.getWidth() / 2, bottomDoor.Y - bottomDoor.Height * 2 - p.body.getHeight() / 2);

                            }
                            else if (p.Intersects(bottomDoor) && map.CheckRoom(x, y + 1) && playerMoveVec.Y > 0)
                            {
                                y++;
                                p.body.setPos(topDoor.X + topDoor.Width / 2 - p.body.getWidth() / 2, topDoor.Y + topDoor.Height * 2 + p.body.getHeight() / 2);
                            }
                            else Console.WriteLine("Room movement error");

                            map.GetRoom(x, y).EnterRoom(Content, p);
                            enteringRoom = true;
                        }
                    }
                    else
                    {
                        if(doorTimer >= doorTimerMax)
                        {
                            enteringRoom = false;
                            doorTimer = 0;
                        }
                        doorTimer++;
                    }

                }

            }


            crosshair.setPos(m.X-24, m.Y-24);

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
                map.GetRoom(x, y).Draw(spriteBatch, 0, (GraphicsDevice.Viewport.Height - map.GetRoom(x, y).Height * tileSize) / 2, tileSize);   //Draws room with offset x, y and tile size of tileSize
                spriteBatch.Draw(placeHc, pauseMenuRect, Color.White);
                spriteBatch.Draw(pMB, pauseMusicButton, Color.White);
                spriteBatch.Draw(placeHc, pauseSfxButton, Color.White);
                spriteBatch.Draw(placeHc, pauseOptionsButton, Color.White);
                spriteBatch.Draw(placeHc, pauseExitButton, Color.White);


                for (int i = 0; i < p.GetHP(); i++)
                {
                    //hearts[i] = new Sprite((1000- 100*i) + (50 * i), 100);
                }

            }
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static Vector2 TileToPixelCoords(int x, int y)
        {
            x = tileSize * x + (tileSize / 2);
            y = (verticalOffset / 2) + (y * tileSize) + (tileSize / 2);

            return new Vector2(x, y);
        }

        private bool isTileShootableThrough(Vector2 pos)
        {
            Vector2 tempPos = new Vector2(pos.X, pos.Y - (verticalOffset / 2));

            if (tempPos.X < 0 || tempPos.Y < 0)
                return false;
            else if (tempPos.X > tileSize * 18 || tempPos.Y > tileSize * 10)
                return false;

            return map.GetRoom(x, y).isTileShootableThrough((int)tempPos.X / tileSize, (int)tempPos.Y / tileSize);
        }

        private bool isTileWalkableThrough(Vector2 pos)
        {
            Vector2 tempPos = new Vector2(pos.X, pos.Y - (verticalOffset / 2));

            if (tempPos.X < 0 || tempPos.Y < 0)
                return false;
            else if (tempPos.X > tileSize * 18 || tempPos.Y > tileSize * 10)
                return false;

            return map.GetRoom(x, y).isTileWalkableThrough((int)tempPos.X / tileSize, (int)tempPos.Y / tileSize);
        }

        /// <summary>
        /// Work in progress. Finish this later
        /// </summary>
        /// <param name="e"></param>
        /// <param name="moveVector"></param>
        /// <returns></returns>

        public bool checkNewPos(Entity e, Vector2 moveVector)       //Checks whether player can move in moveVector direction
        {
            return isTileWalkableThrough(e.body.getOriginPos() + moveVector);
        }
    }

}
