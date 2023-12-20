using System;
using System.Collections;
using System.Collections.Generic;
using AwARe.RoomScan.Path;
using NUnit.Framework;
using Unity.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PathGenTests
    {

        [Test, Description("Tests if the ToNativeGrid method of the GridcConverter class works correctly")]
        public void Test_GridConverter_ToNativeGrid()
        {
            bool[,] grid = new bool[3, 3] { { false, true, false }, { true, true, true }, { false, false, false } };
            NativeArray<bool> native = GridConverter.ToNativeGrid(grid);
            Assert.IsTrue(native[0] == false);
            Assert.IsTrue(native[1] == true);
            Assert.IsTrue(native[2] == false);
            Assert.IsTrue(native[3] == true);
            Assert.IsTrue(native[4] == true);
            Assert.IsTrue(native[5] == true);
            Assert.IsTrue(native[6] == false);
            Assert.IsTrue(native[7] == false);
            Assert.IsTrue(native[8] == false);
        }

        [Test, Description("Tests if the ToGrid method of the GridConverter class works correctly")]
        public void Test_GridConverter_ToGrid()
        {
            bool[,] grid = new bool[3, 3] { { false, true, false }, { true, true, true }, { false, false, false } };
            NativeArray<bool> native = GridConverter.ToNativeGrid(grid);
            bool[,] grid2 = GridConverter.ToGrid(native, 3, 3);
            Assert.AreEqual(grid, grid2);
        }

        [Test, Description("Tests if the DrawLine method of the LineDrawer class works correctly")]
        //tests: vertical line, horizontal line, other diagonal line, no line (2 times same point), line out of grid bounds, line correct positions (how?)
        //test carve mode results
        public void Test_LineDrawer_DrawLine_Grid1(
            [Values(-1, 0, 1, 5, 10)] int x1,
            [Values(-1, 0, 1, 5, 10)] int y1,
            [Values(-1, 0, 1, 5, 10)] int x2,
            [Values(-1, 0, 1, 5, 10)] int y2,
            [Values(true, false)] bool gridtype
        )
        {
            bool[,] grid = new bool[10, 10];
            if (gridtype)
            {
                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    for (int j = 0; j < grid.GetLength(1); j++) { grid[i, j] = true; }
                }
            }

            LineDrawer.DrawLine(ref grid, ((x1, y1), (x2, y2)), gridtype);

            //here check that shit went well.
            //check no exception thrown
            //check line correct: how?
            //every line pixel should be adjacent to at most 2 others
            //start en endpoint should be true
            //no pixel outside of the 'line bounding box' should be of line value
            //every line pixels minimum distance from line should be no more than 0.5. how compute distance? make second line 90 deg on first through pixel

            //slope = 0: distance = ydiff
            //divzero (slope inf): distance = xdiff
            //otherwise: slope = 1 / -a, gooit door point, find intersection, distance = intersection en pixel diff met pythagoras

            bool divzero = false;
            double divider = x2 - x1;
            double a = 0;
            double b = y1;
            if(divider == 0) {divzero = true;}
            else
            {
                a = (y2 - y1) / divider;
                b = y1 - x1 * a;
            }

            //check if the distance from each pixel that is part of the line to the mathematical line between the points is at most 0.5.
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == gridtype) continue;

                    if (divzero)
                    {
                        //distance = xdiff of line and current pixel
                        double xdiff = Math.Abs(i - x1);
                        Assert.IsTrue(xdiff <= 0.5);
                    }
                    else if (a == 0)
                    {
                        //distance = ydiff of line and current pixel
                        double ydiff = Math.Abs(j - y1);
                        Assert.IsTrue(ydiff <= 0.5);
                    }
                    else
                    {
                        //compute the perpendicular line
                        double a2 = 1 / -a;
                        double b2 = j - i * a2;

                        //compute the intersection
                        double intersectx = (b - b2) / (a2 - a);
                        double intersecty = a * intersectx + b;

                        //compute the distance and check it
                        double distance = Math.Sqrt(Math.Pow(intersectx - i, 2) + Math.Pow(intersecty - j, 2));
                        Assert.IsTrue(distance <= 0.5);
                    }
                }
            }
        }
    }
}
