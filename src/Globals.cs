using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mc_clone
{
    public static class Globals
    {
        public static readonly short CHUNK_SIZE_XZ = 16;
        public static readonly short CHUNK_SIZE_Y = 16;
        public static readonly short TEXTURE_WIDTH = 16; // Width of the texture of a single block face (in pixels)

        public static Vector2 TEXTURE_STEP_FACTOR;
    }
}
