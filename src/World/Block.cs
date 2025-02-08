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
        private CubeMesh mesh = new CubeMesh();
        public CubeMesh Mesh { get { return mesh; } }
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
        public VertexPositionColor[] vertices;

        public BlockFace(BlockFaceDirection direction)
        {
            this.direction = direction;
            this.vertices = direction.ToDefaultVertices();
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

        public static VertexPositionColor[] ToDefaultVertices(this BlockFaceDirection direction)
        {
            return direction switch
            {
                BlockFaceDirection.Top => new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(0, 1, 0), Color.Green),
                        new VertexPositionColor(new Vector3(0, 1, 1), Color.Green),
                        new VertexPositionColor(new Vector3(1, 1, 1), Color.Green),
                        new VertexPositionColor(new Vector3(1, 1, 0), Color.Green),
                    },
                BlockFaceDirection.Bottom => new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(0, 0, 0), Color.Yellow),
                        new VertexPositionColor(new Vector3(1, 0, 0), Color.Yellow),
                        new VertexPositionColor(new Vector3(1, 0, 1), Color.Yellow),
                        new VertexPositionColor(new Vector3(0, 0, 1), Color.Yellow),
                    },
                BlockFaceDirection.East => new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(0, 1, 1), Color.HotPink),
                        new VertexPositionColor(new Vector3(0, 1, 0), Color.HotPink),
                        new VertexPositionColor(new Vector3(0, 0, 0), Color.HotPink),
                        new VertexPositionColor(new Vector3(0, 0, 1), Color.HotPink),
                    },
                BlockFaceDirection.West => new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(1, 1, 0), Color.Purple),
                        new VertexPositionColor(new Vector3(1, 1, 1), Color.Purple),
                        new VertexPositionColor(new Vector3(1, 0, 1), Color.Purple),
                        new VertexPositionColor(new Vector3(1, 0, 0), Color.Purple),
                    },
                BlockFaceDirection.South => new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(0, 0, 0), Color.Red),
                        new VertexPositionColor(new Vector3(0, 1, 0), Color.Red),
                        new VertexPositionColor(new Vector3(1, 1, 0), Color.Red),
                        new VertexPositionColor(new Vector3(1, 0, 0), Color.Red),
                    },
                BlockFaceDirection.North => new VertexPositionColor[]
                    {
                        new VertexPositionColor(new Vector3(1, 0, 1), Color.Blue),
                        new VertexPositionColor(new Vector3(1, 1, 1), Color.Blue),
                        new VertexPositionColor(new Vector3(0, 1, 1), Color.Blue),
                        new VertexPositionColor(new Vector3(0, 0, 1), Color.Blue),
                    },
                _ => throw new Exception("Tried getting default vertices for an unknown face direction.")
            };
        }
    }
}
