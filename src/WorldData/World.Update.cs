using Microsoft.Xna.Framework.Graphics;

using mc_clone.src.WorldData.Blocks;
using mc_clone.src.WorldData.Blocks.Types;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace mc_clone.src.WorldData
{
    public partial class World
    {
        private List<ChunkCoordinates> chunkUpdateBuffer = new();
        private List<(BlockCoordinates, Block)> blockSetUpdateBuffer = new();
        private List<BlockCoordinates> neighborChangedUpdateBuffer = new();

        public void Update(GraphicsDevice graphicsDevice)
        {
            // Update blocks which are due to be changed. (placed / removed)
            List<(BlockCoordinates, Block)> _blockSetUpdateBuffer = blockSetUpdateBuffer;
            blockSetUpdateBuffer = new();

            _blockSetUpdateBuffer = _blockSetUpdateBuffer.Distinct().ToList(); // Remove duplicates to prevent unnecessary work.
            foreach ((BlockCoordinates coords, Block block) blockUpdate in _blockSetUpdateBuffer)
            {
                SetBlock(blockUpdate.coords, blockUpdate.block);
            }


            // Update blocks which had neighbors changed.
            List<BlockCoordinates> _neighborUpdateBuffer = neighborChangedUpdateBuffer;
            neighborChangedUpdateBuffer = new();

            _neighborUpdateBuffer = _neighborUpdateBuffer.Distinct().ToList(); // Remove duplicates to prevent unnecessary work.
            foreach (BlockCoordinates blockCoords in _neighborUpdateBuffer)
            {
                Block tryUpdate = GetBlock(blockCoords);
                tryUpdate?.NeighborChangedUpdateHandler(this, blockCoords);
            }

            // Chunk meshing updates
            chunkUpdateBuffer = chunkUpdateBuffer.Distinct().ToList(); // Remove duplicates to prevent unnecessary work.
            foreach (ChunkCoordinates coords in chunkUpdateBuffer)
            {
                RegenerateChunkMesh(graphicsDevice, coords);
            }
            chunkUpdateBuffer.Clear();


            // Sets the block and adds necessary updates to queue to be handled next tick.
            // Function is declared within Update() scope to prevent access from outside,
            // any block updates must be handled through the update buffers. (blockSetUpdateBuffer, neighborChangedUpdateBuffer)
            void SetBlock(BlockCoordinates globalCoords, Block block)
            {
                (ChunkCoordinates chunkCoords, LocalBlockCoordinates localCoords) = LocalBlockCoordinates.FromGlobal(globalCoords);

                if (chunks.TryGetValue(chunkCoords, out Chunk chunk))
                {
                    chunks[chunkCoords].SetBlock(localCoords, block); // Update the data stored in chunk.
                    chunkUpdateBuffer.Add(chunkCoords);
                    if (TryGetBlock(globalCoords, out Block placedBlock))
                    {
                        placedBlock.SelfChangedUpdateHandler(this, globalCoords);
                    }

                    neighborChangedUpdateBuffer.Add(globalCoords);
                    foreach (var neighborKvp in globalCoords.GetNeighborCoordinates())
                    {
                        if (neighborKvp.Value != null) neighborChangedUpdateBuffer.Add(neighborKvp.Value);
                    }

                    // Get neighboring chunks to update.
                    chunkUpdateBuffer.AddRange(globalCoords.GetNeighborCoordinates()
                        .Select(neighborKvp => neighborKvp.Value.ToChunkCoordinates()));
                }
                else
                {
                    Debug.WriteLine($"Tried setting a block at {globalCoords}, but failed to find it. Coordinates are probably not in a loaded chunk.");
                }
            }
        }

        public void AddSetBlockUpdate(BlockCoordinates coords, Block block)
        {
            blockSetUpdateBuffer.Add((coords, block));
        }

        public void RemoveBlock(BlockCoordinates coords) => AddSetBlockUpdate(coords, null);

        public void AddBlock(BlockCoordinates coords, Block block) => AddSetBlockUpdate(coords, block);
        public void AddBlock(BlockCoordinates coords, BlockType type) => AddSetBlockUpdate(coords, type.ToNewBlock());
    }
}
