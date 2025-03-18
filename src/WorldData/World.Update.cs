using Microsoft.Xna.Framework.Graphics;

using mc_clone.src.WorldData.Blocks;
using mc_clone.src.WorldData.Blocks.Types;
using System.Linq;
using System.Diagnostics;

namespace mc_clone.src.WorldData
{
    public partial class World
    {
        public void SetBlock(BlockCoordinates globalCoords, Block block)
        {
            (ChunkCoordinates chunkCoords, LocalBlockCoordinates blockCoords) = LocalBlockCoordinates.FromGlobal(globalCoords);

            if (chunks.TryGetValue(chunkCoords, out Chunk chunk))
            {
                chunks[chunkCoords].SetBlock(blockCoords, block); // Update the data stored in chunk.
                //GetBlock(globalCoords).Update(this, globalCoords);
                futureBlocksToUpdate.Add(globalCoords);
                futureBlocksToUpdate.AddRange(block.GetNeighborCoordinates(globalCoords)
                    .Select(neighborKvp => neighborKvp.Value));

                chunksToUpdate.Add(chunkCoords);
                // Get neighboring chunks to update.
                chunksToUpdate.AddRange(block.GetNeighborCoordinates(globalCoords)
                    .Select(neighborKvp => neighborKvp.Value.ToChunkCoordinates()));
            } else
            {
                Debug.WriteLine($"Tried setting a block at {globalCoords}, but failed to find it. Coordinates are probably not in a loaded chunk.");
            }
        }

        public void RemoveBlock(BlockCoordinates coords) => SetBlock(coords, new Air());

        public void AddBlock(BlockCoordinates coords, Block block) => SetBlock(coords, block);
        public void AddBlock(BlockCoordinates coords, BlockTypes type) => SetBlock(coords, type.ToNewBlock());

        public void Update(GraphicsDevice graphicsDevice)
        {
            blocksToUpdate = futureBlocksToUpdate;
            futureBlocksToUpdate = new();

            // Block updates
            blocksToUpdate.Distinct(); // Remove duplicates to prevent unnecessary work.
            foreach (BlockCoordinates blockCoords in blocksToUpdate)
            {
                Block tryUpdate = GetBlock(blockCoords);
                if (tryUpdate != null) tryUpdate.Update(this, blockCoords);
            }
            blocksToUpdate.Clear();

            // Chunk meshing updates
            chunksToUpdate.Distinct(); // Remove duplicates to prevent unnecessary work.
            foreach (ChunkCoordinates coords in chunksToUpdate)
            {
                RegenerateChunkMesh(graphicsDevice, coords);
            }
            chunksToUpdate.Clear();
        }
    }
}
