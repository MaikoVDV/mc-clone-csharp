using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace mc_clone
{
    public class Chunk
    {
        public Block[,,] blocks;
        private ChunkCoordinates chunkCoords;

        public Chunk(ChunkCoordinates coords, bool fill = true)
        {
            this.chunkCoords = coords;
            blocks = new Block[Globals.CHUNK_SIZE_XZ, Globals.CHUNK_SIZE_Y, Globals.CHUNK_SIZE_XZ];
            if(fill) blocks = GenerateChunk();
        }

        public (VertexPositionTexture[] vertices, int[] indices) BuildMesh(World world)
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
                        for (int i = 0; i < block.Faces.Count(); i++)
                        {
                            BlockFace face = block.Faces[i];

                            Vector3 neighborOffset = face.direction.ToOffsetVector();
                            LocalBlockCoordinates neighborCoords = LocalBlockCoordinates.Create(
                                x + (int)neighborOffset.X,
                                y + (int)neighborOffset.Y,
                                z + (int)neighborOffset.Z);

                            if (neighborCoords == null)
                            {
                                // Coords are outside of chunk bounds.
                                Block globalNeighbor = world.GetBlock(new BlockCoordinates(chunkCoords, 
                                    x + (int)neighborOffset.X,
                                    y + (int)neighborOffset.Y,
                                    z + (int)neighborOffset.Z));
                                if (globalNeighbor != null) continue;
                            } else
                            {
                                Block neighbor = GetBlock(neighborCoords);
                                if (neighbor != null) continue;

                            }

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

        public Block GetBlock(LocalBlockCoordinates coords)
        {
            if (coords.X < 0 || coords.X >= blocks.GetLength(0)) return null;
            if (coords.Y < 0 || coords.Y >= blocks.GetLength(1)) return null;
            if (coords.Z < 0 || coords.Z >= blocks.GetLength(2)) return null;
            return blocks[coords.X, coords.Y, coords.Z];
        }
        public void SetBlock(LocalBlockCoordinates coords, Block block)
        {
            blocks[coords.X, coords.Y, coords.Z] = block;
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
                        blocks[x, y, z] = new SolidBlock(type);
                    }
                }
            }
            return blocks;
        }
    }

    public class ChunkCoordinates
    {
        int x,y,z;
        public int X { get { return this.x; } }
        public int Y { get { return this.y; } }
        public int Z { get { return this.z; } }

        public ChunkCoordinates(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public ChunkCoordinates(Vector3 position)
        {
            this.x = (int)MathF.Floor(position.X);
            this.y = (int)MathF.Floor(position.Y);
            this.z = (int)MathF.Floor(position.Z);
        }
        public static ChunkCoordinates Zero
        {
            get
            {
                return new ChunkCoordinates(0, 0, 0);
            }
        }
        public override string ToString()
        {
            return $"ChunkCoordinates ({x}, {y}, {z})";
        }
        public static bool operator ==(ChunkCoordinates a, ChunkCoordinates b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }
        public static bool operator !=(ChunkCoordinates a, ChunkCoordinates b)
        {
            return a.x != b.x || a.y != b.y || a.z != b.z;
        }
        public override bool Equals(object obj)
        {
            if (obj is ChunkCoordinates other)
            {
                return this == other;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(x, y, z);
        }

        public static ChunkCoordinates operator +(ChunkCoordinates a, ChunkCoordinates b)
        {
            return new ChunkCoordinates(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static ChunkCoordinates operator -(ChunkCoordinates a, ChunkCoordinates b)
        {
            return new ChunkCoordinates(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        public static ChunkCoordinates operator +(ChunkCoordinates a, Vector3 b)
        {
            ChunkCoordinates bNew = new ChunkCoordinates(b);
            return a + bNew;
        }
        public static ChunkCoordinates operator -(ChunkCoordinates a, Vector3 b)
        {
            ChunkCoordinates bNew = new ChunkCoordinates(b);
            return a - bNew;
        }
    }
}
