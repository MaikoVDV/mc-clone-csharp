using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace mc_clone
{
    public static class Globals
    {
        public static readonly Vector3 AXIS_VECTOR_X = new Vector3(1, 0, 0);
        public static readonly Vector3 AXIS_VECTOR_Y = new Vector3(0, 1, 0);
        public static readonly Vector3 AXIS_VECTOR_Z = new Vector3(0, 0, 1);
        public static readonly short CHUNK_SIZE_XZ = 16;
        public static readonly short CHUNK_SIZE_Y = 16;
        public static readonly short TEXTURE_WIDTH = 16; // Width of the texture of a single block face (in pixels)

        public static Vector2 TEXTURE_STEP_FACTOR;

        public static Effect basicEffect;
    }
}
