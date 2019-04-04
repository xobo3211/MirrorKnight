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
        List<string> lines;
        List<Room> psRoomsNormal, psRoomsRreasure, psRoomsShop, psRoomsBoss, psRoomsSecret, psRoomsPuzzle;
        string[,] tilesRead;
        Dictionary<String, Rectangle> tiles = new Dictionary<string, Rectangle>();


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

        private void loadTiles()
        {
            ReadFileAsStrings(@"Content/presetRooms/testroom.txt");
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
            placeHc = Content.Load<Texture2D>("pc");
            
            player.body.addTexture("");
        }

        private void ReadFileAsStrings(string path)
        {

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    for (int j = 0; !reader.EndOfStream; j++)
                    {
                        string line = reader.ReadLine();
                        string[] parts = line.Split(' ');
                        for (int i = 0; i < 240; i++)
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

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
