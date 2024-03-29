﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNET
{
    [Serializable]
    public class Vector2Int
    {
        public int x;
        public int y;

        public static implicit operator Vector2Int[](Vector2Int v)
        {
            Vector2Int[] vs = new Vector2Int[1];
            vs[0] = v;
            return vs;
        }

        public static implicit operator Vector2Int((int, int) values) => new Vector2Int(values.Item1, values.Item2);

        public Vector2Int()
        {
            x = 0;
            y = 0;
        }

        public Vector2Int(int X, int Y)
        {
            x = X;
            y = Y;
        }

        public static Vector2Int operator *(Vector2Int a, int b)
        {
            a.x *= b;
            a.y *= b;
            return a;
        }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            a.x += b.x;
            a.y += b.y;
            return a;
        }

        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            a.x -= b.x;
            a.y -= b.y;
            return a;
        }

        public static Vector2Int Average(List<Vector2Int> vs)
        {
            if (vs.Count == 0) return new Vector2Int();
            int x = 0;
            int y = 0;
            int n = 0;
            for(int i = 0; i < vs.Count; i++)
            {
                x += vs[i].x;
                y += vs[i].y;
                n++;
            }
            x /= n;
            y /= n;
            return new Vector2Int(x, y);
        }
    }
}
