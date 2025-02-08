using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mc_clone
{
    public class MCClone : Game
    {
        private GraphicsDeviceManager _graphics;
        private BasicEffect _effect;
        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private Camera mainCam;

        private bool paused = false;

        public static readonly short CHUNK_SIZE_XZ = 16;
        public static readonly short CHUNK_SIZE_Y = 16;

        public MCClone()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1080;
            _graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            //// Define cube vertices (8 corners)
            //VertexPositionColor[] vertices = new VertexPositionColor[]
            //{
            //new VertexPositionColor(new Vector3(-1, -1, -1), Color.Red), // 0
            //new VertexPositionColor(new Vector3(1, -1, -1), Color.Green), // 1
            //new VertexPositionColor(new Vector3(1, 1, -1), Color.Blue), // 2
            //new VertexPositionColor(new Vector3(-1, 1, -1), Color.Yellow), // 3
            //new VertexPositionColor(new Vector3(-1, -1, 1), Color.Cyan), // 4
            //new VertexPositionColor(new Vector3(1, -1, 1), Color.Magenta), // 5
            //new VertexPositionColor(new Vector3(1, 1, 1), Color.White), // 6
            //new VertexPositionColor(new Vector3(-1, 1, 1), Color.Black) // 7
            //};

            //// Define indices (two triangles per face, 6 faces, 12 triangles)
            //short[] indices = new short[]
            //{
            //0, 1, 2,  0, 2, 3, // Front
            //1, 5, 6,  1, 6, 2, // Right
            //5, 4, 7,  5, 7, 6, // Back
            //4, 0, 3,  4, 3, 7, // Left
            //3, 2, 6,  3, 6, 7, // Top
            //4, 5, 1,  4, 1, 0  // Bottom
            //};

            //for (int i = 0; i < vertices.Length; i++)
            //{
            //    vertices[i].Position.Z -= 5;
            //}

            Chunk chunk = new Chunk();
            var mesh = chunk.BuildMesh();

            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                mesh.vertices[i].Position.Z -= 5;
            }
            Debug.WriteLine(mesh.indices.Length);
            _vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), mesh.vertices.Length, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(mesh.vertices);

            _indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, mesh.indices.Length, BufferUsage.WriteOnly);
            _indexBuffer.SetData(mesh.indices);

            //for (int i = 0; i < mesh.indices.Length - 2; i += 3)
            //{
            //    Debug.WriteLine($"{mesh.indices[i]}, {mesh.indices[i + 1]}, {mesh.indices[i + 2]}, ");
            //}
            //for (int i = 0; i < mesh.vertices.Length - 2; i += 3)
            //{
            //    Debug.WriteLine($"{mesh.vertices[i]}, {mesh.vertices[i + 1]}, {mesh.vertices[i + 2]}, ");
            //}

            // Initialize BasicEffect
            _effect = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
                LightingEnabled = false
            };

            // Initialize the camera
            mainCam = new Camera((_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight));

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

                mainCam.Update(gameTime, GraphicsDevice, keyState, mouseState);

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
            //if (!IsActive) return; // If application unfocussed, don't draw.

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Set vertex and index buffers
            GraphicsDevice.SetVertexBuffer(_vertexBuffer);
            GraphicsDevice.Indices = _indexBuffer;

            (Matrix view, Matrix projection) = mainCam.Matrices;

            _effect.World = Matrix.Identity;
            _effect.View = view;
            _effect.Projection = projection;

            // Apply effect and draw
            foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _indexBuffer.IndexCount);
            }

            base.Draw(gameTime);
        }
        public bool PointInsideViewport(Point p)
        {
            return p.X > 0 && p.X < GraphicsDevice.Viewport.Width
                && p.Y > 0 && p.Y < GraphicsDevice.Viewport.Height;
        }
    }
}
