using mc_clone.src.WorldData.Blocks.Types;
using Microsoft.Xna.Framework;
using System;

namespace mc_clone.src.WorldData.Blocks
{
    public enum BlockType
    {
        Stone,
        Dirt,
        Grass,
        Water,
    }

    public static class BlockTypesExtension
    {
        public static Vector2 ToUV(this BlockType type, CardinalDirection direction)
        {
            (int x, int y) textureCoords = type switch
            {
                BlockType.Stone => (1, 0),
                BlockType.Dirt => (2, 0),
                BlockType.Grass => direction switch
                {
                    CardinalDirection.Top => (0, 0),
                    CardinalDirection.Bottom => (2, 0),
                    _ => (3, 0)
                },
                BlockType.Water => (14, 0),
                _ => throw new NotImplementedException($"Tried getting texture coords for an unknown blocktype {type.ToString()}")
            };
            return new Vector2(
                (float)textureCoords.x * Globals.TEXTURE_STEP_FACTOR.X,
                (float)textureCoords.y * Globals.TEXTURE_STEP_FACTOR.Y);
        }
        public static Block ToNewBlock(this BlockType type)
        {
            return type switch
            {
                BlockType.Dirt or
                BlockType.Stone or
                BlockType.Grass => new SolidBlock(type),
                BlockType.Water => new Liquid(type, Liquid.DEFAULT_WATER_SPREAD),
                _ => throw new NotImplementedException($"Tried creating new block with an unknown blocktype {type.ToString()}")
            };
        }
    }
}
