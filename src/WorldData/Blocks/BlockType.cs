using mc_clone.src.WorldData.Blocks.Behaviors;
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
            (int x, int y) = type switch
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
                (float)x * Globals.TEXTURE_STEP_FACTOR.X,
                (float)y * Globals.TEXTURE_STEP_FACTOR.Y);
        }

        public static bool IsLiquid(this BlockType blockType)
        {
            return blockType switch
            {
                BlockType.Water => true,
                _ => false,
            };
        }
    }
}
