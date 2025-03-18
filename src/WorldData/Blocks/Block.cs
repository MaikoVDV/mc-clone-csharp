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
        private BlockType type;
        public BlockType Type { get { return type; } }
        private BlockFace[] faces;
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
        public virtual void SelfChangedUpdateHandler(World world, BlockCoordinates coords) { } // Triggered when the block's state is changed or when the block is placed.
        //public virtual void NeighborPlacedUpdateHandler(World world, BlockCoordinates coords) { } // Triggered when a neighboring block's type changes.
        public virtual void NeighborChangedUpdateHandler(World world, BlockCoordinates coords) { } // Triggered when a neighboring block's state changes.

        public Dictionary<CardinalDirection, Block> GetNeighborBlocks(World world, BlockCoordinates coords)
        {
            Dictionary<CardinalDirection, Block> result = new();
            foreach (var neighbor in coords.GetNeighborCoordinates())
            {
                result.Add(neighbor.Key, world.GetBlock(neighbor.Value));
            }
            return result;
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
