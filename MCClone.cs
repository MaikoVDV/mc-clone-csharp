using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using mc_clone.src.WorldData;
using mc_clone.src.Entities.Player;

namespace mc_clone
{
    public class MCClone : Game
    {
        private GraphicsDeviceManager _graphics;
        private Texture2D _textureAtlas;

        private World world;

        private bool paused = false;

        public MCClone()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1080;
            _graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";

            //IsFixedTimeStep = false;
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            _textureAtlas = Content.Load<Texture2D>("texture_atlas_new");
            Globals.TEXTURE_STEP_FACTOR = new Vector2(
                (float)Globals.TEXTURE_WIDTH / (float)_textureAtlas.Width,
                (float)Globals.TEXTURE_WIDTH / (float)_textureAtlas.Height);


            // Initialize BasicEffect
            Globals.basicEffect = Content.Load<Effect>("Effects/Basic");
            Globals.basicEffect.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());
            Globals.basicEffect.Parameters["AmbientIntensity"].SetValue(0.1f);
            Globals.basicEffect.Parameters["ModelTexture"].SetValue(_textureAtlas);

            base.LoadContent();
        }

        protected override void Initialize()
        {
            base.Initialize();

            world = new World(GraphicsDevice, _textureAtlas, new Camera((_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight)));

            // Configure mouse
            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            IsMouseVisible = false;
        }

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive) return; // If application unfocussed, don't update.

            // Aquire inputs
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (!paused)
            {
                // Check if the game should be paused
                if (keyState.IsKeyDown(Keys.Escape))
                {
                    IsMouseVisible = true;
                    paused = true;
                }
                world.timeSinceUpdate += gameTime.ElapsedGameTime.TotalSeconds;
                if (world.timeSinceUpdate > 1 / 20)
                {
                    world.Update(GraphicsDevice);
                }
                world.player.Update(gameTime, GraphicsDevice, keyState, mouseState);

            } else
            {
                // Check if the game should be unpaused
                if (mouseState.LeftButton == ButtonState.Pressed && PointInsideViewport(mouseState.Position))
                {
                    IsMouseVisible = false;
                    paused = false;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp; // Don't interpolated pixel art textures.

            world.Draw(GraphicsDevice);

            base.Draw(gameTime);
        }

        public bool PointInsideViewport(Point p)
        {
            return p.X > 0 && p.X < GraphicsDevice.Viewport.Width
                && p.Y > 0 && p.Y < GraphicsDevice.Viewport.Height;
        }
    }
}
