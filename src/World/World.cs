using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace mc_clone
{
    public partial class World
    {
        private Dictionary<ChunkCoordinates, Chunk> chunks = new();
        private Dictionary<ChunkCoordinates, (VertexBuffer vertexBuffer, IndexBuffer indexBuffer)> chunkMeshes = new();
        private List<ChunkCoordinates> chunksToUpdate = new();
        private BasicEffect chunkEffect;

        public World(GraphicsDevice graphicsDevice, Texture2D textureAtlas)
        {

            // Initialize BasicEffect
            chunkEffect = new BasicEffect(graphicsDevice)
            {
                Texture = textureAtlas,
                TextureEnabled = true,
                LightingEnabled = false
            };
            chunks.Add(ChunkCoordinates.Zero, new Chunk(ChunkCoordinates.Zero));
            chunks.Add(new ChunkCoordinates(1, 0, 0), new Chunk(new ChunkCoordinates(1, 0, 0)));
            chunks.Add(new ChunkCoordinates(0, 1, 0), new Chunk(new ChunkCoordinates(0, 1, 0)));
            chunks.Add(new ChunkCoordinates(0, 0, 1), new Chunk(new ChunkCoordinates(0, 0, 1)));
            chunks.Add(new ChunkCoordinates(0, 1, 1), new Chunk(new ChunkCoordinates(0, 1, 1), false));

            foreach (KeyValuePair<ChunkCoordinates, Chunk> chunkEntry in chunks)
            {
                RegenerateChunkMesh(graphicsDevice, chunkEntry.Key);
            }
        }


        public void Draw(GraphicsDevice graphicsDevice, Camera camera)
        {
            (Matrix view, Matrix projection) = camera.Matrices;

            chunkEffect.View = view;
            chunkEffect.Projection = projection;

            foreach ((ChunkCoordinates coords, var mesh) in chunkMeshes)
            {
                // Load faces
                graphicsDevice.SetVertexBuffer(mesh.vertexBuffer);
                graphicsDevice.Indices = mesh.indexBuffer;

                chunkEffect.World = Matrix.CreateTranslation(coords.X * Globals.CHUNK_SIZE_XZ, coords.Y * Globals.CHUNK_SIZE_Y, coords.Z * Globals.CHUNK_SIZE_XZ);

                // Apply effect and draw
                foreach (EffectPass pass in chunkEffect.CurrentTechnique.Passes)
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
                        if (GetBlock(x, y, z) is Block _)
                        {
                            output.Add(new BlockCoordinates(x, y, z));
                        }
                    }
                }
            }
            return output.ToArray();
        }

        public Block GetBlock(BlockCoordinates globalCoords)
        {
            (ChunkCoordinates chunkCoords, LocalBlockCoordinates localCoords) = LocalBlockCoordinates.FromGlobal(globalCoords);

            if (chunks.TryGetValue(chunkCoords, out var chunk))
            {
                return chunks[chunkCoords].GetBlock(localCoords);
            }
            return null;
        }
        public Block GetBlock(int x, int y, int z) { return GetBlock(new BlockCoordinates(x, y, z)); }
    }
}
