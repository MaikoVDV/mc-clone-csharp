using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mc_clone
{
    public partial class World
    {
        // DDA algorithm basedon https://lodev.org/cgtutor/raycasting.html
        public Nullable<(Block block, BlockFaceDirection, Vector3 hitPoint, BlockCoordinates coords)> CastRay(Ray ray, float maxDistance = float.MaxValue)
        {
            BlockCoordinates gridCoords = new BlockCoordinates(ray.Position);

            Vector3 stepDirection = new Vector3(Math.Sign(ray.Direction.X), Math.Sign(ray.Direction.Y), Math.Sign(ray.Direction.Z));

            // Length of ray needed to travel to the next grid block for each axis.
            // Check for 0 is just to prevent dividing by zero.
            Vector3 deltaDist = new Vector3(
                ray.Direction.X == 0 ? float.MaxValue : MathF.Abs(1 / ray.Direction.X),
                ray.Direction.Y == 0 ? float.MaxValue : MathF.Abs(1 / ray.Direction.Y),
                ray.Direction.Z == 0 ? float.MaxValue : MathF.Abs(1 / ray.Direction.Z));

            Vector3 sideDist = new Vector3(
                (stepDirection.X > 0 ? (gridCoords.X + 1f - ray.Position.X) : (ray.Position.X - gridCoords.X)) * deltaDist.X,
                (stepDirection.Y > 0 ? (gridCoords.Y + 1f - ray.Position.Y) : (ray.Position.Y - gridCoords.Y)) * deltaDist.Y,
                (stepDirection.Z > 0 ? (gridCoords.Z + 1f - ray.Position.Z) : (ray.Position.Z - gridCoords.Z)) * deltaDist.Z
                );

            bool hit = false;
            char side;
            BlockFaceDirection faceDir;

            float distanceTravelled = 0;

            int index = 0;
            while (!hit && index < 10)
            {
                float prevDist = distanceTravelled;
                index++;
                if (GetBlock(gridCoords) != null && index == 0)
                {
                    // Started DDA search inside a block.
                    Debug.WriteLine("Started DDA search from inside a block");
                    return null;
                }

                if (sideDist.Min() == sideDist.X)
                {
                    distanceTravelled = sideDist.X;
                    sideDist.X += deltaDist.X;
                    gridCoords += new Vector3(stepDirection.X, 0, 0);
                    side = 'x';
                }
                else if (sideDist.Min() == sideDist.Y)
                {
                    distanceTravelled = sideDist.Y;
                    sideDist.Y += deltaDist.Y;
                    gridCoords += new Vector3(0, stepDirection.Y, 0);
                    side = 'y';
                }
                else if (sideDist.Min() == sideDist.Z)
                {
                    distanceTravelled = sideDist.Z;
                    sideDist.Z += deltaDist.Z;
                    gridCoords += new Vector3(0, 0, stepDirection.Z);
                    side = 'z';
                }
                else
                {
                    throw new Exception("Error in DDA: Found now lowest value for side dists.");
                }

                switch (side)
                {
                    case 'x':
                        faceDir = stepDirection.X == 1 ? BlockFaceDirection.East : BlockFaceDirection.West;
                        break;
                    case 'y':
                        faceDir = stepDirection.Y == 1 ? BlockFaceDirection.Bottom : BlockFaceDirection.Top;
                        break;
                    case 'z':
                        faceDir = stepDirection.Z == 1 ? BlockFaceDirection.South : BlockFaceDirection.North;
                        break;
                    default: throw new Exception("wat");
                }


                Vector3 currentRayPoint = ray.Position + (sideDist.Min() - deltaDist.Min()) * ray.Direction;
                if (distanceTravelled >= maxDistance) return null;
                Block hitBlock = GetBlock(gridCoords);
                if (hitBlock != null)
                {
                    hit = true;
                    return (hitBlock,
                        faceDir,
                        currentRayPoint,
                        gridCoords);
                }
            }

            return null;
        }
    }
}
