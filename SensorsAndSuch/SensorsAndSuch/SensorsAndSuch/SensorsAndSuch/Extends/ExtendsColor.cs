using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SensorsAndSuch.Extensions
{
    static class ExtendsColor{
    
        public static Color Combine(this Color col1, Color col2)
        {
            return new Color((col1.R + col2.R) / 2, (col1.G + col2.G) / 2, (col1.B + col2.B) / 2);
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
