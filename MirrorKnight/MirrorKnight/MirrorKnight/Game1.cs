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

        List<Dictionary<String, Rectangle>> tiles;
        Texture2D tileSprite;


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
            String[] lines = Useful.readFileLines(@"Content\listv2.txt");
            tiles = new List<Dictionary<string, Rectangle>>();

            Dictionary<string, Rectangle> current = new Dictionary<string, Rectangle>();

            for (int i = 0; i < lines.Length; i++)
            {
                if(lines[i].Trim().Length == 0)
                {
                    tiles.Add(current);
                    current = new Dictionary<string, Rectangle>();
                }
                else
                {
                    string[] split = Useful.readWords(lines[i]);
                    int x = 0, y = 0, width = 0, height = 0;
                    string name = split[0];
                    try
                    {
                        x = Convert.ToInt32(split[1]);
                        y = Convert.ToInt32(split[2]);
                        width = Convert.ToInt32(split[3]);
                        height = Convert.ToInt32(split[4]);
                    }
                    catch (FormatException fe)
                    {
                        Console.WriteLine("Unable to convert to string. \nCause: " + fe.Message + "\nSource: " + fe.Source);
                    }
                    current.Add(name, new Rectangle(x, y, width, height));
                }
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

            tileSprite = Useful.getTexture("tileset2");
            loadTiles();
            player.body.addTexture(tileSprite);
            player.body.useRegion(true);
            player.body.defRegion(tiles[0].Values.ToArray()[0]);
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


    public class Player
    {
        public Sprite body;
        public static int health = 100, speed = 4, damage = 10;
        public Player()
        {
            body = new Sprite();
        }

    }
}
