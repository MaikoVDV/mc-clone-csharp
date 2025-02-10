using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mc_clone
{
    internal class Player
    {
        public Camera camera;
        private World world;

        public Player(World world, Camera camera)
        {
            this.world = world;
            this.camera = camera;
        }

        public void Update(
            GameTime gameTime,
            GraphicsDevice graphicsDevice,
            KeyboardState keyState,
            MouseState mouseState
        )
        {
            camera.Update(gameTime, graphicsDevice, keyState, mouseState);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                Ray pointer = new Ray(camera.Position, camera.ViewDirection);
                var hit = world.CastRay(pointer);

                if (hit is (Block block, Vector3 point, (int x, int y, int z) coords))
                {
                    world.RemoveBlock(coords);
                }
            }
        }
    }
}
