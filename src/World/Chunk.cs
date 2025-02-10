using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace mc_clone
{
    internal class Chunk
    {
        public Block[,,] blocks;

        public Chunk()
        {
            blocks = GenerateChunk();
        }

        public (VertexPositionTexture[] vertices, int[] indices) BuildMesh()
        {
            List<VertexPositionTexture> chunkVertices = new();
            List<int> chunkIndices = new();

            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                // Step through bottom to top
                for (int z = 0;  z < blocks.GetLength(2); z++)
                {
                    for (int x = 0; x < blocks.GetLength(0); x++)
                    {
                        Block block = blocks[x, y, z];
                        if (block == null) continue;

                        // Build each face individually
                        for (int i = 0; i < block.Mesh.faces.Count(); i++)
                        {
                            BlockFace face = block.Mesh.faces[i];

                            Vector3 neighborOffset = face.direction.ToOffsetVector();
                            Block neighbor = GetBlock(
                                x + (int)neighborOffset.X, 
                                y + (int)neighborOffset.Y, 
                                z + (int)neighborOffset.Z);
                            if (neighbor != null) continue;

                            List<VertexPositionTexture> faceVertices = new();
                            for (int vIndex = 0;  vIndex < face.vertices.Length; vIndex++)
                            {
                                Vector3 vertexPos = face.vertices[vIndex] + new Vector3(x, y, z);
                                Vector2 uv = block.Type.ToUV(face.direction);
                                uv += vIndex switch
                                {
                                    0 => new Vector2(0,                             Globals.TEXTURE_STEP_FACTOR.Y),
                                    1 => new Vector2(0,                             0),
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

        public Block GetBlock(int x, int y, int z)
        {
            if (x < 0 || x >= blocks.GetLength(0)) return null;
            if (y < 0 || y >= blocks.GetLength(1)) return null;
            if (z < 0 || z >= blocks.GetLength(2)) return null;
            return blocks[x, y, z];
        }
        public void SetBlock((int x, int y,int z) coords, Block block)
        {
            blocks[coords.x, coords.y, coords.z] = block;
        }

        private Block[,,] GenerateChunk()
        {
            // Initialize array
            Block[,,] blocks = new Block[Globals.CHUNK_SIZE_XZ, Globals.CHUNK_SIZE_Y, Globals.CHUNK_SIZE_XZ];
            
            // Assign values to each coordinate within chunk
            for (int y = 0; y < Globals.CHUNK_SIZE_Y; y++)
            {
                // Step through bottom to top
                for (int z = 0; z < Globals.CHUNK_SIZE_XZ; z++)
                {
                    for (int x = 0; x < Globals.CHUNK_SIZE_XZ; x++)
                    {
                        BlockTypes type = BlockTypes.Stone;
                        if (y > Globals.CHUNK_SIZE_Y - 3) type = BlockTypes.Dirt;
                        if (y == Globals.CHUNK_SIZE_Y - 1) type = BlockTypes.Grass;
                        blocks[x, y, z] = new Block(type);
                    }
                }
            }
            return blocks;
        }
    }
}
