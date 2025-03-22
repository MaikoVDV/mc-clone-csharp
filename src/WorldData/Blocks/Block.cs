using mc_clone.src.WorldData.Blocks.Behaviors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Schema;

namespace mc_clone.src.WorldData.Blocks
{
    public class Block
    {
        private BlockType type;
        public BlockType Type { get { return type; } }
        private readonly BlockFace[] faces;
        public BlockFace[] Faces { get { return faces; } }

        public Block(BlockType type)
        {
            this.type = type;
            this.faces = new BlockFace[]
            {
                new BlockFace(CardinalDirection.Top),
                new BlockFace(CardinalDirection.Bottom),
                new BlockFace(CardinalDirection.East),
                new BlockFace(CardinalDirection.West),
                new BlockFace(CardinalDirection.South),
                new BlockFace(CardinalDirection.North),
            };
        }
    }

    public struct BlockFace
    {
        public CardinalDirection direction;
        public Vector3[] vertices;

        public Vector3 centerPosition
        {
            get
            {
                Vector3 vertexPositions = Vector3.Zero;
                foreach (var vertex in vertices)
                {
                    vertexPositions += vertex;
                }
                return vertexPositions / vertices.Length;
            }
        }

        public BlockFace(CardinalDirection direction)
        {
            this.direction = direction;
            this.vertices = direction.ToDefaultVertices();
        }
    }
}
