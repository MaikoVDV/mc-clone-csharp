namespace mc_clone.src.WorldData.Blocks.Behaviors
{
    public interface IBlockBehavior
    {
        public void SelfChangedUpdateHandler(World world, BlockCoordinates coords);
        public void NeighborChangedUpdateHandler(World world, BlockCoordinates coords);
    }
}
