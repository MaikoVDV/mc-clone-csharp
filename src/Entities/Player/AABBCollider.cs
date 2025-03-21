﻿using Microsoft.Xna.Framework;
using System;

using mc_clone.src.WorldData.Blocks;

namespace mc_clone.src.Entities.Player
{
    public class AABBCollider
    {
        private Vector3 parentPos;
        private Vector3 scale;
        private Vector3 offset;

        public Vector3 Position { get { return parentPos; } set { parentPos = value; } }
        private Vector3 Center { get { return parentPos + offset; } }
        public Vector3 MinValues { get { return Center - scale / 2; } }
        public Vector3 MaxValues { get { return Center + scale / 2; } }

        public AABBCollider(Vector3 parentPos, Vector3 scale, Vector3 offset)
        {
            this.parentPos = parentPos;
            this.scale = scale;
            this.offset = offset;
        }
        public void Translate(Vector3 movement)
        {
            parentPos += movement;
        }
        public override string ToString()
        {
            return $"AABB {{ Center: {Center} Scale: {scale} }}";
        }

        // Does the AABB intersect a given other AABB
        public bool Intersects(AABBCollider other)
        {
            return MinValues.X < other.MaxValues.X &&
                MaxValues.X > other.MinValues.X &&
                MinValues.Y < other.MaxValues.Y &&
                MaxValues.Y > other.MinValues.Y &&
                MinValues.Z < other.MaxValues.Z &&
                MaxValues.Z > other.MinValues.Z;
        }

        // Does the AABB intersect with any of the blocks in the given array
        public bool IntersectsBlocks(BlockCoordinates[] blocks)
        {
            foreach (BlockCoordinates coords in blocks)
            {
                if (Intersects(coords.ToAABB())) return true;
            }
            return false;
        }

        // Determines the overlap (aka collision depth) between this and another AABB.
        public Vector3? IntersectsDepth(AABBCollider other)
        {
            if (!Intersects(other)) return null;
            Vector3 overlap = new Vector3(
                MathF.Min(MaxValues.X - other.MinValues.X, other.MaxValues.X - MinValues.X),
                MathF.Min(MaxValues.Y - other.MinValues.Y, other.MaxValues.Y - MinValues.Y),
                MathF.Min(MaxValues.Z - other.MinValues.Z, other.MaxValues.Z - MinValues.Z)
                );
            if (overlap.X <= 0 || overlap.Y <= 0 || overlap.Z <= 0) return null;

            return overlap;
        }

        // Determines how much a given AABB should be translated to avoid clipping into the given blocks.
        public Vector3? IntersectsBlocksMTV(BlockCoordinates[] blocks)
        {
            float minOverlap = float.MaxValue;
            Vector3 minDepth = Vector3.One * float.MaxValue;
            Vector3 mtv = Vector3.Zero;
            foreach (BlockCoordinates coords in blocks)
            {
                if (IntersectsDepth(coords.ToAABB()) is Vector3 depth)
                {
                    if (depth.Length() < minDepth.Length())
                    {
                        if (depth.X < minOverlap)
                        {
                            minOverlap = depth.X;
                            mtv = new Vector3(MinValues.X < coords.ToAABB().MinValues.X ? depth.X : -depth.X, 0, 0);
                        }
                        if (depth.Y < minOverlap)
                        {
                            minOverlap = depth.Y;
                            mtv = new Vector3(0, MinValues.Y < coords.ToAABB().MinValues.Y ? depth.Y : -depth.Y, 0);
                        }
                        if (depth.Z < minOverlap)
                        {
                            minOverlap = depth.Z;
                            mtv = new Vector3(0, 0, MinValues.Z < coords.ToAABB().MinValues.Z ? depth.Z : -depth.Z);
                        }
                    }
                }
            }
            return mtv;
        }
    }
}
