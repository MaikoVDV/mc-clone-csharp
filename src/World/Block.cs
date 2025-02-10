using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace mc_clone
{
    internal class Block
    {
        private BlockTypes type;
        public BlockTypes Type { get { return type; } }
        private CubeMesh mesh = new CubeMesh();
        public CubeMesh Mesh { get { return mesh; } }

        public Block(BlockTypes type)
        {
            this.type = type;
        }
    }

    internal struct CubeMesh
    {
        public BlockFace[] faces;

        public CubeMesh()
        {
            faces = new BlockFace[]
            {
                new BlockFace(BlockFaceDirection.Top),
                new BlockFace(BlockFaceDirection.Bottom),
                new BlockFace(BlockFaceDirection.East),
                new BlockFace(BlockFaceDirection.West),
                new BlockFace(BlockFaceDirection.South),
                new BlockFace(BlockFaceDirection.North),
            };
        }
    }

    internal struct BlockFace
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
