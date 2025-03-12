

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace mc_clone
{
    public static class Util
    {
        public static float Min(this Vector3 vec)
        {
            return MathF.Min(vec.X, MathF.Min(vec.Y, vec.Z));
        }
        public static Vector3 Min(this List<Vector3> list)
        {
            Vector3 min = Vector3.One * float.MaxValue;
            foreach (Vector3 vec in list)
            {
                if (vec.Length() < min.Length()) min = vec;
            }
            return min;
        }
        // Returns an array of axis-aligned unit vectors representing order of input vector's coordinates low-to-high
        public static Vector3[] SortAxes(this Vector3 vec)
        {
            Vector3[] res = new Vector3[3];
            float[] coords = {vec.X, vec.Y, vec.Z};
            Array.Sort(coords);
            res[Array.IndexOf(coords, vec.X)] = Globals.AXIS_VECTOR_X;
            res[Array.IndexOf(coords, vec.Y)] = Globals.AXIS_VECTOR_Y;
            res[Array.IndexOf(coords, vec.Z)] = Globals.AXIS_VECTOR_Z;
            return res;
        }
        public static Vector3 InvertUnitVector(this Vector3 vec)
        {
            Vector3 res = Vector3.Zero;
            res.X = vec.X == 0 ? 1 : 0;
            res.Y = vec.Y == 0 ? 1 : 0;
            res.Z = vec.Z == 0 ? 1 : 0;
            return res;
        }
        public static Vector3 InvertUnitVectorClamped(this Vector3 vec)
        {
            Vector3 res = Vector3.Zero;
            res.X = vec.X < 0.05f ? 1 : 0;
            res.Y = vec.Y < 0.05f ? 1 : 0;
            res.Z = vec.Z < 0.05f ? 1 : 0;
            return res;
        }
    }
}
