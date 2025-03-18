using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

using mc_clone.src.WorldData.Blocks;
using System.Diagnostics;
using mc_clone.src.WorldData.Blocks.Types;

namespace mc_clone.src.WorldData
{
    public partial class World
    {
        public void RegenerateChunkMesh(GraphicsDevice graphicsDevice, ChunkCoordinates coords)
        {
            (VertexPositionTexture[] vertices, int[] indices) = BuildMesh(coords);
            if (vertices.Length == 0 || indices.Length == 0)
            {
                chunkMeshes[coords] = (null, null);
                return;
            }
            VertexBuffer vb = new VertexBuffer(graphicsDevice, typeof(VertexPositionTexture), vertices.Length, BufferUsage.WriteOnly);
            IndexBuffer ib = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Length, BufferUsage.WriteOnly);
            vb.SetData(vertices);
            ib.SetData(indices);
            chunkMeshes[coords] = (vb, ib);
        }

        public (VertexPositionTexture[] vertices, int[] indices) BuildMesh(ChunkCoordinates chunkCoords)
        {
            Chunk chunk = null;
            chunks.TryGetValue(chunkCoords, out chunk);
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
                        if (block == null || block is Air) continue;

                        // Build each face individually
                        for (int i = 0; i < block.Faces.Length; i++)
                        {
                            BlockFace face = block.Faces[i];

                            Vector3 neighborOffset = face.direction.ToOffsetVector();
                            BlockCoordinates neighborCoords = new BlockCoordinates(chunkCoords,
                                    x + (int)neighborOffset.X,
                                    y + (int)neighborOffset.Y,
                                    z + (int)neighborOffset.Z);
                            Block neighbor = GetBlock(neighborCoords);

                            if (!(neighbor is Air || neighbor == null)) continue;

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
}
