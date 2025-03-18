using mc_clone.src.WorldData.Blocks.Types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace mc_clone.src.WorldData.Blocks
{
    public abstract class Block
    {
        private BlockTypes type;
        public BlockTypes Type { get { return type; } }
        private BlockFace[] faces;
        public BlockFace[] Faces { get { return faces; } }

        public Block(BlockTypes type)
        {
            this.type = type;
            this.faces = new BlockFace[]
            {
                new BlockFace(BlockFaceDirection.Top),
                new BlockFace(BlockFaceDirection.Bottom),
                new BlockFace(BlockFaceDirection.East),
                new BlockFace(BlockFaceDirection.West),
                new BlockFace(BlockFaceDirection.South),
                new BlockFace(BlockFaceDirection.North),
            };
        }
        public virtual void Update(World world, BlockCoordinates coords) { }

        public Dictionary<BlockFaceDirection, BlockCoordinates> GetNeighborCoordinates(BlockCoordinates coords)
        {
            Dictionary<BlockFaceDirection, BlockCoordinates> result = new();
            foreach (var face in faces)
            {
                Vector3 neighborOffset = face.direction.ToOffsetVector();
                result.Add(face.direction, 
                    new BlockCoordinates(
                        coords.X + (int)neighborOffset.X,
                        coords.Y + (int)neighborOffset.Y,
                        coords.Z + (int)neighborOffset.Z));
            }
            return result;
        }
        public Dictionary<BlockFaceDirection, Block> GetNeighborBlocks(World world, BlockCoordinates coords)
        {
            Dictionary<BlockFaceDirection, Block> result = new();
            foreach (var neighbor in GetNeighborCoordinates(coords))
            {
                result.Add(neighbor.Key, world.GetBlock(neighbor.Value));
            }
            return result;
        }
    }

    public struct BlockFace
    {
        public BlockFaceDirection direction;
        public Vector3[] vertices;
        //public (int x, int y) textureCoords; // Integer coordinates for which part of the texture atlas. These are NOT UV coordinates.

        public BlockFace(BlockFaceDirection direction /*, (int x, int y) textureCoords*/)
        {
            this.direction = direction;
            this.vertices = direction.ToDefaultVertices();
            //this.textureCoords = textureCoords;
        }
    }

    public enum BlockFaceDirection
    {
        Top,   Bottom,
        East,  West, // Left,  right
        South, North // Front, back
    }
    public static class BlockFaceDirectionExtension
    {
        public static Vector3 ToOffsetVector(this BlockFaceDirection direction)
        {
            return direction switch
            {
                BlockFaceDirection.Top => new Vector3(0, 1, 0),
                BlockFaceDirection.Bottom => new Vector3(0, -1, 0),
                BlockFaceDirection.East => new Vector3(-1, 0, 0),
                BlockFaceDirection.West => new Vector3(1, 0, 0),
                BlockFaceDirection.South => new Vector3(0, 0, -1),
                BlockFaceDirection.North => new Vector3(0, 0, 1),
                _ => Vector3.Zero
            };
        }

        public static Vector3[] ToDefaultVertices(this BlockFaceDirection direction)
        {
            return direction switch
            {
                BlockFaceDirection.Top => new Vector3[]
                    {
                        new Vector3(0, 1, 0),
                        new Vector3(0, 1, 1),
                        new Vector3(1, 1, 1),
                        new Vector3(1, 1, 0),
                    },
                BlockFaceDirection.Bottom => new Vector3[]
                    {
                        new Vector3(0, 0, 0),
                        new Vector3(1, 0, 0),
                        new Vector3(1, 0, 1),
                        new Vector3(0, 0, 1),
                    },
                BlockFaceDirection.East => new Vector3[]
                    {
                        new Vector3(0, 0, 1),
                        new Vector3(0, 1, 1),
                        new Vector3(0, 1, 0),
                        new Vector3(0, 0, 0),
                    },
                BlockFaceDirection.West => new Vector3[]
                    {
                        new Vector3(1, 0, 0),
                        new Vector3(1, 1, 0),
                        new Vector3(1, 1, 1),
                        new Vector3(1, 0, 1),
                    },
                BlockFaceDirection.South => new Vector3[]
                    {
                        new Vector3(0, 0, 0),
                        new Vector3(0, 1, 0),
                        new Vector3(1, 1, 0),
                        new Vector3(1, 0, 0),
                    },
                BlockFaceDirection.North => new Vector3[]
                    {
                        new Vector3(1, 0, 1),
                        new Vector3(1, 1, 1),
                        new Vector3(0, 1, 1),
                        new Vector3(0, 0, 1),
                    },
                _ => throw new Exception("Tried getting default vertices for an unknown face direction.")
            };
        }
    }
}
