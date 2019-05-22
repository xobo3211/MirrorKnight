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
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SureDroid
{
    /// <summary>
    /// Custom Sprite Class - Created By Rithvik Senthilkumar | 
    /// Allows for easy handling of teextures, sprites, and animations. | 
    /// If you are not Rithvik and do not have permission from him, please dispose of this file immediately.
    /// </summary>
    public class Sprite
    {
        // -----------------------------------------------------------------
        // -------------------------- Variables ----------------------------
        // -----------------------------------------------------------------
        private static List<Sprite> sprites = new List<Sprite>();
        private static Dictionary<String,List<Sprite>> groups = new Dictionary<String,List<Sprite>>(); 

        private Rectangle rectangle;
        private Vector2 pos, origin;
        private List<Texture2D> textures = new List<Texture2D>();
        private List<Size> sizes = new List<Size>();
        private List<List<Rectangle>> regions = new List<List<Rectangle>>();
        private int index = 0, regionIndex = 0;
        private float timer = 0, rotation = 0;
        private bool visible = true, animation = false, regionUse = false;
        private double scale = 1.0;
        private float timeframe = 1 / 3f;
        private SpriteEffects effect = SpriteEffects.None;
        private Color color = Color.White;
        private float depth = 1;

        public delegate void CustomUpdate();
        private CustomUpdate customUpdateMethod;

        // -------------------------------------------------------------------
        // ------------------------- Constructors ----------------------------
        // -------------------------------------------------------------------

        /// <summary>
        /// Initializes the sprite to a provided x and y location.
        /// </summary>
        public Sprite(float x, float y)
        {
            if (Useful.game == null) throw new NullReferenceException("Please use Useful.set(Game game) before you use any class.");
            rectangle = new Rectangle();
            pos = new Vector2(x, y);
            origin = new Vector2(0, 0);
            updatePos();
            sprites.Add(this);
        }

        /// <summary>
        /// Initializes the sprite to a default (0, 0) position.
        /// </summary>
        public Sprite() : this(0, 0) { }

        /// <summary>
        /// Initialize the sprite class using with a provided position.
        /// </summary>
        /// <param name="pos">Position for the sprite in a vector2 format</param>
        public Sprite(Vector2 pos) : this(pos.X, pos.Y) { }

        // -----------------------------------------------------------------
        // ---------------------- GET/SET CODE -----------------------------
        // -----------------------------------------------------------------

        /// <summary>
        /// Get the width of the sprite on the current index.
        /// </summary>
        public int getWidth()
        {
            if (sizes.Count == 0) throw new NullReferenceException("The size for this sprite has not been yet determined. Please use this method after loading a texture. The index of this sprite is " + sprites.IndexOf(this) + ".");
            if (!regionUse)
                return (int)((Double)sizes[index].width * scale);
            else return (int)((Double)getRegion().Width * scale);
        }

        /// <summary>
        /// Get the height of the sprite on the current index.
        /// </summary>
        public int getHeight()
        {
            if (sizes.Count == 0) throw new NullReferenceException("The size for this sprite has not been yet determined. Please use this method after loading a texture. The index of this sprite is " + sprites.IndexOf(this) + ".");
            if (!regionUse)
                return (int)((Double)sizes[index].height * scale);
            else return (int)((Double)getRegion().Height * scale);
        }

        /// <summary>
        /// Returns if the sprite has any textures loaded. (Useful for debugging.)
        /// </summary>
        public bool hasTextures()
        {
            return (sizes.Count > 0);
        }

        /// <summary>
        /// Removes the sprite from being drawn.
        /// </summary>
        public void deleteThis()
        {
            sprites.Remove(this);
        }

        public void addThis()
        {
            sprites.Add(this);
        }

        public void dispose()
        {
            deleteThis();
            textures.ForEach(texture => texture.Dispose());
        }

        /// <summary>
        /// Set the effect for a sprite.
        /// </summary>
        /// <param name="newEffect">An effect from the SpriteEffects class.</param>
        public void setEffect(SpriteEffects newEffect)
        {
            effect = newEffect;
        }

        /// <summary>
        /// Set the color of the sprite.
        /// </summary>
        /// <param name="color">A color from the color class.</param>
        public void setColor(Color color)
        {
            this.color = color;
        }

        /// <summary>
        /// Return if a sprites intersects with another sprite.
        /// </summary>
        /// <param name="other">The other sprite you are comparing for intersection with.</param>
        /// <returns></returns>
        public Boolean intersects(Sprite other)
        {
            return getRectangle().Intersects(other.getRectangle());
        }

        /// <summary>
        /// Gets the rectangle for the current sprite. No changes can be made.
        /// </summary>
        /// <returns>A rectangle, containing size and position.</returns>
        public Rectangle getRectangle()
        {
            updatePos();
            return rectangle;
        }

        public Rectangle getBounds()
        {
            return new Rectangle((int)getOriginPos().X - getWidth()/2, (int)getOriginPos().Y - getHeight()/2, getWidth(), getHeight());
        }

        /// <summary>
        /// Sets the order of this sprite to move behind or in front of a sprite.
        /// </summary>
        public void setOrder(int index)
        {
            sprites.Remove(this);
            sprites.Insert(index, this);
        }

        /// <summary>
        /// Sets the depth (layer in drawing) of the sprite.
        /// </summary>
        /// <param name="depth">A provided depth in float value.</param>
        public void setDepth(float depth)
        {
            this.depth = depth;
        }

        /// <summary>
        /// Returns the depth (layer in drawing) of the sprite.
        /// </summary>
        /// <returns>The depth in a float value.</returns>
        public float getDepth()
        {
            return depth;
        }

        /// <summary>
        /// Sets the rotation for the sprite using degrees.
        /// </summary>
        public void setRotation(float d)
        {
            //if(d < 0 || d > 360) throw new ArgumentOutOfRangeException("Degrees is not within bounds.");
            rotation = MathHelper.ToRadians(d);
        }

        /// <summary>
        /// Rotates the sprite using degrees.
        /// </summary>
        public void rotate(float d)
        {
            //if (d < 0 || d > 360) Console.WriteLine("Degrees is not within bounds.");
            float degrees = MathHelper.ToDegrees(rotation);
            if (degrees + d < 0) degrees = (360 - Math.Abs(degrees + d));
            else if (degrees + d > 360) degrees = degrees + d - 360;
            else degrees += d;
            rotation = MathHelper.ToRadians(degrees);
        }

        /// <summary>
        /// Returns the rotation of the sprite in degrees.
        /// </summary>
        public float getRotation()
        {
            return MathHelper.ToDegrees(rotation);
        }

        /// <summary>
        /// Returns the rotation of the sprite in degrees. Useful for methods such as Math.Cos, Math.Sin, etc which requires radians.
        /// </summary>
        public float getRotationRadians()
        {
            return rotation;
        }

        /// <summary>
        /// Returns the index of the sprite.
        /// </summary>
        public int getIndex()
        {
            return index;
        }

        /// <summary>
        /// Sets the origin for the sprite.
        /// </summary>
        public void setOrigin(int x, int y)
        {
            //if (x > getWidth() || x < 0 || y > getHeight() || y < 0) throw new ArgumentOutOfRangeException("The origin is out of bounds.");
            //if (true) {
            origin.X = (int)(x / scale);
            origin.Y = (int)(y / scale);
            //}
            /*
            else
            {
                origin.X = (int)((x + getRegion().X) / scale);
                origin.Y = (int)((y + getRegion().Y) / scale);
            }
            */
        }

        /// <summary>
        /// Centers the sprite based on the current size. Only works once size is determined.
        /// </summary>
        public void centerOrigin()
        {
            setOrigin(getWidth() / 2, getHeight() / 2);
        }

        /// <summary>
        /// Enables or disables if the sprite is rendered.
        /// </summary>
        public void setVisible(bool value)
        {
            visible = value;
        }


        /// <summary>
        /// Get the visibility of the sprite. (Does not include if the sprite has no textures.)
        /// </summary>
        /// <returns>The boolean value if the sprite visabe.</returns>
        public bool isVisable()
        {
            return visible;
        }

        /// <summary>
        /// Sets the visibility for all sprites.
        /// </summary>
        public static void setAllVisible(Boolean value)
        {
            foreach (Sprite sprite in sprites) sprite.setVisible(value);
        }

        /// <summary>
        /// Sets the size of the sprite based off of the scale factor.
        /// </summary>
        public void setScale(Double factor)
        {
            scale = factor;
        }

        /// <summary>
        /// Sets the size of the sprite based off of the provided width and height.
        /// </summary>
        public void setSize(float width, float height)
        {
            if (sizes.Count == 0) throw new NullReferenceException("The size for this sprite has not been yet determined. Please use this method after loading a texture. The index of this sprite is " + sprites.IndexOf(this) + ".");
            sizes[index].set((int)width, (int)height);
        }

        /// <summary>
        /// Sets the size of the sprite based off of the provided width and height (in a Vector2).
        /// </summary>
        public void setSize(Vector2 size)
        {
            setSize(size.X, size.Y);
        }


        public void setUpdate(CustomUpdate update)
        {
            this.customUpdateMethod = update;
        }

        // -----------------------------------------------------------------
        // ---------------------- TEXTURE CODE -----------------------------
        // -----------------------------------------------------------------

        /// <summary>
        /// Adds the displayable texture to the sprite and sets the size.
        /// </summary>
        public void addTexture(Texture2D texture)
        {
            textures.Add(texture);
            sizes.Add(new Size(texture.Width, texture.Height));
            regions.Add(new List<Rectangle>());
            rectangle.Width = texture.Width;
            rectangle.Height = texture.Height;
        }

        /// <summary>
        /// Sets the sprite's active texture to one specified by the provided index.
        /// </summary>
        public void setTexture(int index)
        {
            if (index > textures.Count - 1 || index < 0)
            {
                Console.WriteLine("Invalid Index");
                return;
            }
            this.index = index;
            regionIndex = 0;
        }

        /// <summary>
        /// Sets the sprite's active texture to the next one.
        /// </summary>
        public void nextTexture()
        {
            index++;
            if (index > textures.Count - 1) index = 0;
        }

        /// <summary>
        /// Sets the sprite's active texture to the previous one.
        /// </summary>
        public void previousTexture()
        {
            index--;
            if (index < 0) index = textures.Count - 1;
        }

        /// <summary>
        /// Gets the current texture based off the textureIndex.
        /// </summary>
        public Texture2D getTexture()
        {
            return textures[index];
        }

        /// <summary>
        /// Gets the current texture based off the provided index.
        /// </summary>
        public Texture2D getTexture(int index)
        {
            return textures[index];
        }

        /// <summary>
        /// Gets the current color of the sprite.
        /// </summary>
        public Color getColor()
        {
            return color;
        }

        /// <summary>
        /// Adds a texture to the sprite using a provided filename. You can only use this if you set the Content Manager in a previous method.
        /// </summary>
        public void addTexture(String fileName)
        {
            //if (contentManager == null) throw new NullReferenceException("You have not set the content manager yet. Please use the setLoader modethod to set the content manager before you run this method.");
            addTexture(Useful.game.Content.Load<Texture2D>(fileName));
            regions.Add(new List<Rectangle>());
        }


        // --------------------------------------------------------------------------
        // ---------------------------- REGION CODE ---------------------------------
        // --------------------------------------------------------------------------

        /// <summary>
        /// Define the rectangle (of a texture) for animation.
        /// </summary>
        public void defRegion(int x, int y, int width, int height)
        {
            defRegion(new Rectangle(x, y, width, height));
        }

        /// <summary>
        /// Define the rectangle (of a texture) for animation.
        /// </summary>
        public void defRegion(Rectangle box)
        {
            getRegionList().Add(box);
            sizes.Add(new Size(box.Width, box.Height));
        }

        /// <summary>
        /// Sets the current region for the sprite using a provided index.
        /// </summary>
        /// <param name="index">An integer within the amount of regions.</param>
        public void setRegion(int index)
        {
            if (index < 0 || index > getRegionList().Count()) throw new IndexOutOfRangeException("There is no region with this index.");
            regionIndex = index;
        }

        /// <summary>
        /// Call this before you start adding regions.
        /// </summary>
        /// <param name="value"></param>
        public void useRegion(Boolean value)
        {
            regionUse = value;
            sizes.Clear();
        }

        /// <summary>
        /// Sets the sprite's active region to the next one.
        /// </summary>
        public void nextRegion()
        {
            regionIndex++;
            if (regionIndex > regions[index].Count - 1) regionIndex = 0;
        }

        /// <summary>
        /// Sets the sprite's active region to the previous one.
        /// </summary>
        public void previousRegion()
        {
            regionIndex--;
            if (regionIndex < 0) regionIndex = regions[index].Count - 1;
        }

        /// <summary>
        /// Gets the region using a provided texture index and a provided regionIndex for the texture.
        /// </summary>
        public Rectangle getRegion(int index, int regionIndex)
        {
            return regions[index][regionIndex];
        }

        /// <summary>
        /// Gets the region using a provided texture index and the currently active regionIndex.
        /// </summary>
        public Rectangle getRegion(int index)
        {
            return regions[index][regionIndex];
        }

        /// <summary>
        /// Gets the region using the currently active texture index and the currently active regionIndex.
        /// </summary>
        public Rectangle getRegion()
        {
            return regions[index][regionIndex];
        }

        /// <summary>
        /// Gets the region list for the provided texture index.
        /// </summary>
        public List<Rectangle> getRegionList(int index)
        {
            return regions[index];
        }

        /// <summary>
        /// Gets the region list for the currently active region index.
        /// </summary>
        public List<Rectangle> getRegionList()
        {
            return regions[index];
        }

        // --------------------------------------------------------------------
        // ------------------------ ANIMATION CODE ----------------------------
        // --------------------------------------------------------------------
        /// <summary>
        /// Updates the animation using the timer and comparing it to the timeframe.
        /// </summary>
        private void updateAnimation()
        {
            timer += 1 / 60f;
            if (timer >= timeframe)
            {
                if (!regionUse)
                    nextTexture();
                else nextRegion();
                timer = 0;
            }
        }

        /// <summary>
        /// Enables or disables the animation loop.
        /// </summary>
        public void setAnimation(Boolean value)
        {
            timer = index = 0;
            animation = value;
        }

        /// <summary>
        /// Set how fast you want each animation to loop per second.
        /// Default is one frame every three seconds.
        /// </summary>
        public void setTimeFrame(float value)
        {
            timeframe = value;
        }

        // -------------------------------------------------------------------------
        // --------------------------- POSITION CODE -------------------------------
        // -------------------------------------------------------------------------

        /// <summary>
        /// Moves the sprite in a certain direction.
        /// </summary>
        public void translate(float x, float y)
        {
            pos.X += x;
            pos.Y += y;
        }

        /// <summary>
        /// Sets the position of the sprite.
        /// </summary>
        public void setPos(float x, float y)
        {
            pos.X = x;
            pos.Y = y;
        }

        /// <summary>
        /// Sets the X position of the sprite.
        /// </summary>
        public void setX(float x)
        {
            pos.X = x;
        }

        /// <summary>
        /// Sets the Y position of the sprite.
        /// </summary>
        public void setY(float y)
        {
            pos.Y = y;
        }

        /// <summary>
        /// Moves the sprite in a certain direction.
        /// </summary>
        public void translate(Vector2 pos)
        {
            this.pos += pos;
        }

        /// <summary>
        /// Sets the position of the sprite.
        /// </summary>
        public void setPos(Vector2 pos)
        {
            this.pos = pos;
        }

        /// <summary>
        /// Returns the position of the sprite in a Vector2 format.
        /// </summary>
        public Vector2 getPos()
        {
            return pos;
        }

        /// <summary>
        /// Returns the position of the sprite plus the origin in a Vector2 format.
        /// </summary>
        public Vector2 getOriginPos()
        {
            return pos + origin;
        }

        /// <summary>
        /// Gets the X position of the sprite.
        /// </summary>
        public float getX()
        {
            return pos.X;
        }

        /// <summary>
        /// Gets the Y position of the sprite.
        /// </summary>
        public float getY()
        {
            return pos.Y;
        }

        /// <summary>
        /// Updates the position of the rectangle respective to the vector2 position variable.
        /// </summary>
        private void updatePos()
        {
            rectangle.X = (int)(pos.X);
            rectangle.Y = (int)(pos.Y);
        }

        // -----------------------------------------------------------------
        // ----------------------- USESPRITE CODE --------------------------
        // -----------------------------------------------------------------

        public void setGroup(String name)
        {
            if (!groups.ContainsKey(name))
                groups.Add(name,new List<Sprite>());
            groups[name].Add(this);
        }

        public static List<Sprite> getGroup(String name)
        {
            List<Sprite> ival;
            groups.TryGetValue(name, out ival);
            return ival;
        }

        public static bool groupAction(String name, Action<Sprite> action)
        {
            if (groups.ContainsKey(name))
            {
                getGroup(name).ForEach(action);
                return true;
            }
            return false;
        }

        // -----------------------------------------------------------------
        // ----------------------- USESPRITE CODE --------------------------
        // -----------------------------------------------------------------

        /// <summary>
        /// Updates all the logic in the sprite classes.
        /// Required if you want to do animation.
        /// </summary>
        public void update()
        {
            if (animation) updateAnimation();
            customUpdateMethod?.Invoke();
        }

        /// <summary>
        /// Draws the sprite if it is visable.
        /// </summary>
        public void draw(SpriteBatch batch)
        {
            if (!visible || sizes.Count == 0) return;
            updatePos();
            rectangle.Width = getWidth();
            rectangle.Height = getHeight();
            if (!regionUse) batch.Draw(getTexture(), rectangle, null, color, rotation, origin, effect, depth);
            else batch.Draw(getTexture(), rectangle, getRegion(), color, rotation, origin, effect, depth);
        }


        /// <summary>
        /// Draws all sprites created in order of creation using a provided spritebatch.
        /// </summary>
        public static void drawAll(SpriteBatch batch)
        {
            foreach (Sprite sprite in sprites)
            {
                sprite.draw(batch);
            }
        }
        /*
        /// <summary>
        /// Draws all sprites created in order of creation using the gloabal spritebatch.
        /// </summary>
        public static void drawAll()
        {
            drawAll(Useful.spriteBatch);
        }
        */

        /// <summary>
        /// Updates all sprites created.
        /// </summary>
        public static void updateAll()
        {
            foreach (Sprite sprite in sprites)
            {
                sprite.update();
            }
        }

        /// <summary>
        /// Storage of size in integeger form.
        /// </summary>
        private class Size
        {
            public int width, height;
            public Size(int width, int height)
            {
                this.width = width;
                this.height = height;
            }
            public void set(int width, int height)
            {
                this.width = width;
                this.height = height;
            }
        }
    }


    public static class KeyControl {
        public delegate void OnKeyEvent();

        private static KeyboardState kd, okd;

        private static Dictionary<Keys, OnKeyEvent> managerPress = new Dictionary<Keys, OnKeyEvent>(), 
            managerHold = new Dictionary<Keys, OnKeyEvent>();

        internal static void init()
        {
            okd = Keyboard.GetState();
        }

        public static bool addKeyPress(Keys key, OnKeyEvent press)
        {
            if (press != null)
            {
                managerPress[key] = press;
                return true;
            }
            return false;
        }

        public static bool addKeyHold(Keys key, OnKeyEvent press)
        {
            if (press != null)
            {
                managerHold[key] = press;
                return true;
            }
            return false;
        }

        public static bool removeKey(Keys key)
        {
            bool returnVal = false;
            if(removeKeyPress(key)) returnVal = true;
            if (removeKeyHold(key)) returnVal = true;
            return returnVal;
        }

        public static bool removeKeyPress(Keys key)
        {
            return managerPress.Remove(key);
        }

        public static bool removeKeyHold(Keys key)
        {
            return managerHold.Remove(key);
        }

        internal static void update()
        {
            kd = Keyboard.GetState();
            foreach (KeyValuePair<Keys, OnKeyEvent> entry in managerPress)
            {
                if (kd.IsKeyDown(entry.Key))
                {
                    entry.Value.Invoke();
                }
            }
            foreach (KeyValuePair<Keys, OnKeyEvent> entry in managerPress)
            {
                if (kd.IsKeyDown(entry.Key) && okd.IsKeyUp(entry.Key))
                {
                    entry.Value.Invoke();
                }
            }
            okd = kd;
        }
    }

    public static class ControllerControl
    {
        public delegate void OnControllerUpdate(GamePadState state);
        private static Dictionary<PlayerIndex, OnControllerUpdate> manager = new Dictionary<PlayerIndex, OnControllerUpdate>();

        public static bool add(OnControllerUpdate newControllerUpdate)
        {
            return add(PlayerIndex.One, newControllerUpdate);
        }
        public static bool add(PlayerIndex index, OnControllerUpdate newControllerUpdate)
        {
            if (newControllerUpdate != null)
            {
                manager.Add(index, newControllerUpdate);
                return true;
            }
            return false;
        }
        public static bool remove(PlayerIndex index)
        {
            return manager.Remove(index);
        }
        internal static void update()
        {
            foreach(KeyValuePair<PlayerIndex, OnControllerUpdate> entry in manager)
            {
                entry.Value.Invoke(GamePad.GetState(entry.Key));
            }
        }

    }

    /// <summary>
    /// Custom Texture Class - Created By Rithvik Senthilkumar | 
    /// Allows for easy handling of text. | 
    /// If you are not Rithvik and do not have permission from him, please dispose of this file immediately.
    /// </summary>
    public class Text
    {
        private static List<Text> texts = new List<Text>();
        public String content = "";
        public Vector2 pos = new Vector2(0, 0);
        private SpriteFont font;
        private static SpriteFont normal;
        public Boolean visable = true, useFont = false;
        public Color color = Color.Black;
        /// <summary>
        /// Sets the default font for all fonts.
        /// </summary>
        /// <param name="fontName"></param>
        public static void setDefaultFont(String fontName)
        {
            normal = Useful.game.Content.Load<SpriteFont>(fontName);
        }
        /// <summary>
        /// Create new Text Object with all values defaulted.
        /// </summary>
        public Text() {
            texts.Add(this);
        }
        
        /// <summary>
        /// Create a Text class with content provided and default position values.
        /// </summary>
        public Text(String content) : this() { this.content = content; }

        /// <summary>
        /// Create a Text class with content and position provided.
        /// </summary>
        public Text(String content, float x, float y) : this() { this.content = content; pos.X = x; pos.Y = y; }
        /// <summary>
        /// Create a Text class with no content and position provided.
        /// </summary>
        public Text(float x, float y) : this() { pos.X = x; pos.Y = y; }

        /// <summary>
        /// Set the font for the current Text.
        /// </summary>
        /// <param name="fontName">The font file name to load.</param>
        public void setFont(String fontName)
        {
            useFont = true;
            font = Useful.game.Content.Load<SpriteFont>(fontName);
        }

        /*
        public static void drawAll()
        {
            drawAll(Useful.spriteBatch);
        }
        */

        /// <summary>
        /// Draw all Texts using a provided SpriteBatch.
        /// </summary>
        public static void drawAll(SpriteBatch batch)
        {
            foreach (Text text in texts) text.draw(batch);
        }

        /// <summary>
        /// Draw the current text class using a provided SpriteBatch.
        /// </summary>
        public void draw(SpriteBatch batch)
        {
            if (visable)
            {
                if (useFont) batch.DrawString(font, content, pos, color);
                else batch.DrawString(normal, content, pos, color);
            }
        }

        /// <summary>
        /// Moves the sprite in a certain direction.
        /// </summary>
        public void translate(float x, float y)
        {
            pos.X += x;
            pos.Y += y;
        }

        /// <summary>
        /// Moves the text in a certain direction.
        /// </summary>
        public void translate(Vector2 pos)
        {
            this.pos += pos;
        }

        /// <summary>
        /// Returns the size of the font in pixels when drawn in a Vector2 format.
        /// </summary>
        public Vector2 getSize()
        {
            if (useFont)
                return font.MeasureString(content);
            else
                return normal.MeasureString(content);
        }

        /// <summary>
        /// Positions the text at the center of the screen.
        /// </summary>
        public void center()
        {
            pos.X = Useful.getWWidth() / 2 - getSize().X / 2;
            pos.Y = Useful.getWHeight() / 2 - getSize().Y / 2;
        }


        //Static Methods

        public static void drawLine(SpriteBatch batch, String content, Vector2 pos, Color color)
        {
            batch.DrawString(normal, content, pos, color);
        }

        public static void drawLine(SpriteBatch batch, String content, float x, float y, Color color)
        {
            batch.DrawString(normal, content, new Vector2(x,y), color);
        }

        public static void drawLine(SpriteBatch batch, String content, Vector2 pos)
        {
            batch.DrawString(normal, content, pos, Color.Black);
        }

        public static void drawLine(SpriteBatch batch, String content, float x, float y)
        {
            batch.DrawString(normal, content, new Vector2(x, y), Color.Black);
        }

        public static Vector2 getSize(String content)
        {
            return normal.MeasureString(content);
        }
    }


    public static class Useful
    {
        internal static Game game;
        //internal static SpriteBatch spriteBatch;

        internal static Camera2d cam;

        public static void setCamera(Camera2d camera)
        {
            cam = camera;
        }

        /// <summary>
        /// Gets the window width of the application. A shortcut for GraphicsDevice.Viewport.Width.
        /// </summary>
        public static int getWWidth()
        {
            return game.GraphicsDevice.Viewport.Width;
        }

        /// <summary>
        /// Gets the window height of the application. A shortcut for GraphicsDevice.Viewport.Width.
        /// </summary>
        public static int getWHeight()
        {
            return game.GraphicsDevice.Viewport.Height;
        }

        /// <summary>
        /// Creates a filled rectangle texture2d.
        /// </summary>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="color">Color of the rectangle</param>
        /// <returns>A Texture2d Rectangle using the specified parameters.</returns>
        public static Texture2D CreateRectangle(int width, int height, Color color)
        {
            Texture2D rectangleTexture = new Texture2D(game.GraphicsDevice, width, height);
            // create the rectangle texture, ,but it will have no color! lets fix that
            Color[] data = new Color[width * height];//set the color to the amount of pixels in the textures
            for (int i = 0; i < data.Length; i++)//loop through all the colors setting them to whatever values we want
            {
                data[i] = color;
            }
            rectangleTexture.SetData(data);//set the color data on the texture
            return rectangleTexture;//return the texture
        }

        /// <summary>
        /// Create a hollow box texture2d.
        /// </summary>
        /// <param name="width">Width of the box</param>
        /// <param name="height">Height of the box</param>
        /// <param name="color">Color of the box</param>
        /// <param name="depth">The amount of pixels for the width of the lines of the box.</param>
        /// <returns>A Texture2d Rectangle using the specified parameters.</returns>
        public static Texture2D CreateBox(int width, int height, int depth, Color color)
        {
            Texture2D rectangleTexture = new Texture2D(game.GraphicsDevice,width,height);
            Color[,] data = new Color[height,width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    data[i, j] = Color.Transparent;
            for (int amount = 0; amount < depth; amount++)
            {
                for (int count = 0; count < height; count++)
                {
                    data[count,amount] = color;
                    data[count, width - 1 - amount] = color;
                }
                for (int count = 0; count < width; count++)
                {
                    data[amount,count] = color;
                    data[height-1-amount,count] = color;
                }
            }

            rectangleTexture.SetData(d2Tod1(data));//set the color data on the texture
            return rectangleTexture;//return the texture
        }
        /*
        public static void drawAll()
        {
            drawAll(spriteBatch);
        }
        */

        /// <summary>
        /// Draws all for both the Text Class and Sprite Class.
        /// </summary>
        public static void drawAll(SpriteBatch batch)
        {
            Sprite.drawAll(batch);
            Text.drawAll(batch);
        }

        /// <summary>
        /// Required before doing any of the methods in the SureDroid package.
        /// </summary>
        /// <param name="g">Game Class (This)</param>
        public static void set(Game g)
        {
            game = g;
            game.Components.Add(new Automate(game));
            //spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        /// <summary>
        /// Gets a Texture2D from a file.
        /// </summary>
        public static Texture2D getTexture(String fileName)
        {
            return Useful.game.Content.Load<Texture2D>(fileName);
        }

        /// <summary>
        /// Gets a SpriteFont from a file.
        /// </summary>
        public static SpriteFont getFont(String fileName)
        {
            return Useful.game.Content.Load<SpriteFont>(fileName);
        }

        /// <summary>
        /// Returns the contents of a file in a string.
        /// </summary>
        /// <param name="filePath">The path of the file with the name and extension.</param>
        /// <example>
        /// <code>
        /// String contents = Useful.readFile(@"Content\textFile.txt");
        /// </code>
        /// </example>
        public static string readFile(String filePath)
        {
            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// Returns the contents of a file in a string array. Each line is its own string in the array.
        /// </summary>
        /// <param name="filePath">The path of the file with the name and extension.</param>
        /// <example>
        /// <code>
        /// String[] contentLines = Useful.readFileLines(@"Content\textFile.txt");
        /// </code>
        /// </example>
        public static string[] readFileLines(String filePath)
        {
            return File.ReadAllLines(filePath);
        }

        /// <summary>
        /// Reads all numbers from a provided input. Ignores non-digits.
        /// </summary>
        public static int[] readAllNumbers(string input)
        {
            List<int> ints = new List<int>();
            string[] values = Regex.Split(input, @"\D+");
            foreach (string value in values)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    ints.Add(int.Parse(value));
                } else
                {
                    Console.WriteLine("Value during number conversoin was null or empty. Value: \"" + value + "\"");
                }
            }
            return ints.ToArray();
        }

        private static char[] splitChars = new char[] { '-' };

        /// <summary>
        /// Splits the provided string into words, seperated by spaces.
        /// </summary>
        public static string[] split(String value)
        {
            return Regex.Replace(value, @"\r\n?|\n|\s+", "-").Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Splits the provided string into words, seperated by a provided character.
        /// </summary>
        public static string[] split(String value, char spChar)
        {
            return Regex.Replace(value, @"\r\n?|\n|\s+|"+spChar, "-").Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Writes a provided string to a provided file location.
        /// </summary>
        /// <param name="filePath">The path of the file with the name and extension.</param>
        public static void writeFile(string filePath, string text)
        {
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// Writes a provided string array to a provided file location, with each string in the string array being a seperate line.
        /// </summary>
        /// <param name="filePath">The path of the file with the name and extension.</param>
        public static void writeFile(string filePath, string[] text)
        {
            File.WriteAllLines(filePath, text);
        }

        /// <summary>
        /// Writes a provided string to a provided file location, with the option of appending it to the file instead of overwriting to it.
        /// </summary>
        /// <param name="filePath">The path of the file with the name and extension.</param>
        public static void writeFile(string filePath, string text, bool append)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, append))
            {
                file.WriteLine(text);
            }
        }

        /// <summary>
        /// Converts a two dimentional color array to an one dimentional color array.
        /// </summary>
        private static Color[] d2Tod1(Color[,] array)
        {
            int width = array.GetLength(1);
            int height = array.GetLength(0);
            Color[] newArray = new Color[height * width];

            for (int i = 0; i < height; ++i)
                for (int j = 0; j < width; ++j)
                {
                    newArray[i * width + j] = array[i,j];
                }

            return newArray;
        }

    }

    public static class RandomExtensions
    {
        /// <summary>
        /// Return a random value between 0 inclusive and max exclusive.
        /// </summary>
        public static double NextDouble(this Random rand, double max)
        {
            return rand.NextDouble() * max;
        }
        /// <summary>
        /// Return a random value between min inclusive and max exclusive.
        /// </summary>
        public static double NextDouble(this Random rand,
            double min, double max)
        {
            return min + (rand.NextDouble() * (max - min));
        }

        /// <summary>
        /// Clones a list.
        /// </summary>
        public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }

    public class Camera2d
    {
        protected float _zoom; // Camera Zoom
        public Matrix _transform; // Matrix Transform
        public Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation
        public bool _centered = true;

        /// <summary>
        /// Returns if the camera is centered.
        /// </summary>
        public bool Centered
        {
            get { return _centered; }
            set { _centered = value; }
        }

        /// <summary>
        /// The zoom value of the camera.
        /// </summary>
        /// <remarks>
        /// A negetive zoom will flip the image.
        /// </remarks>
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        /// <summary>
        /// The rotation value of the camera.
        /// </summary>
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        /// <summary>
        /// Auxiliary function to move the camera
        /// </summary>
        public void Move(Vector2 amount)
        {
            _pos += amount;
        }

        /// <summary>
        /// The position value of the camera.
        /// </summary>
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        /// <summary>
        /// Gets the transformed value for the camera.
        /// </summary>
        public Matrix get_transformation()
        {
            _transform = _centered ?
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(Useful.getWWidth() * 0.5f, Useful.getWHeight() * 0.5f, 0)) :

              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(1, 1, 0));
            return _transform;
        }

        /// <summary>
        /// The constructor for the camera.
        /// </summary>
        public Camera2d()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
        }

        /*
          spriteBatch.Begin(SpriteSortMode.BackToFront,
                        BlendState.AlphaBlend,
                        null,
                        null,
                        null,
                        null,
                        cam.get_transformation());
                        */

    }

    internal class Automate : DrawableGameComponent
    {
        SpriteBatch spriteBatch;

        KeyboardState kb, okb;

        public Automate(Game game) : base(game) { }

        public override void Initialize()
        {
            base.Initialize();
            spriteBatch = new SpriteBatch(Useful.game.GraphicsDevice);
            KeyControl.init();
        }
        

        public override void Update(GameTime gameTime)
        {
            KeyControl.update();
            ControllerControl.update();
            Sprite.updateAll();
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if(Useful.cam == null)
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, null,null);
            else
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, null, null, Useful.cam.get_transformation());
            Useful.drawAll(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }

    public abstract class Drawable
    {
        public abstract void update(GameTime time);
    }

    public interface IDrawable
    {
        void draw(GameTime time);
    }

    public abstract class Updatable
    {
        public abstract void update(SpriteBatch batch);
    }

    public interface IUpdatable
    {
        void update(GameTime time);
    }

    public interface Component : IDrawable , IUpdatable
    {

    }
    
    public class Bar
    { 
        private int currentVal, max;
        private Sprite fill, outline, cover;
        private static int count = 0;
        private readonly int position;
        private Wrapper<int> reference;

        public Bar(int x, int y, int width, int height, int max) : this(x,y,width,height, max, max) { }

        public Bar(int x, int y, int width, int height, int current, int max) : this(x, y, width, height, current, max, Color.Red, Color.Green, Color.White) { }

        public Bar(int x, int y, int width, int height, int current, int max, Color baseColor, Color coverColor, Color outlineColor)
        {
            Rectangle baseRect = new Rectangle(x, y, width, height);
            position = count;
            count++;

            this.max = max;

            fill = new Sprite();
            cover = new Sprite();
            outline = new Sprite();

            fill.setDepth(0.3f);
            cover.setDepth(0.2f);
            outline.setDepth(0.1f);

            fill.setGroup("bar" + position);
            outline.setGroup("bar" + position);
            cover.setGroup("bar" + position);


            Sprite.groupAction("bar" + position, sprite => sprite.setPos(x, y));

            Texture2D box = Useful.CreateRectangle(width, height, Color.White);

            fill.addTexture(box);
            outline.addTexture(Useful.CreateBox(width, height, 3, Color.White));
            cover.addTexture(box);

            setVal(current);

            setBaseColor(baseColor);
            setOutlineColor(outlineColor);
            setCoverColor(coverColor);
            
        }

        public void setReference(Wrapper<int> val)
        {
            reference = val;
            fill.setUpdate(() =>
            {
                if (reference.Value != currentVal)
                {
                    setVal(reference.Value);
                }
            });
        }

        public void setBaseColor(Color color)
        {
            fill.setColor(color);
        }

        public void setOutlineColor(Color color)
        {
            outline.setColor(color);
        }

        public void setCoverColor(Color color)
        {
            cover.setColor(color);
        }

        public void setVal(int num)
        {
            if (currentVal != num)
            {
                if (num < 0)
                    num = 0;
                else if (num > max)
                    num = max;
                else
                    currentVal = num;
                updateBar();
            }
        }

        public void setMax(int num)
        {
            max = num;
            //setVal(currentVal + num);
            updateBar();
        }

        public void increment(int num)
        {
            setVal(currentVal + num);
        }

        public void decrement(int num)
        {
            setVal(currentVal - num);
        }

        private void updateBar()
        {
            cover.setSize(((float)currentVal / max) * fill.getWidth(), fill.getHeight());
        }

        public void change(Action<Sprite> action)
        {
            Sprite.groupAction("bar" + position, action);
            updateBar();
        }
    }


    public class Wrapper<T> where T : struct
    {
        public static implicit operator T(Wrapper<T> w)
        {
            return w.Value;
        }

        public Wrapper(T t)
        {
            _t = t;
        }

        public T Value
        {
            get
            {
                return _t;
            }

            set
            {
                _t = value;
            }
        }

        public override string ToString()
        {
            return _t.ToString();
        }

        private T _t;
    }
}