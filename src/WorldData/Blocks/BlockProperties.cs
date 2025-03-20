using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using mc_clone.src.WorldData.Blocks.Behaviors;

namespace mc_clone.src.WorldData.Blocks
{
    public class BlockProperties
    {
        public bool isSolid = true;
        public IBlockBehavior behavior;
        public BlockData dataObject;
        public Effect effect = Globals.basicEffect;
    }

    public static class BlockPropertyRegistry
    {
        public static readonly Dictionary<BlockType, BlockProperties> Properties = new()
        {
            { BlockType.Stone, new BlockProperties { } },
            { BlockType.Grass, new BlockProperties { } },
            { BlockType.Dirt,  new BlockProperties { } },
            // There might be a problem that multiple blocks use the same BlockData object here?
            { BlockType.Water, new BlockProperties {
                isSolid = false,
                behavior = new LiquidBehavior(),
                dataObject = new LiquidData(),
                effect = Globals.waterEffect,
            } },
        };

        public static BlockProperties Get(BlockQuery query)
        {
            return Properties[query.Block.Type];
        }
        public static BlockProperties Get(Block block)
        {
            return Properties[block.Type];
        }

        public static BlockProperties Get(BlockType type)
        {
            return Properties[type];
        }
    }
}
