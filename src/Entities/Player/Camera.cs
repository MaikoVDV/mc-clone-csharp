using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mc_clone.src.Entities.Player
{
    public class Camera
    {
        private Vector3 position;
        public Vector3 Position { get { return position; } set { position = value; } }
        private Vector3 viewTarget;
        public Vector3 ViewTarget { get { return viewTarget; } }
        public Vector3 ViewDirection
        {
            get
            {
                return new Vector3(
                    (float)(Math.Cos(yaw) * Math.Cos(pitch)),
                    (float)Math.Sin(pitch),
                    (float)(Math.Sin(yaw) * Math.Cos(pitch))
                );
            }
        }
        public Vector3 ViewDirectionFlat
        {
            get
            {
                return new Vector3(
                    (float)Math.Cos(yaw),
                    0,
                    (float)Math.Sin(yaw)
                );
            }
        }
        private readonly Vector3 upVector;

        // Rotation
        private float pitch = 0f; // Up/Down rotation
        private float yaw = 0f;   // Left/Right rotation
        private float _sensitivity = 0.002f; // Mouse sensitivity

        // Projection
        private Matrix view, projection;
        public (Matrix view, Matrix projection) Matrices
        {
            get
            {
                return (view, projection);
            }
        }

        public Camera((float width, float height) windowDimensions)
        {
            position = new Vector3(-8, 24, 8);
            viewTarget = Vector3.Zero;
            upVector = Vector3.Up;

            // Set camera matrices
            view = Matrix.CreateLookAt(position, viewTarget, upVector);
            projection = Matrix.CreatePerspectiveFieldOfView(80f * ((float)Math.PI / 180f), windowDimensions.width / windowDimensions.height, 0.1f, 1000f);
        }

        public void Update(
            GameTime gameTime,
            GraphicsDevice graphicsDevice,
            KeyboardState keyState,
            MouseState mouseState
        )
        {


            // Get mouse movement from center of window
            int centerX = graphicsDevice.Viewport.Width / 2;
            int centerY = graphicsDevice.Viewport.Height / 2;

            float deltaX = mouseState.X - centerX;
            float deltaY = mouseState.Y - centerY;

            yaw += deltaX * _sensitivity;
            pitch -= deltaY * _sensitivity;

            // Clamp vertical rotation to avoid flipping
            pitch = MathHelper.Clamp(pitch, -MathHelper.PiOver2 + 0.1f, MathHelper.PiOver2 - 0.1f);


            viewTarget = position + Vector3.Normalize(ViewDirection);

            // Update the view matrix
            view = Matrix.CreateLookAt(position, viewTarget, upVector);

            // Reset mouse parentPos to center
            Mouse.SetPosition(centerX, centerY);
        }
    }
}
