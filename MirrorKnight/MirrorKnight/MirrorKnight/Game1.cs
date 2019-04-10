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
        Texture2D placeHc;
        List<string> lines;
        string[,] tilesRead;
        Dictionary<String, Rectangle> tiles = new Dictionary<string, Rectangle>();
        bool pauseMenu;
        public static Dictionary<string, Dictionary<String, Texture2D>> sprites;

        bool pauseMenu, pauseOptionsBool;
        Rectangle pauseOptionsButton, pauseMusicButton, pauseSfxButton, pauseExitButton;

        Dictionary<string, Dictionary<String, Texture2D>> sprites;
        KeyboardState oldKB;
        MouseState oldM;
        GamePadState oldGP;


        bool usingController = false, usingKeyboard = true;

        Player p;
        Map m;

        int x, y;       //Contains the current room the player is in.



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
            p = new Player();
            oldGP = GamePad.GetState(PlayerIndex.One);
            oldKB = Keyboard.GetState();
            oldM = Mouse.GetState();

            lines = new List<string>();
            tilesRead = new string[18, 10];

            ReadFileAsStrings("presetRooms/testroom.txt");

            m = new Map();

            x = m.GetDimensions().X / 2;
            y = m.GetDimensions().Y / 2;
            
            base.Initialize();
            pauseOptionsButton = new Rectangle();
            pauseExitButton = new Rectangle();
            pauseMusicButton = new Rectangle();
            pauseSfxButton = new Rectangle();
            
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

            loadTiles();
            p.load();
            //player.body.addTexture(tileSprite);
            p.body.setScale(3);
            p.body.setTimeFrame(1 / 16f);
            m.SetRoom(new MirrorKnight.Room(Room.Type.NORMAL, tilesRead, placeHc), m.GetDimensions().X / 2, m.GetDimensions().Y / 2);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

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

            Vector2 playerMoveVec = Vector2.Zero;
            Vector2 playerAimVec = Vector2.Zero;


            if (kb.IsKeyDown(Keys.Tab) && !oldKB.IsKeyDown(Keys.Tab) && pauseMenu == false)
            {
                pauseMenu = true;
            }
            else if (kb.IsKeyDown(Keys.Tab) && !oldKB.IsKeyDown(Keys.Tab) && pauseMenu == true)
            {
                pauseMenu = false;
            }

            if (pauseMenu == true)
            {
                
            }
            else
            {

            }
            
            if (usingKeyboard)
            {
                if(gp.ThumbSticks.Left != Vector2.Zero || gp.ThumbSticks.Right != Vector2.Zero)
                {
                    usingKeyboard = false;
                    usingController = true;
                }
                if (kb.IsKeyDown(Keys.W))
                {
                    playerMoveVec.Y = -1;
                }
                else if (kb.IsKeyDown(Keys.S))
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

                playerAimVec = new Vector2(m.X - p.body.getX(), m.Y - p.body.getY());
            }
            if(usingController)
            {
                if(Keyboard.GetState().GetPressedKeys().Length > 0 || oldM.X != m.X)
                {
                    usingController = false;
                    usingKeyboard = true;
                }
                if (gp.ThumbSticks.Left != Vector2.Zero)
                {
                    playerMoveVec = gp.ThumbSticks.Left;
                }
                if(gp.ThumbSticks.Right != Vector2.Zero)
                {
                    playerAimVec = gp.ThumbSticks.Right; 
                }
            }
            playerMoveVec.Normalize();

            //p.body.translate(playerMoveVec);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            m.GetRoom(x, y).Draw(spriteBatch, 0, (GraphicsDevice.Viewport.Height - m.GetRoom(x, y).Height * tileSize) / 2, tileSize);   //Draws room with offset x, y and tile size of tileSize

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void ReadFileAsStrings(string path)
        {

            try
            {
                using (StreamReader reader = new StreamReader("../../../../MirrorKnightContent/" + path))
                {
                    for (int j = 0; !reader.EndOfStream; j++)
                    {
                        string line = reader.ReadLine();
                        string[] parts = line.Split(' ');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            string c = parts[i];
                            tilesRead[i, j] = c;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
    }



}
