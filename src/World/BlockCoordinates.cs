using Microsoft.Xna.Framework;
using System;

namespace mc_clone
{
    internal class BlockCoordinates
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
        public static bool operator ==(BlockCoordinates a, BlockCoordinates b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }
        public static bool operator !=(BlockCoordinates a, BlockCoordinates b)
        {
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
        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
        public AABBCollider ToAABB()
        {
            return new AABBCollider(
                new Vector3(x, y, z), 
                new Vector3(1), 
                new Vector3(0.5f, 0.5f, 0.5f));
        }
    }
}
