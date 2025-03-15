using Microsoft.Xna.Framework.Graphics;
using System;

using mc_clone.src.WorldData.Blocks;
using mc_clone.src.WorldData.Blocks.Types;
using System.Linq;
using System.Collections.Generic;
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

                chunksToUpdate.Add(chunkCoords);
                // Get neighboring chunks to update.
                chunksToUpdate.AddRange(block.GetNeighborCoordinates(globalCoords)
                    .Select(neighborCoords => neighborCoords.ToChunkCoordinates()));
            } else
            {
                Debug.WriteLine($"Tried setting a block at {globalCoords}, but failed to find it. Coordinates are probably not in a loaded chunk.");
            }
        }

        public void RemoveBlock(BlockCoordinates coords) => SetBlock(coords, new Air());

        public void AddBlock(BlockCoordinates coords, BlockTypes type) => SetBlock(coords, new SolidBlock(type));

        public void Update(GraphicsDevice graphicsDevice)
        {
            chunksToUpdate.Distinct(); // Remove duplicates to prevent unnecessary work.
            foreach (ChunkCoordinates coords in chunksToUpdate)
            {
                RegenerateChunkMesh(graphicsDevice, coords);
            }
            chunksToUpdate.Clear();
        }
    }
}
