using mc_clone.src.WorldData.Blocks.Behaviors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        //public (int x, int y) textureCoords; // Integer coordinates for which part of the texture atlas. These are NOT UV coordinates.

        public BlockFace(CardinalDirection direction /*, (int x, int y) textureCoords*/)
        {
            this.direction = direction;
            this.vertices = direction.ToDefaultVertices();
            //this.textureCoords = textureCoords;
        }
    }
}
