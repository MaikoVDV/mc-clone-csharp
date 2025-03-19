﻿using Microsoft.Xna.Framework;
using System;

using mc_clone.src.WorldData.Blocks;

namespace mc_clone.src.WorldData
{
    public class Chunk
    {
        public Block[,,] blocks;
        public BlockDataStore blockDataStore = new();

        public Chunk(ChunkCoordinates coords, bool fill = true)
        {
            blocks = new Block[Globals.CHUNK_SIZE_XZ, Globals.CHUNK_SIZE_Y, Globals.CHUNK_SIZE_XZ];
            blocks = GenerateChunk(fill);
        }

        public BlockQuery GetBlock(LocalBlockCoordinates coords)
        {
            if (coords.X < 0 || coords.X >= blocks.GetLength(0)) return new BlockQuery(null, null);
            if (coords.Y < 0 || coords.Y >= blocks.GetLength(1)) return new BlockQuery(null, null);
            if (coords.Z < 0 || coords.Z >= blocks.GetLength(2)) return new BlockQuery(null, null);
            return new BlockQuery(blocks[coords.X, coords.Y, coords.Z], blockDataStore[coords]);
        }
        public void SetBlock(LocalBlockCoordinates coords, Block block, BlockData data = null)
        {
            blocks[coords.X, coords.Y, coords.Z] = block;
            blockDataStore[coords] = data;
        }

        private Block[,,] GenerateChunk(bool fill)
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
                        if (fill)
                        {
                            BlockType type = BlockType.Stone;
                            if (y > Globals.CHUNK_SIZE_Y - 3) type = BlockType.Dirt;
                            if (y == Globals.CHUNK_SIZE_Y - 1) type = BlockType.Grass;
                            blocks[x, y, z] = new Block(type);
                        } else
                        {
                            blocks[x, y, z] = null;
                        }
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
