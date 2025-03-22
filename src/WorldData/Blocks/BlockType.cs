using mc_clone.src.WorldData.Blocks.Behaviors;
using Microsoft.Xna.Framework;
using System;

namespace mc_clone.src.WorldData.Blocks
{
    public delegate bool BlockTypeFilterClause(BlockType type);

    public enum BlockType
    {
        Stone,
        Dirt,
        Grass,
        Glass,
        Water,
    }

    public static class BlockTypesExtension
    {
        public static Vector2 ToUV(this BlockType type, CardinalDirection direction)
        {
            (int x, int y) = type switch
            {
                BlockType.Stone => (19, 0),
                BlockType.Dirt => (18, 1),
                BlockType.Grass => direction switch
                {
                    CardinalDirection.Top => (2, 0),
                    CardinalDirection.Bottom => (18, 1),
                    _ => (3, 0)
                },
                BlockType.Glass => (24, 4),
                BlockType.Water => (13, 11),
                _ => throw new NotImplementedException($"Tried getting texture coords for an unknown blocktype {type.ToString()}")
            };
            return new Vector2(
                (float)x * Globals.TEXTURE_STEP_FACTOR.X,
                (float)y * Globals.TEXTURE_STEP_FACTOR.Y);
        }
        public static float ToMaterialId(this BlockType type, CardinalDirection direction)
        {
            int id = type switch
            {
                BlockType.Water => 1,
                _ => 0
            };
            return (float)id;
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
