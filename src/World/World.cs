using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace mc_clone
{
    internal class World
    {
        private Dictionary<(int x, int y), Chunk> chunks = new();
        private Dictionary<(int x, int y), (VertexBuffer vertexBuffer, IndexBuffer indexBuffer)> chunkMeshes = new();
        private List<(int x, int y)> chunksToUpdate = new();
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
            chunks.Add((0, 0), new Chunk());
            //chunks.Add((1, 0), new Chunk());
            //chunks.Add((0, 1), new Chunk());

            foreach (KeyValuePair<(int x, int y), Chunk> chunkEntry in chunks)
            {
                RegenerateChunkMesh(graphicsDevice, chunkEntry.Key.x, chunkEntry.Key.y);
            }
        }

        public void RemoveBlock((int x, int y, int z) coords)
        {
            (int x, int y) chunkCoords = (coords.x / Globals.CHUNK_SIZE_XZ, coords.z / Globals.CHUNK_SIZE_XZ);
            (int x, int y, int z) blockCoords = (coords.x % Globals.CHUNK_SIZE_XZ, coords.y % Globals.CHUNK_SIZE_Y, coords.z % Globals.CHUNK_SIZE_XZ);
            if (chunks.TryGetValue(chunkCoords, out var chunk))
            {
                chunks[(chunkCoords.x, chunkCoords.y)].SetBlock(blockCoords, null);
                chunksToUpdate.Add(chunkCoords);
                Debug.WriteLine("Removing block");
            }
        }
        public void AddBlock((int x, int y, int z) coords, BlockTypes type)
        {

        }

        // DDA algorithm basedon https://lodev.org/cgtutor/raycasting.html
        public Nullable<(Block block, Vector3 point, (int x, int y, int z) coords)> CastRay(Ray ray)
        {
            var gridCoords = Vec3ToGridCoords(ray.Position);

            // Distances from current point to respective sides of the current grid box.
            float sideDistX, sideDistY, sideDistZ;

            // Length of ray needed to travel to the next grid block for each axis.
            // Check for 0 is just to prevent dividing by zero.
            float deltaDistX = ray.Direction.X == 0 ? float.MaxValue : MathF.Abs(1 / ray.Direction.X);
            float deltaDistY = ray.Direction.Y == 0 ? float.MaxValue : MathF.Abs(1 / ray.Direction.Y);
            float deltaDistZ = ray.Direction.Z == 0 ? float.MaxValue : MathF.Abs(1 / ray.Direction.Z);
            float perpWallDist;

            int stepX, stepY, stepZ;
            bool hit = false;
            char side;

            if (ray.Direction.X < 0)
            {
                stepX = -1;
                sideDistX = (ray.Position.X - gridCoords.x) * deltaDistX;
            } else
            {
                stepX = 1;
                sideDistX = (gridCoords.x + 1f - ray.Position.X) * deltaDistX;
            }
            if (ray.Direction.Y < 0)
            {
                stepY = -1;
                sideDistY = (ray.Position.Y - gridCoords.y) * deltaDistY;
            }
            else
            {
                stepY= 1;
                sideDistY = (gridCoords.y + 1f - ray.Position.Y) * deltaDistY;
            }
            if (ray.Direction.Z < 0)
            {
                stepZ = -1;
                sideDistZ = (ray.Position.Z - gridCoords.z) * deltaDistZ;
            } else
            {
                stepZ = 1;
                sideDistZ = (gridCoords.z + 1f - ray.Position.Z) * deltaDistZ;
            }

            int index = 0;
            while (!hit && index < 10)
            {
                index++;
                float lowestSideDist = new[] { sideDistX, sideDistY, sideDistZ }.Min();
                if (lowestSideDist == sideDistX)
                {
                    sideDistX += deltaDistX;
                    gridCoords.x += stepX;
                    side = 'x';
                } else if (lowestSideDist == sideDistY)
                {
                    sideDistY += deltaDistY;
                    gridCoords.y += stepY;
                    side = 'y';
                } else if (lowestSideDist == sideDistZ)
                {
                    sideDistZ += deltaDistZ;
                    gridCoords.z += stepZ;
                    side = 'z';
                } else
                {
                    throw new Exception("Error in DDA: Found now lowest value for side dists.");
                }
                if (GetBlock(gridCoords.x, gridCoords.y, gridCoords.z) != null)
                {
                    hit = true;
                    return (GetBlock(gridCoords.x, gridCoords.y, gridCoords.z), 
                        new Vector3(sideDistX, sideDistY, sideDistZ),
                        gridCoords);
                }
            }

            return null;
        }

        public void RegenerateChunkMesh(GraphicsDevice graphicsDevice, int x, int y)
        {
            (VertexPositionTexture[] vertices, int[] indices) = chunks[(x, y)].BuildMesh();
            VertexBuffer vb = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), vertices.Length, BufferUsage.WriteOnly);
            IndexBuffer ib = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
            vb.SetData(vertices);
            ib.SetData(indices);
            chunkMeshes[(x, y)] = (vb, ib);
        }

        public void Update(GraphicsDevice graphicsDevice)
        {
            foreach ((int x, int y) chunkCoords in chunksToUpdate)
            {
                Debug.WriteLine("Updating chunk.");
                RegenerateChunkMesh(graphicsDevice, chunkCoords.x, chunkCoords.y);
            }
            chunksToUpdate.Clear();
        }

        public void Draw(GraphicsDevice graphicsDevice, Camera camera)
        {
            (Matrix view, Matrix projection) = camera.Matrices;

            chunkEffect.View = view;
            chunkEffect.Projection = projection;

            foreach ((var coords, var mesh) in chunkMeshes)
            {
                // Load mesh
                graphicsDevice.SetVertexBuffer(mesh.vertexBuffer);
                graphicsDevice.Indices = mesh.indexBuffer;

                chunkEffect.World = Matrix.CreateTranslation(coords.x * Globals.CHUNK_SIZE_XZ, 0, coords.y * Globals.CHUNK_SIZE_XZ);

                // Apply effect and draw
                foreach (EffectPass pass in chunkEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, mesh.indexBuffer.IndexCount / 3);
                }
            }
        }

        // Rounds down a vector3 point to be used as block coordinates in the voxel world.
        public (int x, int y, int z) Vec3ToGridCoords(Vector3 point)
        {
            return ((int)MathF.Floor(point.X),
                (int)MathF.Floor(point.Y),
                (int)MathF.Floor(point.Z));
        }
        public Block GetBlock(int x, int y, int z) {
            (int x, int y) chunkCoords = (x / Globals.CHUNK_SIZE_XZ, z / Globals.CHUNK_SIZE_XZ);
            (int x, int y, int z) blockCoords = (x % Globals.CHUNK_SIZE_XZ, y % Globals.CHUNK_SIZE_Y, z % Globals.CHUNK_SIZE_XZ);
            if (chunks.TryGetValue(chunkCoords, out var chunk)) {
                return chunks[(chunkCoords.x, chunkCoords.y)].GetBlock(blockCoords.x, blockCoords.y, blockCoords.z);
            }
            return null;
        }
    }
}
