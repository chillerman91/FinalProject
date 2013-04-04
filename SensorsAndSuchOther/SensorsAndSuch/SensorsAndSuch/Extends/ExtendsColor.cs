using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SensorsAndSuch.Extensions
{
    static class ExtendsColor{
    
        public static Color Combine(this Color col1, Color col2, float weight = .5f)
        {
            float w2 = 1 - weight;
            return new Color((int)(col1.R * weight + col2.R * w2), 
                (int) (col1.G * weight + col2.G * w2), 
                (int) (col1.B * weight + col2.B * w2), 
                (int) (col1.A * weight + col2.A * w2));
        }

        public static int Compare(this Color col1, Color col2)
        {
            if (col1.A < 255 || col2.A < 255)
                return 0;
            int ret = Math.Abs(col1.G-col2.G);
            ret += Math.Abs(col1.R - col2.R);

            ret += Math.Abs(col1.B - col2.B);
            return ret;
        }
    }
}
