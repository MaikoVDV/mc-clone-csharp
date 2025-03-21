﻿using Microsoft.Xna.Framework;
using System;

namespace mc_clone.src.WorldData.Blocks;

// A block's coordinates WITHIN a chunk.
public class LocalBlockCoordinates : BlockCoordinates
{
    private LocalBlockCoordinates(int x, int y, int z) : base(x, y, z) { }
    public LocalBlockCoordinates(Vector3 position) : base(position) { }

    public static LocalBlockCoordinates Create(int x, int y, int z)
    {
        if (!(x >= 0 && x < Globals.CHUNK_SIZE_XZ &&
            y >= 0 && y < Globals.CHUNK_SIZE_Y &&
            z >= 0 && z < Globals.CHUNK_SIZE_XZ))
        {
            // Block coords are outside chunk bounds
            //throw new ArgumentOutOfRangeException($"x {x}, y {y}, z{z}", "Entered local block coordinates which are outside chunk boundaries.");
            return null;
        }
        return new LocalBlockCoordinates(x, y, z);
    }
    public static (ChunkCoordinates, LocalBlockCoordinates) FromGlobal(BlockCoordinates globalCoords)
    {
        LocalBlockCoordinates localCoords = new LocalBlockCoordinates(
            globalCoords.X % Globals.CHUNK_SIZE_XZ,
            globalCoords.Y % Globals.CHUNK_SIZE_Y,
            globalCoords.Z % Globals.CHUNK_SIZE_XZ);
        return (globalCoords.ToChunkCoordinates(), localCoords);
    }
}
