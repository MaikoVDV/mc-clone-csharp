using mc_clone.src.Entities.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using mc_clone.src.WorldData.Blocks;
using System.Diagnostics;
using System.Linq;
using mc_clone.src.WorldData.Blocks.Behaviors;

namespace mc_clone.src.WorldData
{
    public partial class World
    {
        private Dictionary<ChunkCoordinates, Chunk> chunks = new();
        private Dictionary<ChunkCoordinates, (VertexBuffer vertexBuffer, IndexBuffer indexBuffer)> chunkMeshes = new();
        private readonly BasicEffect solidBlockEffect;

        public World(GraphicsDevice graphicsDevice, Texture2D textureAtlas)
        {
            // Initialize BasicEffect
            solidBlockEffect = new BasicEffect(graphicsDevice)
            {
                Texture = textureAtlas,
                TextureEnabled = true,
                LightingEnabled = false
            };
            chunks.Add(ChunkCoordinates.Zero, new Chunk(ChunkCoordinates.Zero));
            chunks.Add(new ChunkCoordinates(1, 0, 0), new Chunk(new ChunkCoordinates(1, 0, 0)));
            chunks.Add(new ChunkCoordinates(1, 1, 0), new Chunk(new ChunkCoordinates(1, 0, 0), false));
            chunks.Add(new ChunkCoordinates(0, 1, 0), new Chunk(new ChunkCoordinates(0, 1, 0)));
            chunks.Add(new ChunkCoordinates(0, 0, 1), new Chunk(new ChunkCoordinates(0, 0, 1)));
            chunks.Add(new ChunkCoordinates(0, 1, 1), new Chunk(new ChunkCoordinates(0, 1, 1), false));
            chunks.Add(new ChunkCoordinates(1, 0, 1), new Chunk(new ChunkCoordinates(0, 1, 0)));
            chunks.Add(new ChunkCoordinates(1, 1, 1), new Chunk(new ChunkCoordinates(0, 1, 0), false));

            foreach (KeyValuePair<ChunkCoordinates, Chunk> chunkEntry in chunks)
            {
                RegenerateChunkMesh(graphicsDevice, chunkEntry.Key);
            }
        }


        public void Draw(GraphicsDevice graphicsDevice, Camera camera)
        {
            (Matrix view, Matrix projection) = camera.Matrices;

            solidBlockEffect.View = view;
            solidBlockEffect.Projection = projection;

            foreach ((ChunkCoordinates coords, var mesh) in chunkMeshes)
            {
                if (mesh.vertexBuffer == null || mesh.indexBuffer == null) continue;
                // Load faces
                graphicsDevice.SetVertexBuffer(mesh.vertexBuffer);
                graphicsDevice.Indices = mesh.indexBuffer;

                solidBlockEffect.World = Matrix.CreateTranslation(coords.X * Globals.CHUNK_SIZE_XZ, coords.Y * Globals.CHUNK_SIZE_Y, coords.Z * Globals.CHUNK_SIZE_XZ);

                // Apply effect and draw
                foreach (EffectPass pass in solidBlockEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.indexBuffer.IndexCount / 3);
                }
            }
        }
        public BlockCoordinates[] GetBlocksInAABB(Vector3 min, Vector3 max)
        {
            List<BlockCoordinates> output = new();
            BlockCoordinates minCoords = new BlockCoordinates(min);
            BlockCoordinates maxCoords = new BlockCoordinates(max);
            for (int y = minCoords.Y; y < maxCoords.Y; y++)
            {
                for (int z = minCoords.Z; z < maxCoords.Z; z++)
                {
                    for (int x = minCoords.X; x < maxCoords.X; x++)
                    {
                        Block currentBlock = GetBlock(x, y, z).Block;
                        if (currentBlock != null && currentBlock?.Type != BlockType.Water)
                        {
                            output.Add(new BlockCoordinates(x, y, z));
                        }
                    }
                }
            }
            return output.ToArray();
        }

        public BlockQuery GetBlock(BlockCoordinates globalCoords)
        {
            (ChunkCoordinates chunkCoords, LocalBlockCoordinates localCoords) = LocalBlockCoordinates.FromGlobal(globalCoords);

            if (chunks.TryGetValue(chunkCoords, out var chunk))
            {
                return chunks[chunkCoords].GetBlock(localCoords);
            }
            //Debug.WriteLine("Null");
            return new BlockQuery(null, null);
        }

        public BlockQuery GetBlock(int x, int y, int z) { return GetBlock(new BlockCoordinates(x, y, z)); }

        public BlockQuery[] GetBlocks(BlockCoordinates[] globalCoordsArray) 
        {
            return globalCoordsArray.Select(coords =>
            {
                return GetBlock(coords);
            }).ToArray();
        }

        //public bool TryGetBlock(BlockCoordinates globalCoords, out BlockQuery queryResult)
        //{
        //    (ChunkCoordinates chunkCoords, LocalBlockCoordinates localCoords) = LocalBlockCoordinates.FromGlobal(globalCoords);

        //    if (chunks.TryGetValue(chunkCoords, out var chunk))
        //    {
        //        queryResult = chunks[chunkCoords].GetBlock(localCoords);
        //        Debug.WriteLine(queryResult == null);
        //        return queryResult != null;
        //    }
        //    queryResult = null;
        //    return false;
        //}
    }
}
