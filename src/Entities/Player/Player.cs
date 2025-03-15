using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using mc_clone.src.WorldData;
using mc_clone.src.WorldData.Blocks;

namespace mc_clone.src.Entities.Player
{
    public class Player : PhysicsEntity
    {
        public Camera camera;
        private MouseState prevMouseState;
        private float accelForce = 0.5f;
        private float jumpForce = 9f;

        private Vector3 cameraOffset = new Vector3(0, 1.5f, 0);

        public Player(World world, Camera camera) : base(world)
        {
            this.camera = camera;

            position = new Vector3(22, 17, 12);
            maxSpeed = 3;
            dragVector = new Vector3(1.5f, 0f, 1.5f);
            //dragVector = new Vector3(1.5f, 1.5f, 1.5f);
            gravityVector = new Vector3(0, -0.5f, 0);
        }

        public void Update(
            GameTime gameTime,
            GraphicsDevice graphicsDevice,
            KeyboardState keyState,
            MouseState mouseState
        )
        {
            // Camera movement (WASD, LCtrl, Space)
            Vector3 forward = camera.ViewDirectionFlat;
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.Up));

            if (keyState.IsKeyDown(Keys.W))
                ApplyForce(forward * accelForce);
            if (keyState.IsKeyDown(Keys.S))
                ApplyForce(-forward * accelForce);
            if (keyState.IsKeyDown(Keys.A))
                ApplyForce(-right * accelForce);
            if (keyState.IsKeyDown(Keys.D))
                ApplyForce(right * accelForce);
            if (keyState.IsKeyDown(Keys.Space) && OnGround)
                ApplyForce(Vector3.Up * jumpForce);
            if (keyState.IsKeyDown(Keys.LeftShift))
                ApplyForce(-Vector3.Up * accelForce);

            UpdatePhysics(gameTime);

            camera.Position = position + cameraOffset;
            camera.Update(gameTime, graphicsDevice, keyState, mouseState);

            // Placing and breaking blocks
            if (SingleClickLeft(mouseState))
            {
                Ray pointer = new Ray(camera.Position, camera.ViewDirection);
                var hit = world.CastRay(pointer);

                if (hit is (Block block, BlockFaceDirection side, Vector3 point, BlockCoordinates coords))
                {
                    world.RemoveBlock(coords);
                }
            }
            if (SingleClickRight(mouseState))
            {
                Ray pointer = new Ray(camera.Position, camera.ViewDirection);
                var hit = world.CastRay(pointer);

                if (hit is (Block block, BlockFaceDirection side, Vector3 point, BlockCoordinates coords))
                {

                    BlockCoordinates newBlockLocation = coords + side.ToOffsetVector();
                    world.AddBlock(newBlockLocation, BlockTypes.Grass);
                }
            }

            prevMouseState = mouseState;
        }
        private bool SingleClickLeft(MouseState currentState)
        {
            return currentState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released;
        }
        private bool SingleClickRight(MouseState currentState)
        {
            return currentState.RightButton == ButtonState.Pressed && prevMouseState.RightButton == ButtonState.Released;
        }
    }
}
