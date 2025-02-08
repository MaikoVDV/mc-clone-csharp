using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mc_clone
{
    internal class Camera
    {
        private Vector3 position;
        private Vector3 viewTarget;
        private readonly Vector3 upVector;
        private float moveSpeed = 0.2f;

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
            position = new Vector3(0, 0, 5);
            viewTarget = Vector3.Zero;
            upVector = Vector3.Up;

            // Set camera matrices
            view = Matrix.CreateLookAt(position, viewTarget, upVector);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, windowDimensions.width / (float)windowDimensions.height, 0.1f, 100f);

        }

        public void Update(
            GameTime gameTime,
            GraphicsDevice graphicsDevice,
            KeyboardState keyState,
            MouseState mouseState
        ) {
            // Camera movement (WASD, LCtrl, Space)
            Vector3 forward = Vector3.Normalize(viewTarget - position);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, upVector));

            if (keyState.IsKeyDown(Keys.W))
                position += forward * moveSpeed;
            if (keyState.IsKeyDown(Keys.S))
                position -= forward * moveSpeed;
            if (keyState.IsKeyDown(Keys.A))
                position -= right * moveSpeed;
            if (keyState.IsKeyDown(Keys.D))
                position += right * moveSpeed;
            if (keyState.IsKeyDown(Keys.Space))
                position += upVector * moveSpeed;
            if (keyState.IsKeyDown(Keys.LeftControl))
                position -= upVector * moveSpeed;


            // Get mouse movement from center of window
            int centerX = graphicsDevice.Viewport.Width / 2;
            int centerY = graphicsDevice.Viewport.Height / 2;

            float deltaX = mouseState.X - centerX;
            float deltaY = mouseState.Y - centerY;

            yaw += deltaX * _sensitivity;
            pitch -= deltaY * _sensitivity;

            // Clamp vertical rotation to avoid flipping
            pitch = MathHelper.Clamp(pitch, -MathHelper.PiOver2 + 0.1f, MathHelper.PiOver2 - 0.1f);

            // Calculate new camera direction
            Vector3 cameraDirection = new Vector3(
                (float)(Math.Cos(yaw) * Math.Cos(pitch)),
                (float)Math.Sin(pitch),
                (float)(Math.Sin(yaw) * Math.Cos(pitch))
            );

            viewTarget = position + Vector3.Normalize(cameraDirection);
            if (keyState.IsKeyDown(Keys.C))
                viewTarget = position + new Vector3(0, 0, -5);

            // Update the view matrix
            view = Matrix.CreateLookAt(position, viewTarget, upVector);

            // Reset mouse position to center
            Mouse.SetPosition(centerX, centerY);
        }
    }
}
