using Microsoft.Xna.Framework;

using System;

namespace mc_clone.src.WorldData
{
    public enum CardinalDirection
    {
        Top, Bottom,
        East, West, // Left,  right
        South, North // Front, back
    }
    public static class CardinalDirectionExtension
    {
        public static Vector3 ToOffsetVector(this CardinalDirection direction)
        {
            return direction switch
            {
                CardinalDirection.Top => new Vector3(0, 1, 0),
                CardinalDirection.Bottom => new Vector3(0, -1, 0),
                CardinalDirection.East => new Vector3(-1, 0, 0),
                CardinalDirection.West => new Vector3(1, 0, 0),
                CardinalDirection.South => new Vector3(0, 0, -1),
                CardinalDirection.North => new Vector3(0, 0, 1),
                _ => Vector3.Zero
            };
        }

        public static Vector3[] ToDefaultVertices(this CardinalDirection direction)
        {
            return direction switch
            {
                CardinalDirection.Top => new Vector3[]
                    {
                        new Vector3(0, 1, 0),
                        new Vector3(0, 1, 1),
                        new Vector3(1, 1, 1),
                        new Vector3(1, 1, 0),
                    },
                CardinalDirection.Bottom => new Vector3[]
                    {
                        new Vector3(0, 0, 0),
                        new Vector3(1, 0, 0),
                        new Vector3(1, 0, 1),
                        new Vector3(0, 0, 1),
                    },
                CardinalDirection.East => new Vector3[]
                    {
                        new Vector3(0, 0, 1),
                        new Vector3(0, 1, 1),
                        new Vector3(0, 1, 0),
                        new Vector3(0, 0, 0),
                    },
                CardinalDirection.West => new Vector3[]
                    {
                        new Vector3(1, 0, 0),
                        new Vector3(1, 1, 0),
                        new Vector3(1, 1, 1),
                        new Vector3(1, 0, 1),
                    },
                CardinalDirection.South => new Vector3[]
                    {
                        new Vector3(0, 0, 0),
                        new Vector3(0, 1, 0),
                        new Vector3(1, 1, 0),
                        new Vector3(1, 0, 0),
                    },
                CardinalDirection.North => new Vector3[]
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
