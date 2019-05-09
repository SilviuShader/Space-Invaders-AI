using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGame
{
    public struct SPoint
    {
        public double x, y;
        
        public static SPoint operator +(SPoint point, SVector2D vec)
        {
            SPoint result = new SPoint(point.x + vec.x, point.y + vec.y);
            return result;    
        }
        public static SPoint operator +(SPoint point, double val)
        {
            SPoint result = new SPoint(point.x + val, point.y + val);
            return result;
        }
        public static SPoint operator -(SPoint point, SVector2D vec)
        {
            SPoint result = new SPoint(point.x - vec.x, point.y - vec.y);
            return result;  
        }
        public static SPoint operator -(SPoint point, double val)
        {
            SPoint result = new SPoint(point.x - val, point.y - val);
            return result;
        }

        public SPoint(double a = 0, double b = 0) 
        {
            x = a;
            y = b;
        }
    }

    public struct SVector2D
    {
        public double x, y;

        public static SVector2D operator +(SVector2D vec1, SVector2D vec2)
        {
            SVector2D result = new SVector2D(vec1.x + vec2.x, vec1.y + vec2.y);
            return result;
        }
        public static SVector2D operator +(SVector2D vec1, double val)
        {
            SVector2D result = new SVector2D(vec1.x + val, vec1.y + val);
            return result;
        } 

        public static SVector2D operator -(SVector2D vec1, SVector2D vec2)
        {
            SVector2D result = new SVector2D(vec1.x - vec2.x, vec1.y - vec2.y);
            return result;
        }
        public static SVector2D operator -(SVector2D vec1, double val)
        {
            SVector2D result = new SVector2D(vec1.x - val, vec1.y - val);
            return result;
        } 

        public static SVector2D operator *(SVector2D vec1, double val)
        {
            SVector2D result = new SVector2D(vec1.x * val, vec1.y * val);
            return result;
        }

        public static SVector2D operator /(SVector2D vec1, double val)
        {
            SVector2D result = new SVector2D(vec1.x / val, vec1.y / val);
            return result;
        }

        public                  SVector2D(double a = 0.0, double b = 0.0)
        {
            x = a;
            y = b;
        }

        public static double    Length(ref SVector2D v)
        {
            return Math.Sqrt((v.x * v.x) + (v.y * v.y));
        }

        public static void      Normalize(ref SVector2D v)
        {
            v = v / Length(ref v);
        }

        public static double    Dot(ref SVector2D v1, ref SVector2D v2)
        {
            return (v1.x * v2.x) + (v1.y * v2.y);
        }

        public static int       Sign(ref SVector2D v1, ref SVector2D v2)
        {
            if(v1.y * v2.x > v1.x * v2.y)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}