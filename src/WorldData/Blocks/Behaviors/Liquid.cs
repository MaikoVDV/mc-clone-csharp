using System.Diagnostics;
using System.Linq;

namespace mc_clone.src.WorldData.Blocks.Behaviors
{
    public class LiquidBehavior : IBlockBehavior
    {
        public void SelfChangedUpdateHandler(World world, BlockCoordinates coords)
        {
            TrySpread(world, coords);
        }
        public void NeighborChangedUpdateHandler(World world, BlockCoordinates coords)
        {
            TrySpread(world, coords);
        }

        // Tries to spread water
        static void TrySpread(World world, BlockCoordinates coords)
        {
            BlockQuery thisBlock = world.GetBlock(coords);

            var neighbors = coords.GetNeighborBlocks(world);
            Block below = neighbors[CardinalDirection.Bottom].Block;
            // Spread down
            if (below is null)
            {
                BlockCoordinates belowCoords = coords.GetNeighborCoordinates()[CardinalDirection.Bottom];
                world.AddBlock(belowCoords, BlockType.Water, new LiquidData());
            }
            // Spread to sides
            if (thisBlock.GetBlockData<LiquidData>()?.spreadLevel > 0)
            {
                var sideNeighbors = neighbors
                    .Where(neighbor => 
                    neighbor.Key != CardinalDirection.Bottom && neighbor.Key != CardinalDirection.Top);

                foreach (var sideNeighbor in sideNeighbors)
                {
                    BlockCoordinates sideCoords = coords.GetNeighborCoordinates()[sideNeighbor.Key];
                    if (world.GetBlock(sideCoords).Block == null)
                    {
                        Block belowCurrent = neighbors[CardinalDirection.Bottom].Block;
                        if (belowCurrent != null && !belowCurrent.Type.IsLiquid())
                        {
                            // Side is empty and block below it isn't, so fill it with water.
                            world.AddBlock(sideCoords, BlockType.Water, new LiquidData(world.GetBlock(coords).GetBlockData<LiquidData>().spreadLevel - 1));
                        }
                    }
                }
            }
        }
    }

    public class LiquidData : BlockData
    {
        public static readonly int DEFAULT_WATER_SPREAD = 4;
        public int spreadLevel;

        public LiquidData(int spreadLevel)
        {
            this.spreadLevel = spreadLevel;
        }
        public LiquidData()
        {
            this.spreadLevel = DEFAULT_WATER_SPREAD;
        }
    }
}
