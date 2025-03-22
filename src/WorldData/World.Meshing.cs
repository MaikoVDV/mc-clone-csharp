using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

using mc_clone.src.WorldData.Blocks;
using System.Linq;

namespace mc_clone.src.WorldData
{
    public partial class World
    {
        private Dictionary<ChunkCoordinates, List<ChunkMeshPart>> chunkMeshes = new();

        public void RegenerateChunkMeshes(GraphicsDevice graphicsDevice, ChunkCoordinates coords)
        {
            var meshes = BuildChunkMeshes(coords);
            if (chunkMeshes.TryGetValue(coords, out var meshList))
            {
                meshList.Clear();
            } else
            {
                chunkMeshes.Add(coords, new List<ChunkMeshPart>());
            }
            foreach (var mesh in meshes)
            {
                if (mesh.vertices.Length == 0 || mesh.indices.Length == 0) continue;

                VertexBuffer vb = new VertexBuffer(graphicsDevice, typeof(VertexPositionTextureData), mesh.vertices.Length, BufferUsage.WriteOnly);
                IndexBuffer ib = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, mesh.indices.Length, BufferUsage.WriteOnly);

                vb.SetData(mesh.vertices);
                ib.SetData(mesh.indices);

                ChunkMeshPart bundle = new ChunkMeshPart
                {
                    vertexBuffer = vb,
                    indexBuffer = ib,
                    effect = mesh.effect,
                };

                chunkMeshes[coords].Add(bundle);
            }
        }

        public List<(VertexPositionTextureData[] vertices, int[] indices, Effect effect)> BuildChunkMeshes(ChunkCoordinates chunkCoords)
        {
            chunks.TryGetValue(chunkCoords, out Chunk chunk);
            if (chunk == null) return new List<(VertexPositionTextureData[] vertices, int[] indices, Effect effect)>();

            List<(VertexPositionTextureData[], int[], Effect)> result = new();

            {
                var (vertices, indices) = BuildMeshByBlockProperty(chunkCoords, true);
                result.Add((vertices, indices, Globals.basicEffect));
            }
            {
                var (vertices, indices) = BuildMeshByBlockProperty(chunkCoords, false);
                result.Add((vertices, indices, Globals.basicEffect));
            }

            return result;
        }

        public (VertexPositionTextureData[] vertices, int[] indices) BuildMeshByBlockProperty(ChunkCoordinates chunkCoords, bool renderSolids)
        {
            chunks.TryGetValue(chunkCoords, out Chunk chunk);
            if (chunk == null) return (Array.Empty<VertexPositionTextureData>(), Array.Empty<int>());

            List<VertexPositionTextureData> chunkVertices = new();
            List<int> chunkIndices = new();
            List<(Vector3, int[])> faceIndexList = new();

            for (int y = 0; y < chunk.blocks.GetLength(1); y++)
            {
                // Step through bottom to top
                for (int z = 0; z < chunk.blocks.GetLength(2); z++)
                {
                    for (int x = 0; x < chunk.blocks.GetLength(0); x++)
                    {
                        Block block = chunk.blocks[x, y, z];
                        // Filter out blocks that shouldn't be part of the mesh
                        if (block == null) continue;
                        if (renderSolids != BlockPropertyRegistry.Get(block).isSolid) continue;

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

                            if (neighbor.Block != null)
                            {
                                if (renderSolids && BlockPropertyRegistry.Get(neighbor.Block).isSolid) continue;
                                else
                                {
                                    if (neighbor.Block.Type == block.Type) continue;
                                }
                            }

                            List<VertexPositionTextureData> faceVertices = new();
                            for (int vIndex = 0; vIndex < face.vertices.Length; vIndex++)
                            {
                                Vector3 vertexPos = face.vertices[vIndex] + new Vector3(x, y, z);
                                Vector2 uv = block.Type.ToUV(face.direction);
                                float materialId = block.Type.ToMaterialId(face.direction);

                                uv += vIndex switch
                                {
                                    0 => new Vector2(0, Globals.TEXTURE_STEP_FACTOR.Y),
                                    1 => new Vector2(0, 0),
                                    2 => new Vector2(Globals.TEXTURE_STEP_FACTOR.X, 0),
                                    3 => new Vector2(Globals.TEXTURE_STEP_FACTOR.X, Globals.TEXTURE_STEP_FACTOR.Y),
                                    _ => throw new Exception($"Tried assigning texture coordinate to a vertex with an index outside 0-3. Index is {vIndex}")
                                };
                                faceVertices.Add(new VertexPositionTextureData(vertexPos, uv, materialId));
                            }

                            int verticesBeforeFaceAdd = chunkVertices.Count;
                            chunkVertices.AddRange(faceVertices);

                            int[] faceIndices = new int[] {
                                verticesBeforeFaceAdd + 2,
                                verticesBeforeFaceAdd + 1,
                                verticesBeforeFaceAdd + 0,

                                verticesBeforeFaceAdd + 3,
                                verticesBeforeFaceAdd + 2,
                                verticesBeforeFaceAdd + 0,
                            };
                            faceIndexList.Add((chunkCoords.ToGlobalVector3() + new Vector3(x, y, z) + face.centerPosition, faceIndices));
                            chunkIndices.AddRange(faceIndices);
                        }
                    }
                }
            }

            // Sort transparent faces
            if (!renderSolids)
            {
                if (player != null)
                {
                    Vector3 camPos = player.camera.Position;
                    chunkIndices = faceIndexList.OrderByDescending(faceTuple => Vector3.Distance(faceTuple.Item1, camPos)).SelectMany(faceTuple => faceTuple.Item2).ToList();
                }
            }

            return (chunkVertices.ToArray(), chunkIndices.ToArray());
        }
    }

    public struct ChunkMeshPart
    {
        public VertexBuffer vertexBuffer;
        public IndexBuffer indexBuffer;
        public Effect effect;
    }
}
