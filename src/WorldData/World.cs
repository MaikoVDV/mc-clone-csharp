using mc_clone.src.Entities.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using mc_clone.src.WorldData.Blocks;
using System.Diagnostics;
using System.Linq;
using mc_clone.src.WorldData.Blocks.Behaviors;
using System.Security.Cryptography.X509Certificates;

namespace mc_clone.src.WorldData
{
    public partial class World
    {
        private Dictionary<ChunkCoordinates, Chunk> chunks = new();
        private Texture2D textureAtlas;
        public Player player;

        public World(GraphicsDevice graphicsDevice, Texture2D textureAtlas, Camera playerCam)
        {
            this.textureAtlas = textureAtlas;
            player = new Player(this, playerCam);

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
                RegenerateChunkMeshes(graphicsDevice, chunkEntry.Key);
            }
        }


        public void Draw(GraphicsDevice graphicsDevice)
        {
            (Matrix view, Matrix projection) = player.camera.Matrices;

            // Sort chunks based on distance to camera
            chunkMeshes = chunkMeshes.OrderByDescending(meshKvp => 
                    Vector3.Distance(meshKvp.Key.GetCenter(), player.camera.Position))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            foreach ((var coords, var meshParts) in chunkMeshes)
            {
                if (!chunks.ContainsKey(coords)) continue;

                Matrix world = Matrix.CreateTranslation(
                                coords.X * Globals.CHUNK_SIZE_XZ,
                                coords.Y * Globals.CHUNK_SIZE_Y,
                                coords.Z * Globals.CHUNK_SIZE_XZ);

                foreach (var meshPart in meshParts)
                {
                    if (meshPart.vertexBuffer == null || meshPart.indexBuffer == null) continue;
                    Effect currentEffect = meshPart.effect;

                    currentEffect.Parameters["View"].SetValue(view);
                    currentEffect.Parameters["Projection"].SetValue(projection);
                    currentEffect.Parameters["World"].SetValue(world);

                    // Load faces
                    graphicsDevice.SetVertexBuffer(meshPart.vertexBuffer);
                    graphicsDevice.Indices = meshPart.indexBuffer;


                    // Apply effect and draw
                    foreach (EffectPass pass in currentEffect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, meshPart.indexBuffer.IndexCount / 3);
                    }
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
