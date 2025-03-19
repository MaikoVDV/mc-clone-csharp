using mc_clone.src.WorldData.Blocks.Behaviors;
using System;
using System.Collections.Generic;

namespace mc_clone.src.WorldData.Blocks
{
    public delegate void BlockUpdateAction(World world, BlockCoordinates coords);

    public class BlockProperties
    {
        public bool isSolid = true;
        public IBlockBehavior behavior;
        public BlockData dataObject;

        public BlockProperties(bool isSolid = true, IBlockBehavior behavior = null)
        {
            this.isSolid = isSolid;
            this.behavior = behavior;
        }
    }

    public static class BlockPropertyRegistry
    {
        public static readonly Dictionary<BlockType, BlockProperties> Properties = new()
        {
            { BlockType.Stone, new BlockProperties { } },
            { BlockType.Grass, new BlockProperties { } },
            { BlockType.Dirt,  new BlockProperties { } },
            // There might be a problem that multiple blocks use the same BlockData object here?
            { BlockType.Water, new BlockProperties { behavior = new LiquidBehavior(), dataObject = new LiquidData() } },
        };

        public static BlockProperties Get(BlockQuery query)
        {
            return Properties[query.Block.Type];
        }
        public static BlockProperties Get(Block block)
        {
            return Properties[block.Type];
        }
    }
}
