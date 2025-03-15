using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mc_clone
{
    public partial class World
    {
        public void SetBlock(BlockCoordinates globalCoords, Block block)
        {
            (ChunkCoordinates chunkCoords, LocalBlockCoordinates blockCoords) = LocalBlockCoordinates.FromGlobal(globalCoords);

            if (chunks.TryGetValue(chunkCoords, out var chunk))
            {
                chunks[chunkCoords].SetBlock(blockCoords, block);
                chunksToUpdate.Add(chunkCoords);
            }
        }

        public void RemoveBlock(BlockCoordinates coords) => SetBlock(coords, null);

        public void AddBlock(BlockCoordinates coords, BlockTypes type) => SetBlock(coords, new SolidBlock(type));

        public void RegenerateChunkMesh(GraphicsDevice graphicsDevice, ChunkCoordinates coords)
        {
            (VertexPositionTexture[] vertices, int[] indices) = chunks[coords].BuildMesh(this);
            if (vertices.Length == 0 || indices.Length == 0) return;
            VertexBuffer vb = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), vertices.Length, BufferUsage.WriteOnly);
            IndexBuffer ib = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
            vb.SetData(vertices);
            ib.SetData(indices);
            chunkMeshes[coords] = (vb, ib);
        }

        public void Update(GraphicsDevice graphicsDevice)
        {
            foreach (ChunkCoordinates coords in chunksToUpdate)
            {
                RegenerateChunkMesh(graphicsDevice, coords);
            }
            chunksToUpdate.Clear();
        }
    }
}
