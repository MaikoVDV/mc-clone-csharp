namespace mc_clone.src.WorldData.Blocks.Types
{
    public class Liquid : Block
    {
        public static readonly int DEFAULT_WATER_SPREAD = 4;

        private int spreadLevel;
        public Liquid(BlockTypes type, int spreadLevel) : base(type)
        {
            this.spreadLevel = spreadLevel;
        }

        /// TODO: Clean this up lmao
        public override void Update(World world, BlockCoordinates coords)
        {
            var neighbors = GetNeighborBlocks(world, coords);
            Block below = neighbors[BlockFaceDirection.Bottom];
            if (below is Air)
            {
                BlockCoordinates belowCoords = GetNeighborCoordinates(coords)[BlockFaceDirection.Bottom];
                world.AddBlock(belowCoords, new Liquid(BlockTypes.Water, Liquid.DEFAULT_WATER_SPREAD));
            }

            if (spreadLevel > 0)
            {
                var sideNeighbors = GetNeighborBlocks(world, coords);
                sideNeighbors.Remove(BlockFaceDirection.Bottom);
                sideNeighbors.Remove(BlockFaceDirection.Top);
                foreach (var sideNeighbor in sideNeighbors)
                {
                    BlockCoordinates sideCoords = GetNeighborCoordinates(coords)[sideNeighbor.Key];
                    if (world.GetBlock(sideCoords) is Air)
                    {
                        BlockCoordinates belowSideCoords = sideCoords + new BlockCoordinates(0, -1, 0);
                        Block belowSide = world.GetBlock(belowSideCoords);
                        if (belowSide is not Air && belowSide is not null && belowSide is not Liquid)
                        {
                            // Side is empty and block below it isn't, so fill it with water.
                            world.AddBlock(sideCoords, new Liquid(BlockTypes.Water, spreadLevel - 1));
                        }
                    }
                }
            }
        }
    }
}
