using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

using mc_clone.src.WorldData.Blocks;
using System.Diagnostics;
using System.Linq;

namespace mc_clone.src.WorldData
{
    public partial class World
    {
        private Dictionary<ChunkCoordinates, List<MeshBundle>> chunkMeshes = new();

        public void RegenerateChunkMeshes(GraphicsDevice graphicsDevice, ChunkCoordinates coords)
        {
            var meshes = BuildChunkMeshes(coords);
            if (chunkMeshes.TryGetValue(coords, out var meshList))
            {
                meshList.Clear();
            } else
            {
                chunkMeshes.Add(coords, new List<MeshBundle>());
            }
            foreach (var mesh in meshes)
            {
                if (mesh.vertices.Length == 0 || mesh.indices.Length == 0) continue;

                VertexBuffer vb = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), mesh.vertices.Length, BufferUsage.WriteOnly);
                IndexBuffer ib = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, mesh.indices.Length, BufferUsage.WriteOnly);

                vb.SetData(mesh.vertices);
                ib.SetData(mesh.indices);

                MeshBundle bundle = new MeshBundle
                {
                    vertexBuffer = vb,
                    indexBuffer = ib,
                    effect = mesh.effect,
                };

                chunkMeshes[coords].Add(bundle);
            }
        }

        public List<(VertexPositionTexture[] vertices, int[] indices, Effect effect)> BuildChunkMeshes(ChunkCoordinates chunkCoords)
        {
            chunks.TryGetValue(chunkCoords, out Chunk chunk);
            if (chunk == null) return new List<(VertexPositionTexture[] vertices, int[] indices, Effect effect)>();

            List<(VertexPositionTexture[], int[], Effect)> result = new();

            {
                var (vertices, indices) = BuildMeshByBlockProperty(chunkCoords, (BlockType type) =>
                {
                    return BlockPropertyRegistry.Get(type).isSolid;
                });
                result.Add((vertices, indices, Globals.basicEffect));
            }
            {
                var meshData = BuildMeshByBlockProperty(chunkCoords, (BlockType type) =>
                {
                    return type == BlockType.Water;
                });
                result.Add((meshData.vertices, meshData.indices, Globals.waterEffect));
            }

            return result;
        }

        public (VertexPositionTexture[] vertices, int[] indices) BuildMeshByBlockProperty(ChunkCoordinates chunkCoords, BlockTypeFilterClause filterClause)
        {
            chunks.TryGetValue(chunkCoords, out Chunk chunk);
            if (chunk == null) return (Array.Empty<VertexPositionTexture>(), Array.Empty<int>());

            List<VertexPositionTexture> chunkVertices = new();
            List<int> chunkIndices = new();

            for (int y = 0; y < chunk.blocks.GetLength(1); y++)
            {
                // Step through bottom to top
                for (int z = 0; z < chunk.blocks.GetLength(2); z++)
                {
                    for (int x = 0; x < chunk.blocks.GetLength(0); x++)
                    {
                        Block block = chunk.blocks[x, y, z];
                        if (block == null || !filterClause(block.Type)) continue;

                        // Build each face individually
                        for (int i = 0; i < block.Faces.Length; i++)
                        {
                            BlockFace face = block.Faces[i];

                            Vector3 neighborOffset = face.direction.ToOffsetVector();
                            BlockCoordinates neighborCoords = new BlockCoordinates(chunkCoords,
                                    x + (int)neighborOffset.X,
                                    y + (int)neighborOffset.Y,
                                    z + (int)neighborOffset.Z);
                            BlockQuery neighbor = GetBlock(neighborCoords);

                            if (neighbor.Block != null && filterClause(neighbor.Block.Type)) continue;

                            List<VertexPositionTexture> faceVertices = new();
                            for (int vIndex = 0; vIndex < face.vertices.Length; vIndex++)
                            {
                                Vector3 vertexPos = face.vertices[vIndex] + new Vector3(x, y, z);
                                Vector2 uv = block.Type.ToUV(face.direction);
                                uv += vIndex switch
                                {
                                    0 => new Vector2(0, Globals.TEXTURE_STEP_FACTOR.Y),
                                    1 => new Vector2(0, 0),
                                    2 => new Vector2(Globals.TEXTURE_STEP_FACTOR.X, 0),
                                    3 => new Vector2(Globals.TEXTURE_STEP_FACTOR.X, Globals.TEXTURE_STEP_FACTOR.Y),
                                    _ => throw new Exception($"Tried assigning texture coordinate to a vertex with an index outside 0-3. Index is {vIndex}")
                                };
                                faceVertices.Add(new VertexPositionTexture(vertexPos, uv));
                            }

                            int verticesBeforeFaceAdd = chunkVertices.Count;
                            chunkVertices.AddRange(faceVertices);
                            chunkIndices.AddRange(new int[] {
                                verticesBeforeFaceAdd + 2,
                                verticesBeforeFaceAdd + 1,
                                verticesBeforeFaceAdd + 0,

                                verticesBeforeFaceAdd + 3,
                                verticesBeforeFaceAdd + 2,
                                verticesBeforeFaceAdd + 0,
                            });
                        }
                    }
                }
            }
            return (chunkVertices.ToArray(), chunkIndices.ToArray());
        }
    }
    public struct MeshBundle
    {
        public VertexBuffer vertexBuffer;
        public IndexBuffer indexBuffer;
        public Effect effect;
    }
}
