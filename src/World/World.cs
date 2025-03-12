using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace mc_clone
{
    internal class World
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
            chunks.Add(ChunkCoordinates.Zero, new Chunk());
            chunks.Add(new ChunkCoordinates(1, 0, 0), new Chunk());
            chunks.Add(new ChunkCoordinates(0, 1, 0), new Chunk());
            chunks.Add(new ChunkCoordinates(0, 0, 1), new Chunk());
            chunks.Add(new ChunkCoordinates(0, 1, 1), new Chunk(false));

            foreach (KeyValuePair<ChunkCoordinates, Chunk> chunkEntry in chunks)
            {
                RegenerateChunkMesh(graphicsDevice, chunkEntry.Key);
            }
        }

        public void SetBlock(BlockCoordinates coords, Block block)
        {
            ChunkCoordinates chunkCoords = new ChunkCoordinates(
                (int)MathF.Floor(coords.X / (float)Globals.CHUNK_SIZE_XZ),
                (int)MathF.Floor(coords.Y / (float)Globals.CHUNK_SIZE_Y),
                (int)MathF.Floor(coords.Z / (float)Globals.CHUNK_SIZE_XZ));

            BlockCoordinates blockCoords = new BlockCoordinates(
                coords.X % Globals.CHUNK_SIZE_XZ,
                coords.Y % Globals.CHUNK_SIZE_Y,
                coords.Z % Globals.CHUNK_SIZE_XZ);

            if (chunks.TryGetValue(chunkCoords, out var chunk))
            {
                chunks[chunkCoords].SetBlock(blockCoords, block);
                chunksToUpdate.Add(chunkCoords);
            }
        }

        public void RemoveBlock(BlockCoordinates coords) => SetBlock(coords, null);

        public void AddBlock(BlockCoordinates coords, BlockTypes type) => SetBlock(coords, new Block(type));

        // DDA algorithm basedon https://lodev.org/cgtutor/raycasting.html
        public Nullable<(Block block, BlockFaceDirection, Vector3 hitPoint, BlockCoordinates coords)> CastRay(Ray ray, float maxDistance = float.MaxValue)
        {
            BlockCoordinates gridCoords = new BlockCoordinates(ray.Position);

            Vector3 stepDirection = new Vector3(Math.Sign(ray.Direction.X), Math.Sign(ray.Direction.Y), Math.Sign(ray.Direction.Z));

            // Length of ray needed to travel to the next grid block for each axis.
            // Check for 0 is just to prevent dividing by zero.
            Vector3 deltaDist = new Vector3(
                ray.Direction.X == 0 ? float.MaxValue : MathF.Abs(1 / ray.Direction.X),
                ray.Direction.Y == 0 ? float.MaxValue : MathF.Abs(1 / ray.Direction.Y),
                ray.Direction.Z == 0 ? float.MaxValue : MathF.Abs(1 / ray.Direction.Z));

            Vector3 sideDist = new Vector3(
                (stepDirection.X > 0 ? (gridCoords.X + 1f - ray.Position.X) : (ray.Position.X - gridCoords.X)) * deltaDist.X,
                (stepDirection.Y > 0 ? (gridCoords.Y + 1f - ray.Position.Y) : (ray.Position.Y - gridCoords.Y)) * deltaDist.Y,
                (stepDirection.Z > 0 ? (gridCoords.Z + 1f - ray.Position.Z) : (ray.Position.Z - gridCoords.Z)) * deltaDist.Z
                );

            bool hit = false;
            char side;
            BlockFaceDirection faceDir;

            float distanceTravelled = 0;

            int index = 0;
            while (!hit && index < 10)
            {
                float prevDist = distanceTravelled;
                index++;
                if (GetBlock(gridCoords) != null && index == 0)
                {
                    // Started DDA search inside a block.
                    Debug.WriteLine("Started DDA search from inside a block");
                    return null;
                }

                if (sideDist.Min() == sideDist.X)
                {
                    distanceTravelled = sideDist.X;
                    sideDist.X += deltaDist.X;
                    gridCoords += new Vector3(stepDirection.X, 0, 0);
                    side = 'x';
                } else if (sideDist.Min() == sideDist.Y)
                {
                    distanceTravelled = sideDist.Y;
                    sideDist.Y += deltaDist.Y;
                    gridCoords += new Vector3(0, stepDirection.Y, 0);
                    side = 'y';
                } else if (sideDist.Min() == sideDist.Z)
                {
                    distanceTravelled = sideDist.Z;
                    sideDist.Z += deltaDist.Z;
                    gridCoords += new Vector3(0, 0, stepDirection.Z);
                    side = 'z';
                } else
                {
                    throw new Exception("Error in DDA: Found now lowest value for side dists.");
                }

                switch (side)
                {
                    case 'x':
                        faceDir = stepDirection.X == 1 ? BlockFaceDirection.East : BlockFaceDirection.West;
                        break;
                    case 'y':
                        faceDir = stepDirection.Y == 1 ? BlockFaceDirection.Bottom : BlockFaceDirection.Top;
                        break;
                    case 'z':
                        faceDir = stepDirection.Z == 1 ? BlockFaceDirection.South : BlockFaceDirection.North;
                        break;
                    default: throw new Exception("wat");
                }


                Vector3 currentRayPoint = ray.Position + (sideDist.Min() - deltaDist.Min()) * ray.Direction;
                if (distanceTravelled >= maxDistance) return null;
                Block hitBlock = GetBlock(gridCoords);
                if (hitBlock != null)
                {
                    hit = true;
                    return (hitBlock, 
                        faceDir,
                        currentRayPoint,
                        gridCoords);
                }
            }

            return null;
        }

        public void RegenerateChunkMesh(GraphicsDevice graphicsDevice, ChunkCoordinates coords)
        {
            (VertexPositionTexture[] vertices, int[] indices) = chunks[coords].BuildMesh();
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

        public Block GetBlock(BlockCoordinates globalCoords) {
            ChunkCoordinates chunkCoords = new ChunkCoordinates(
                globalCoords.X / Globals.CHUNK_SIZE_XZ,
                globalCoords.Y / Globals.CHUNK_SIZE_Y,
                globalCoords.Z / Globals.CHUNK_SIZE_XZ);

            BlockCoordinates localCoords = new BlockCoordinates(
                globalCoords.X % Globals.CHUNK_SIZE_XZ,
                globalCoords.Y % Globals.CHUNK_SIZE_Y,
                globalCoords.Z % Globals.CHUNK_SIZE_XZ);

            if (chunks.TryGetValue(chunkCoords, out var chunk))
            {
                return chunks[chunkCoords].GetBlock(localCoords);
            }
            return null;
        }
        public Block GetBlock(int x, int y, int z) { return GetBlock(new BlockCoordinates(x, y, z)); }
    }
}
