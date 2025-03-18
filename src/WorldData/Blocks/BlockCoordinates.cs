using Microsoft.Xna.Framework;
using System;

using mc_clone.src.Entities.Player;
using System.Collections.Generic;

namespace mc_clone.src.WorldData.Blocks;

// A blocks GLOBAL coordinates - not coords inside a chunk.
public class BlockCoordinates
{
    int x, y, z;
    public int X { get { return this.x; } }
    public int Y { get { return this.y; } }
    public int Z { get { return this.z; } }

    public BlockCoordinates(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public BlockCoordinates(Vector3 position)
    {
        this.x = (int)MathF.Floor(position.X);
        this.y = (int)MathF.Floor(position.Y);
        this.z = (int)MathF.Floor(position.Z);
    }
    public override string ToString()
    {
        return $"BlockCoordinates ({x}, {y}, {z})";
    }
    public BlockCoordinates(ChunkCoordinates chunkCoords, int x, int y, int z)
    {
        this.x = chunkCoords.X * Globals.CHUNK_SIZE_XZ + x;
        this.y = chunkCoords.Y * Globals.CHUNK_SIZE_Y + y;
        this.z = chunkCoords.Z * Globals.CHUNK_SIZE_XZ + z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
    public ChunkCoordinates ToChunkCoordinates()
    {
        return new ChunkCoordinates(
            (int)MathF.Floor(this.x / (float)Globals.CHUNK_SIZE_XZ),
            (int)MathF.Floor(this.y / (float)Globals.CHUNK_SIZE_Y),
            (int)MathF.Floor(this.z / (float)Globals.CHUNK_SIZE_XZ));
    }
    public AABBCollider ToAABB()
    {
        return new AABBCollider(
            new Vector3(x, y, z),
            new Vector3(1),
            new Vector3(0.5f, 0.5f, 0.5f));
    }
    public Dictionary<CardinalDirection, BlockCoordinates> GetNeighborCoordinates()
    {
        Dictionary<CardinalDirection, BlockCoordinates> result = new();
        foreach (CardinalDirection direction in Enum.GetValues(typeof(CardinalDirection)))
        {
            Vector3 neighborOffset = direction.ToOffsetVector();
            result.Add(direction,
                new BlockCoordinates(
                    x + (int)neighborOffset.X,
                    y + (int)neighborOffset.Y,
                    z + (int)neighborOffset.Z));
        }
        return result;
    }

    public static bool operator ==(BlockCoordinates a, BlockCoordinates b)
    {
        if (a is null && b is null) return true; // Both are null, therefore equal
        if (a is null || b is null) return false; // One is null, but not both, therefore not equal.
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }
    public static bool operator !=(BlockCoordinates a, BlockCoordinates b)
    {
        if (a is null && b is null) return false; // Both are null, therefore equal
        if (a is null || b is null) return true; // One is null, but not both, therefore not equal.
        return a.x != b.x || a.y != b.y || a.z != b.z;
    }
    public override bool Equals(object obj)
    {
        if (obj is BlockCoordinates other)
        {
            return this == other;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(x, y, z);
    }

    public static BlockCoordinates operator +(BlockCoordinates a, BlockCoordinates b)
    {
        return new BlockCoordinates(a.x + b.x, a.y + b.y, a.z + b.z);
    }
    public static BlockCoordinates operator -(BlockCoordinates a, BlockCoordinates b)
    {
        return new BlockCoordinates(a.x - b.x, a.y - b.y, a.z - b.z);
    }
    public static BlockCoordinates operator +(BlockCoordinates a, Vector3 b)
    {
        BlockCoordinates bNew = new BlockCoordinates(b);
        return a + bNew;
    }
    public static BlockCoordinates operator -(BlockCoordinates a, Vector3 b)
    {
        BlockCoordinates bNew = new BlockCoordinates(b);
        return a - bNew;
    }
}
