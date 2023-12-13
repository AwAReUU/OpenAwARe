using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwARe.RoomScan.Path
{
    public static class LineDrawer
    {

        /// <summary>
        /// draw a line of 'true' values between 2 points on a given grid of booleans.
        /// </summary>
        /// <param name="grid">grid of booleans to draw the line on.</param>
        /// <param name="linepoints"> line to draw. points are coordinates in the grid. </param>
        public static void DrawLine(ref bool[,] grid, ((int, int), (int, int)) linepoints, bool carve = false)
        {
            bool setToValue = !carve;

            int x1 = linepoints.Item1.Item1;
            int y1 = linepoints.Item1.Item2;
            int x2 = linepoints.Item2.Item1;
            int y2 = linepoints.Item2.Item2;

            float xdiff = x2 - x1;
            float ydiff = y2 - y1;
            float a;
            float c;

            if (xdiff == 0)
                a = 0;
            else
                a = ydiff / xdiff;
            if (ydiff == 0)
                c = 0;
            else
                c = xdiff / ydiff;

            float b = (float)y1 - a * (float)x1;
            float d = (float)x1 - c * (float)y1;

            for (int x = Math.Min(x1, x2); x < Math.Max(x1, x2); x++)
            {
                if (x < 0 || x > grid.GetLength(0)) continue;

                float y = a * x + b;
                if (y - (int)y > 0.5)
                {
                    if (y + 1 < 0 || y + 1 > grid.GetLength(1)) continue;
                    grid[x, (int)(y + 1)] = setToValue;
                }
                else
                {
                    if (y < 0 || y > grid.GetLength(1)) continue;
                    grid[x, (int)y] = setToValue;
                }
            }

            for (int y = Math.Min(y1, y2); y < Math.Max(y1, y2); y++)
            {
                if (y < 0 || y > grid.GetLength(1)) continue;

                float x = c * y + d;
                if (x - (int)x > 0.5)
                {
                    if (x + 1 < 0 || x + 1 > grid.GetLength(0)) continue;
                    grid[(int)(x + 1), y] = setToValue;
                }
                else
                {
                    if (x < 0 || x > grid.GetLength(0)) continue;
                    grid[(int)x, y] = setToValue;
                }
            }
        }
    }
}