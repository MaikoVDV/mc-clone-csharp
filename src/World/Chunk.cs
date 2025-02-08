using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mc_clone
{
    internal class Chunk
    {
        public Block[,,] blocks;

        public Chunk()
        {
            blocks = GenerateChunk();
        }

        public (VertexPositionColor[] vertices, int[] indices) BuildMesh()
        {
            List<VertexPositionColor> chunkVertices = new();
            List<int> chunkIndices = new();

            for (int y = 0; y < blocks.GetLength(1); y++)
            {
                // Step through bottom to top
                for (int z = 0;  z < blocks.GetLength(2); z++)
                {
                    for (int x = 0; x < blocks.GetLength(0); x++)
                    {
                        Block block = blocks[x, y, z];


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

                            VertexPositionColor[] faceVertices = face.vertices;
                            for (int vIndex = 0;  vIndex < faceVertices.Length; vIndex++)
                            {
                                faceVertices[vIndex].Position += new Vector3(x, y, z);
                            }

                            int verticesBeforeFaceAdd = chunkVertices.Count;
                            chunkVertices.AddRange(face.vertices);
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

        private Block[,,] GenerateChunk()
        {
            // Initialize array
            Block[,,] blocks = new Block[MCClone.CHUNK_SIZE_XZ, MCClone.CHUNK_SIZE_Y, MCClone.CHUNK_SIZE_XZ];
            
            // Assign values to each coordinate within chunk
            for (int y = 0; y < MCClone.CHUNK_SIZE_Y; y++)
            {
                // Step through bottom to top
                for (int z = 0; z < MCClone.CHUNK_SIZE_XZ; z++)
                {
                    for (int x = 0; x < MCClone.CHUNK_SIZE_XZ; x++)
                    {
                        blocks[x, y, z] = new Block();
                    }
                }
            }
            return blocks;
        }
    }
}
