using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mc_clone
{
    public class MCClone : Game
    {
        private GraphicsDeviceManager _graphics;
        private Texture2D _textureAtlas;

        private Player player;
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
            _textureAtlas = Content.Load<Texture2D>("texture_atlas");
            Globals.TEXTURE_STEP_FACTOR = new Vector2(
                (float)Globals.TEXTURE_WIDTH / (float)_textureAtlas.Width,
                (float)Globals.TEXTURE_WIDTH / (float)_textureAtlas.Height);

            base.LoadContent();
        }

        protected override void Initialize()
        {
            base.Initialize();

            world = new World(GraphicsDevice, _textureAtlas);

            // Initialize the player with a camera and reference to the world
            Camera playerCam = new Camera((_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));
            player = new Player(world, playerCam);

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
                world.Update(GraphicsDevice);
                player.Update(gameTime, GraphicsDevice, keyState, mouseState);

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

            //BasicEffect debugEffect = new BasicEffect(GraphicsDevice);
            //debugEffect.View = player.camera.Matrices.view;
            //debugEffect.Projection = player.camera.Matrices.projection;
            //var rayData = world.CastRay(new Ray(new Vector3(24, 17, 18), Vector3.Down), 0.5f);
            //debugEffect.World = Matrix.CreateTranslation(rayData.Value.hitPoint);
            
            //CubeMesh cubeMesh = new CubeMesh();
            //GraphicsDevice.SetVertexBuffer(cubeMesh.faces[0].vertices);

            world.Draw(GraphicsDevice, player.camera);

            base.Draw(gameTime);
        }
        public bool PointInsideViewport(Point p)
        {
            return p.X > 0 && p.X < GraphicsDevice.Viewport.Width
                && p.Y > 0 && p.Y < GraphicsDevice.Viewport.Height;
        }
    }
}
