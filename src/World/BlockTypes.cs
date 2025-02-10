using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mc_clone
{
    public enum BlockTypes
    {
        Stone,
        Dirt,
        Grass
    }

    public static class BlockTypesExtension
    {
        public static Vector2 ToUV(this BlockTypes type, BlockFaceDirection direction)
        {
            //Debug.WriteLine(type.ToString());
            (int x, int y) textureCoords = type switch
            {
                BlockTypes.Stone => (1, 0),
                BlockTypes.Dirt => (2, 0),
                BlockTypes.Grass => direction switch
                {
                    BlockFaceDirection.Top => (0, 0),
                    BlockFaceDirection.Bottom => (2, 0),
                    _ => (3, 0)
                },
                _ => throw new Exception($"Tried getting texture coords for an unknown blocktype {type.ToString()}")
            };
            return new Vector2(
                (float)textureCoords.x * Globals.TEXTURE_STEP_FACTOR.X,
                (float)textureCoords.y * Globals.TEXTURE_STEP_FACTOR.Y);
        }
    }
}
