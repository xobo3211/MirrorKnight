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
        SpriteFont spectral18;
        Texture2D soulsPic, healthDisplayPic, placeHc;
        Player player;
        Dictionary<string, Dictionary<String, Texture2D>> sprites;

        KeyboardState oldKB;
        MouseState oldM;
        GamePadState oldGP;


        bool usingController = false, usingKeyboard = true;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Useful.set(this);
            this.Window.AllowUserResizing = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();
        }

        Regex nameReg = new Regex(@"^([a-z]+)");
        private void loadTiles()
        {
            DirectoryInfo d = new DirectoryInfo(@"Content/Tiles");
            FileInfo[] Files = d.GetFiles(); //Getting Text files

            foreach (FileInfo file in Files)
            {
                string op = nameReg.Match(file.Name).Groups[0].Value;
                if(sprites.ContainsKey())
                sprites.Add(file.Name, Useful.getTexture("));
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
            player = new Player();

            oldGP = GamePad.GetState(PlayerIndex.One);
            oldKB = Keyboard.GetState();
            oldM = Mouse.GetState();

            lines = new List<string>();
            tilesRead = new string[18, 10];

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
            //placeHc = Content.Load<Texture2D>("pc");

            loadTiles();
            //player.body.addTexture(tileSprite);
            player.body.setScale(10);
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

                playerAimVec = new Vector2(m.X - player.GetPosition().X, m.Y - player.GetPosition().Y);
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

            

            player.Move(playerMoveVec);

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
            //spriteBatch.DrawString(spectral18, "9999", new Vector2(10, 10), Color.White);
            //spriteBatch.Draw(placeHc, new Rectangle(150, 75, 1300, 650), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }



}
