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
using MirrorKnight.Game_Objects;

namespace MirrorKnight
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Texture2D placeHc, loading, pMB, pMBO, pMBF, pSB, pSO, pSF, mainScreen, menuImage, exitB, kk, startS;
        public static Texture2D enemyBullet, reflectedBullet, room;
        List<string> lines;
        string[,] tilesRead;
        public static SoundEffect mbeep, monsterRoar, swordSwing, doorFX, bulletShotgun, bulletReg, bossDoorLock;

        int mMTimer; //main menu scrolling
        Dictionary<String, Rectangle> tiles = new Dictionary<string, Rectangle>();
        Text text;
        Random rn;
        public static bool pauseMenu, pauseOptionsBool, mainMenuBool, soundFxTog, musicTog;
        Rectangle pauseOptionsButton, pauseMusicButton, pauseSfxButton, pauseExitButton, pauseMenuRect, mainMenuRect, mainMenuStart;

        Rectangle leftDoor, topDoor, rightDoor, bottomDoor;     //Contains hitboxes for the exits from rooms
        int doorWidth = 50, doorSize = 10;                      //Contains width and protrusion of the doors
        bool enteringRoom = false;                              //Prevents player from interacting with door immediately after entering a room
        int doorTimerMax = 60, doorTimer = 0;                   //Timer to control the enteringRoom boolean.
        Rectangle bounds = new Rectangle(0, 0, 0, 0);


        static int exitWidth = 50;
        Rectangle floorExit;
        bool playerCanExit = false;                            //Detects if player is allowed to go to next floor

        public static Dictionary<string, Dictionary<String, Texture2D>> sprites;
        KeyboardState oldKB;
        MouseState oldM;
        GamePadState oldGP;

        Rectangle[,] mainMenuKnights;

        bool usingController = false, usingKeyboard = true;

        Player p;

        Texture redBlockThing;
        public static Map map;

        public static int x, y;       //Contains the current room the player is in.
        
        public static List<Projectile> projectiles;                     //Contains list of all active projectiles
        public static List<LivingEntity> enemies;                       //Contains list of all living enemies in a room
        public static List<Entity> entities;                            //Contains list of all non-living entities in a room

        public static int tileSize = 60, verticalOffset;

        //Layer depths for everything. Depths range from 0 ~ 1, with lower numbers being further forward
        public static float INVISIBLE = 1.0f, TILE = 0.8f, PROJECTILE = 0.6f, ENTITY = 0.45f, SUB_UI_ELEMENT = 0.35f , UI_ELEMENT = 0.3f, MENU = 0.2f, MENU_BUTTONS = 0.1f, MINIMAP = 0.0f;

        Texture2D box;

        Sprite[] hearts;
        Sprite crosshair;

        Sprite menuBackground;

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

            menuBackground = new Sprite(0,0);
            // TODO: Add your initialization logic here
            //IsMouseVisible = true;
            mainMenuKnights = new Rectangle[Useful.getWWidth()/40, Useful.getWHeight()/40];

            for (int i = 0; i < (Useful.getWWidth() / 40); i++)
            {
                for (int j = 0; j < (Useful.getWHeight() / 40); j++)
                {
                    mainMenuKnights[i, j] = new Rectangle(40 * j, 40 * i, 16, 28);
                }
            }
            rn = new Random();
            mMTimer = 0;

            mainMenuBool = true;
            mainMenuRect = new Rectangle(Useful.getWWidth()/2 - 300, 100, 600, 100);
            mainMenuStart = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 -100), 400, 60);
            soundFxTog = true;
            musicTog = true;
            crosshair = new Sprite();
            crosshair.setUpdate(()=>crosshair.setPos(Mouse.GetState().X, Mouse.GetState().Y));
                        
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

            pauseMenuRect = new Rectangle(Useful.getWWidth()/2-Useful.getWWidth()/4, Useful.getWHeight() / 2 - Useful.getWHeight() / 4, Useful.getWWidth() / 2, Useful.getWHeight() / 2);
            pauseOptionsButton = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 - 50), 400, 60);
            pauseExitButton = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 + 50), 400, 60);

            pauseMusicButton = new Rectangle(Useful.getWWidth()/2 - 200, (Useful.getWHeight() / 2 )-150, 60, 60);
            pauseSfxButton = new Rectangle(Useful.getWWidth()/2 + 140, (Useful.getWHeight() / 2) - 150, 60, 60);


            //Creates the bounding boxes for the doors
            leftDoor = new Rectangle(24, Useful.getWHeight()/2 - doorWidth / 2, doorSize, doorWidth);
            rightDoor = new Rectangle(Useful.getWWidth() - doorSize, Useful.getWHeight() / 2 - doorWidth / 2, doorSize, doorWidth);

            floorExit = new Rectangle(Useful.getWWidth() / 2 - exitWidth / 2, Useful.getWHeight() / 2 - exitWidth / 2, exitWidth, exitWidth);


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

            menuBackground.addTexture(Useful.CreateRectangle(1, 1, Color.Black));
            menuBackground.setSize(Useful.getWWidth(), Useful.getWHeight());
            //menuBackground.setDepth(MENU_BUTTONS+.1f);
            menuBackground.setVisible(false);

            // TODO: use this.Content to load your game content here
            mbeep = Content.Load<SoundEffect>("MenuBeep"); //menu sfw
            monsterRoar = Content.Load<SoundEffect>("monsterRoar"); //sfw for bosses
            swordSwing = Content.Load<SoundEffect>("swordSwingSFX"); //sfw when player swings sword, regardless of it hitting anything
            doorFX = Content.Load<SoundEffect>("door_lock"); //door sfx
            bulletReg = Content.Load<SoundEffect>("proReg"); //sfx for regular bullet firing from enemies
            bulletShotgun = Content.Load<SoundEffect>("proShotgun"); //sfw for shotgun enemies
            bossDoorLock = Content.Load<SoundEffect>("jail_cell_door"); //sound played when boss door is opened, in addition to normal door sound


            placeHc = Content.Load<Texture2D>("pc");
            crosshair.addTexture("crosshair");
            crosshair.setSize(100, 100);
            crosshair.setDepth(MENU_BUTTONS);
            crosshair.centerOrigin();

            kk = Content.Load<Texture2D>("kk");
            pMBO = Content.Load<Texture2D>("mNoteOn"); //pause button music note on texture
            pMBF = Content.Load<Texture2D>("mNoteOff"); //pause button music note off texture
            pMB = pMBO;
            pSO = Content.Load<Texture2D>("sfxOn"); 
            pSF = Content.Load<Texture2D>("sfxOff"); 
            pSB = pSO;
            mainScreen = Content.Load<Texture2D>("Mirror-Knight");
            menuImage = Content.Load<Texture2D>("menu");
            exitB = Content.Load<Texture2D>("exitButton");
            startS = Useful.getTexture("start");

            //crossHair = Content.Load<Texture2D>("crosshair");
            enemyBullet = Content.Load<Texture2D>("enemyBullet");
            reflectedBullet = Content.Load<Texture2D>("playerBullet");

            room = Content.Load<Texture2D>("Star");
            //pauseMenuRect.addTexture(placeHc);
            //pauseMenuRect.setSize(Useful.getWWidth() / 2, Useful.getWHeight() / 2);
            //pauseMenuRect.depth = 100;
            Text.setDefaultFont("font");

            text.center();

            box = Useful.CreateRectangle(1,1,Color.White);

            loadTiles();

            p = new Player();

            p.body.setTimeFrame(1 / 8f);
            p.body.setPos(new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2));

            map = new Map(Map.Floor.GARDEN);

            x = map.GetDimensions().X / 2;
            y = map.GetDimensions().Y / 2;

            map.GetRoom(x, y).EnterRoom(Content, p);

            verticalOffset = (Useful.getWHeight() - map.GetRoom(x, y).Height * tileSize) / 2;

            topDoor = new Rectangle(Useful.getWWidth() / 2 - doorWidth / 2, verticalOffset, doorWidth, doorSize);
            bottomDoor = new Rectangle(Useful.getWWidth() / 2 - doorWidth / 2, Useful.getWHeight() - doorSize - verticalOffset, doorWidth, doorSize);

            // redBlockThing = Content.Load<Texture2D>("redBlock");
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





            /////////////////////////////////////////////Main menu logic
            if (mainMenuBool == true)
            {
                //mainMenuStart = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 - 200), 400, 100);
                mainMenuStart = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 - 50), 400, 60);
                pauseExitButton = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 + 50), 400, 60);
                mMTimer++;
                for (int i = 0; i < (Useful.getWWidth() / 40); i++)
                {
                    for (int j = 0; j < (Useful.getWHeight() / 40) ; j++)
                    {
                        //mainMenuKnights[i, j].X += 1;
                        mainMenuKnights[i, j].X += (rn.Next(10));
                        if (mainMenuKnights[i, j].X >= Useful.getWWidth())
                        {
                            mainMenuKnights[i, j].X -=  Useful.getWWidth();
                        }
                        
                    }
                }
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
                        if (soundFxTog == true)
                        {
                            mbeep.Play();
                        }

                        //mainMenuTransition(gameTime);
                    }
                    if (crosshair.getRectangle().Intersects(pauseMusicButton))
                    {

                        if (musicTog == true)
                        {
                            pMB = pMBF;
                            musicTog = false;
                        }
                        else if (musicTog == false)
                        {
                            pMB = pMBO;
                            musicTog = true;
                        }


                    }
                    if (crosshair.getRectangle().Intersects(pauseSfxButton))
                    {
                        if (soundFxTog == true)
                        {
                            pSB = pSF;
                            soundFxTog = false;
                        }
                        else if (soundFxTog == false)
                        {
                            pSB = pSO;
                            soundFxTog = true;
                        }
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
            ////////////////////////////////////////////End main menu logic
            if (mainMenuBool == false)
            {
                ///////////////////////////////////Pause menu logic
                if (pauseMenu == true)
                {
                        

                    if (m.LeftButton == ButtonState.Pressed)
                    {
                        if (crosshair.getRectangle().Intersects(pauseMusicButton))
                        {
                            if (musicTog == true)
                            {
                                pMB = pMBF;
                                musicTog = false;
                            }
                            else if (musicTog == false)
                            {
                                pMB = pMBO;
                                musicTog = true;
                            }
                        }
                        if (crosshair.getRectangle().Intersects(pauseSfxButton))
                        {
                            if (soundFxTog == true)
                            {
                                pSB = pSF;
                                soundFxTog = false;
                            }
                            else if (soundFxTog == false)
                            {
                                pSB = pSO;
                                soundFxTog = true;
                            }
                        }
                        if (crosshair.getRectangle().Intersects(pauseOptionsButton))
                        {
                            pauseOptionsBool = true;
                        }
                        if (crosshair.getRectangle().Intersects(pauseExitButton))
                        {
                            this.Exit();
                            if (soundFxTog == true)
                            {
                                mbeep.Play();
                            }
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
                        if (soundFxTog == true)
                        {
                            mbeep.Play();
                        }
                        map.HideMap();
                    }
                    /////////////////////////////////////////////////////////End pause menu logic
                }
                else if (pauseMenu == false)
                {
                ////////////////////////////////////////////////////////////////Begin normal game logic


                    ////////////////////////////////////////////////////////////////Player movement and aiming logic
                    

                    p.body.setVisible(true);
                    if (kb.IsKeyDown(Keys.Tab) && oldKB.IsKeyUp(Keys.Tab))
                    {
                        pauseMenu = true;
                        if (soundFxTog == true)
                        {
                            mbeep.Play();
                        }
                        map.DrawMap();

                    }
                    if (pauseMenu == true)
                    {

                        pauseMenuRect = new Rectangle(Useful.getWWidth() / 2 - Useful.getWWidth() / 4, Useful.getWHeight() / 2 - Useful.getWHeight() / 4, Useful.getWWidth() / 2, Useful.getWHeight() / 2);
                        pauseOptionsButton = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 - 50), 400, 60);
                        pauseExitButton = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2 + 50), 400, 60);

                        pauseMusicButton = new Rectangle(Useful.getWWidth() / 2 - 200, (Useful.getWHeight() / 2) - 150, 60, 60);
                        pauseSfxButton = new Rectangle(Useful.getWWidth() / 2 + 140, (Useful.getWHeight() / 2) - 150, 60, 60);

                    }

                    ///////////////////////////////////////////////////////////////////////////////Game Object update logic

                    for (int i = 0; i < entities.Count; i++)
                    {
                        entities[i].Update();


                        //////////////////////////////World item pickup logic
                        if (entities[i] is WorldItem)
                        {
                            if(p.Intersects(entities[i].GetRectangle()))
                            {
                                int id = ((WorldItem)entities[i]).getID();
                                if (((WorldItem)entities[i]).IsPassive())
                                {
                                    p.i.Add(new PassiveItem(id));
                                }
                                else p.i.Add(new ActiveItem(id), p);

                                if(map.GetRoom(x, y).getRoomType() == Room.Type.TREASURE)
                                {
                                    map.GetRoom(x, y).treasureTaken = true;
                                }
                            }
                        }
                    }
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].Update();

                        if (enemies[i].IsAlive() == false)
                        {
                            enemies[i].Kill();
                            enemies.Remove(enemies[i]);
                            monsterRoar.Play();

                            i--;
                        }
                    }

                    p.Update();

                    ///////////////////////////////////////////////////////////////////////////////Projectile logic

                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        projectiles[i].Update();
                    }

                    for (int i = projectiles.Count - 1; i >= 0; i--)
                    {
                        Vector2 pos = projectiles[i].body.getPos();
                        //Detects if projectile is going offscreen and if so, removes it
                        if (pos.X < 0 || pos.X > graphics.PreferredBackBufferWidth || pos.Y < 0 || pos.Y > graphics.PreferredBackBufferHeight)
                        {
                            projectiles[i].Remove();
                            projectiles.Remove(projectiles[i]);

                            i--;
                        }
                        else if (isTileShootableThrough(projectiles[i].body.getOriginPos()) == false)
                        {
                            projectiles[i].Remove();
                            projectiles.Remove(projectiles[i]);

                            i--;
                        }

                        else //Detects if projectile is currently hitting an enemy and if it is a reflected projectile.
                        {
                            for (int a = enemies.Count-1; a >= 0; a--)
                            {
                                if (projectiles[i].CanHurtEnemies() && enemies[a].body.intersects(projectiles[i].body))
                                {
                                    projectiles[i].Remove();
                                    projectiles.Remove(projectiles[i]);

                                    enemies[a].Hurt((int)p.GetDamage());
                                }
                            }
                        }
                    }

                    ////////////////////////////////////////////////////////Movement from room to room logic

                    //If player enters the hitbox for a door

                    bounds = p.getHitbox();

                    if (enteringRoom == false && (bounds.Intersects(leftDoor) || bounds.Intersects(rightDoor) || bounds.Intersects(topDoor) || bounds.Intersects(bottomDoor)))
                    {
                        if (enemies.Count == 0)  //And if all enemies are dead
                        {
                            //Checks each door hitbox, whether or not the room in that direction exists, and if the player is moving towards that door
                            //If so, moves the player to that room
                            Console.WriteLine(map.CheckRoom(x - 1, y));
                            Console.WriteLine(p.lastMove);
                            Console.WriteLine(bounds.Intersects(leftDoor));
                            if (bounds.Intersects(leftDoor) && map.CheckRoom(x - 1, y) && p.lastMove.X < 0)
                            {
                                x--;
                                p.body.setPos(rightDoor.X - rightDoor.Width * 2 - p.body.getWidth() / 2, rightDoor.Y + rightDoor.Height / 2 - p.body.getHeight() / 2);
                                enteringRoom = true;
                            }
                            else if (bounds.Intersects(rightDoor) && map.CheckRoom(x + 1, y) && p.lastMove.X > 0)
                            {
                                x++;
                                p.body.setPos(leftDoor.X + leftDoor.Width * 2 + p.body.getWidth() / 2, leftDoor.Y + leftDoor.Height / 2 - p.body.getHeight() / 2);
                                enteringRoom = true;

                            }
                            else if (bounds.Intersects(topDoor) && map.CheckRoom(x, y - 1) && p.lastMove.Y < 0)
                            {
                                y--;
                                p.body.setPos(bottomDoor.X + bottomDoor.Width / 2 - p.body.getWidth() / 2, bottomDoor.Y - bottomDoor.Height * 2 - p.body.getHeight() / 2);
                                enteringRoom = true;

                            }
                            else if (bounds.Intersects(bottomDoor) && map.CheckRoom(x, y + 1) && p.lastMove.Y > 0)
                            {
                                y++;
                                p.body.setPos(topDoor.X + topDoor.Width / 2 - p.body.getWidth() / 2, topDoor.Y + topDoor.Height * 2 + p.body.getHeight() / 2);
                                enteringRoom = true;
                            }
                            else Console.WriteLine("Room movement error");

                            if (enteringRoom)
                            {
                                map.EnterRoom(x, y, Content, p);

                                if(map.GetRoom(x, y).getRoomType() == Room.Type.BOSS)
                                {
                                    bossDoorLock.Play();
                                    monsterRoar.Play();
                                }
                                if (soundFxTog == true)
                                {
                                    doorFX.Play();
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        if (doorTimer >= doorTimerMax)
                        {
                            enteringRoom = false;
                            doorTimer = 0;
                        }
                        doorTimer++;
                    }

                    ////////////////////////////////////////////////////////Floor exit logic
                    if(map.GetRoom(x, y).getRoomType() == Room.Type.BOSS)
                    {
                        if(enemies.Count == 0)
                        {
                            if(p.Intersects(floorExit) && playerCanExit)
                            {
                                map = new Map(map.GetNextFloor());

                                x = map.GetDimensions().X / 2;
                                y = map.GetDimensions().Y / 2;

                                map.GetRoom(x, y).EnterRoom(Content, p);
                            }
                            else if(!p.Intersects(floorExit))
                            {
                                playerCanExit = true;
                            }
                        }
                    }

                    

                }
                ///////////////////////////////////////////End normal game logic

            }

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
                for (int i = 0; i < Useful.getWWidth() / 40 ; i++)
                {
                    for (int j = 0; j < Useful.getWHeight() / 40; j++)
                    {
                        spriteBatch.Draw(kk, mainMenuKnights[i, j], null, Color.White);//, 0, Vector2.Zero, SpriteEffects.None, MENU_BUTTONS);
                    }
                }
                spriteBatch.Draw(startS, mainMenuStart, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, MENU_BUTTONS);
                spriteBatch.Draw(pMB, pauseMusicButton, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, MENU_BUTTONS);
                spriteBatch.Draw(pSB, pauseSfxButton, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, MENU_BUTTONS);
                //spriteBatch.Draw(placeHc, pauseOptionsButton, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, MENU_BUTTONS);
                spriteBatch.Draw(exitB, pauseExitButton, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, MENU_BUTTONS);
                spriteBatch.Draw(mainScreen, mainMenuRect, null, Color.White);
                
            }
            else if (mainMenuBool == false)
            {
                map.GetRoom(x, y).Draw(spriteBatch, 0, (GraphicsDevice.Viewport.Height - map.GetRoom(x, y).Height * tileSize) / 2, tileSize);   //Draws room with offset x, y and tile size of tileSize
                spriteBatch.Draw(menuImage, pauseMenuRect, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, MENU);
                spriteBatch.Draw(pMB, pauseMusicButton, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, MENU_BUTTONS);
                spriteBatch.Draw(pSB, pauseSfxButton, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, MENU_BUTTONS);
                //spriteBatch.Draw(placeHc, pauseOptionsButton, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, MENU_BUTTONS);
                spriteBatch.Draw(exitB, pauseExitButton, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, MENU_BUTTONS);

                if(map.GetRoom(x, y).getRoomType() == Room.Type.BOSS && enemies.Count == 0)
                {
                    spriteBatch.Draw(placeHc, floorExit, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, PROJECTILE);
                }
            }
            // spriteBatch.Draw(placeHc, p.getHitbox(), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static Vector2 TileToPixelCoords(int x, int y)
        {
            x = tileSize * x + (tileSize / 2);
            y = (verticalOffset) + (y * tileSize) + (tileSize / 2);

            return new Vector2(x, y);
        }

        public static Vector2 PixelToTileCoords(Vector2 v)
        {
            int newX = (int)(v.X / tileSize);
            int newY = (int)((v.Y - verticalOffset) / tileSize);
            return new Vector2(newX, newY);
        }

        public static Vector2 PixelToTileCoords(double x, double y)
        {
            int newX = (int)(x / tileSize);
            int newY = (int)(y - verticalOffset) / tileSize;
            return new Vector2(newX, newY);
        }

        private bool isTileShootableThrough(Vector2 pos)
        {
            Vector2 tempPos = new Vector2(pos.X, pos.Y - (verticalOffset));

            if (tempPos.X < 0 || tempPos.Y < 0)
                return false;
            else if (tempPos.X > tileSize * 18 || tempPos.Y > tileSize * 10)
                return false;

            return map.GetRoom(x, y).isTileShootableThrough((int)tempPos.X / tileSize, (int)tempPos.Y / tileSize);
        }

        public static Room GetCurrentRoom()
        {
            return map.GetRoom(x, y);
        }

    }

}
