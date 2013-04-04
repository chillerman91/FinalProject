using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SensorsAndSuch.Extensions
{
    static class ExtendsVec {
    
        public static Vector2 Times(this Vector2 vec, double j) {
            float i = (float) j;
            return new Vector2( vec.X * i, vec.Y * i);
        }

        public static Vector2 Flip(this Vector2 vec)
        {
            return new Vector2(vec.Y, vec.X);
        }

        public static Vector2 Align(this Vector2 vec)
        {
            Vector2 ret ;
            if (Math.Abs(vec.X) > Math.Abs(vec.Y))
            {
                ret = new Vector2(vec.X, 0);
                ret.Normalize();
                ret.X += ret.X / 100;
                return ret;
            }
            ret = new Vector2(0, vec.Y);
            ret.Normalize();

            ret.Y += ret.Y / 100;
            return ret;
        }

        public static Vector2 Abs(this Vector2 vec)
        {
            return new Vector2(Math.Abs(vec.X), Math.Abs(vec.Y));
        }

        public static bool Equals(this Vector2 vec, Vector2 vec2)
        {
            return (int)vec.X == (int)vec2.X && (int)vec.Y == (int)vec2.Y;
        }

        public static bool VEquals(this Vector2 vec, Vector2 vec2)
        {
            return (int)vec.X == (int)vec2.X && (int)vec.Y == (int)vec2.Y;
        }

        public static Vector2 AddAng(this Vector2 vec, int X)
        {
            vec = (vec + new Vector2(Globals.rand.Next(10) - 5, Globals.rand.Next(10) - 5));
            vec.Normalize();
            return vec;
        }

        public static Vector2 GetVecFromAng(this float inRadians)
        {
            Vector2 vec = new Vector2((float)Math.Cos(inRadians), (float) Math.Sin(inRadians));
            vec.Normalize();
            return vec;
        }


        public static bool IEquals(this Vector2 vec, int X, int Y)
        {
            return (int)vec.X == X && (int)vec.Y == Y;
        }

        public static List<Vector2> Flip(this List<Vector2> list) 
        {
            List<Vector2> ret = new List<Vector2>();
            while (list.Count > 0) 
            {
                ret.Add(list[list.Count - 1]);

                list.RemoveAt(list.Count - 1);
            }
            return ret;
        }
    }
}
