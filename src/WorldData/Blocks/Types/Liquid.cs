namespace mc_clone.src.WorldData.Blocks.Types
{
    public class Liquid : Block
    {
        public static readonly int DEFAULT_WATER_SPREAD = 4;

        private int spreadLevel;
        public Liquid(BlockType type, int spreadLevel) : base(type)
        {
            this.spreadLevel = spreadLevel;
        }

        /// TODO: Clean this up lmao
        public override void SelfChangedUpdateHandler(World world, BlockCoordinates coords)
        {
            TrySpread(world, coords);
        }
        public override void NeighborChangedUpdateHandler(World world, BlockCoordinates coords)
        {
            TrySpread(world, coords);
        }

        // Tries to spread water
        void TrySpread(World world, BlockCoordinates coords)
        {
            var neighbors = GetNeighborBlocks(world, coords);
            Block below = neighbors[CardinalDirection.Bottom];
            if (below is null)
            {
                BlockCoordinates belowCoords = coords.GetNeighborCoordinates()[CardinalDirection.Bottom];
                world.AddBlock(belowCoords, new Liquid(BlockType.Water, Liquid.DEFAULT_WATER_SPREAD));
            }

            if (spreadLevel > 0)
            {
                var sideNeighbors = GetNeighborBlocks(world, coords);
                sideNeighbors.Remove(CardinalDirection.Bottom);
                sideNeighbors.Remove(CardinalDirection.Top);
                foreach (var sideNeighbor in sideNeighbors)
                {
                    BlockCoordinates sideCoords = coords.GetNeighborCoordinates()[sideNeighbor.Key];
                    if (world.GetBlock(sideCoords) is null)
                    {
                        BlockCoordinates belowSideCoords = sideCoords + new BlockCoordinates(0, -1, 0);
                        Block belowSide = world.GetBlock(belowSideCoords);
                        if (belowSide != null && belowSide is not Liquid)
                        {
                            // Side is empty and block below it isn't, so fill it with water.
                            world.AddBlock(sideCoords, new Liquid(BlockType.Water, spreadLevel - 1));
                        }
                    }
                }
            }
        }
    }
}
