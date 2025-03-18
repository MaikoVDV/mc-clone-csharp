using System.Collections.Generic;

namespace mc_clone.src.WorldData.Blocks
{
    public delegate void BlockUpdateAction(World world, BlockCoordinates coords);

    public class BlockProperties
    {
        public bool isSolid = true;
        public BlockUpdateAction updateAction;

        public BlockProperties(bool isSolid = true, BlockUpdateAction updateAction = null)
        {
            this.isSolid = isSolid;
            this.updateAction = updateAction;
        }
    }

    public static class BlockPropertyRegistry
    {
        public static Dictionary<BlockType, BlockProperties> Properties = new()
        {
            { BlockType.Stone, new BlockProperties() },
            { BlockType.Grass, new BlockProperties() },
            { BlockType.Dirt, new BlockProperties() },
            { BlockType.Water, new BlockProperties(false) },
        };
    }
}
