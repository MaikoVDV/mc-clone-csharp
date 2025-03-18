using mc_clone.src.WorldData.Blocks.Types;
using Microsoft.Xna.Framework;
using System;

namespace mc_clone.src.WorldData.Blocks
{
    public enum BlockTypes
    {
        Air,
        Stone,
        Dirt,
        Grass,
        Water,
    }

    public static class BlockTypesExtension
    {
        public static Vector2 ToUV(this BlockTypes type, BlockFaceDirection direction)
        {
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
                BlockTypes.Water => (14, 0),
                _ => throw new NotImplementedException($"Tried getting texture coords for an unknown blocktype {type.ToString()}")
            };
            return new Vector2(
                (float)textureCoords.x * Globals.TEXTURE_STEP_FACTOR.X,
                (float)textureCoords.y * Globals.TEXTURE_STEP_FACTOR.Y);
        }
        public static Block ToNewBlock(this BlockTypes type)
        {
            return type switch
            {
                BlockTypes.Dirt or
                BlockTypes.Stone or
                BlockTypes.Grass => new SolidBlock(type),
                BlockTypes.Water => new Liquid(type, Liquid.DEFAULT_WATER_SPREAD),
                BlockTypes.Air => new Air(),
                _ => throw new NotImplementedException($"Tried creating new block with an unknown blocktype {type.ToString()}")
            };
        }
    }
}
