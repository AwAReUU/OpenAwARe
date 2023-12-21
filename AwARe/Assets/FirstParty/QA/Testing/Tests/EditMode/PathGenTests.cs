using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using AwARe.RoomScan.Path;
using AwARe.RoomScan.Path.Jobs;
using AwARe.RoomScan.Polygons.Objects;
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
        public void Test_LineDrawer_DrawLine(
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

        [Test, Description("Tests if the CheckInPolygon method of the checkinpolygonjob works correctly")]
        [TestCase(0, 0, 0, ExpectedResult = false)]     //on wall intersection
        [TestCase(0, 0, 1, ExpectedResult = false)]     //on wall
        [TestCase(0, 1, 1, ExpectedResult = true)]      //inside
        [TestCase(1, 1, 1, ExpectedResult = true)]      //outside
        [TestCase(1, 5, 5, ExpectedResult = false)]     //inside but on same horizontal line as wall intersection
        [TestCase(1, -1, -1, ExpectedResult = false)]   //outside and on same horizontal line as wall intersection, point outside of grid
        [TestCase(1, 5, 5, ExpectedResult = false)]     //inside but on same horizontal line as wall intersection
        [TestCase(2, 5, 1, ExpectedResult = false)]     //inside but on '.5' intersection
        [TestCase(2, 5, 5, ExpectedResult = false)]      //inside but on '.5' intersection further away. expeced false, but room for improvement in method
        [TestCase(2, 1, 3, ExpectedResult = false)]     //outside but on '.5' intersection with one line
        public bool Test_CheckInPolygonJob(int polygonsindex, int pointx, int pointy)
        {
            makepolygons();
            CheckInPolygonJob job = new();
            return job.CheckInPolygon(polygons[polygonsindex], (pointx, pointy));
        }

        static List<NativeArray<((int, int), (int, int))>> polygons = new();
        private void makepolygons()
        {
            NativeArray<((int, int), (int, int))> polygon1 = new(4, Allocator.Persistent);
            polygon1[0] = ((0, 0), (0, 10));
            polygon1[1] = ((0, 10), (10, 10));
            polygon1[2] = ((10, 10), (10, 0));
            polygon1[3] = ((10, 0), (0, 0));
            polygons.Add(polygon1);

            NativeArray<((int, int), (int, int))> polygon2 = new(4, Allocator.Persistent);
            polygon2[0] = ((0, 0), (10, 5));
            polygon2[1] = ((10, 5), (5, 10));
            polygon2[2] = ((5, 10), (0, 5));
            polygon2[3] = ((0, 5), (0, 0));
            polygons.Add(polygon2);

            NativeArray<((int, int), (int, int))> polygon3 = new(4, Allocator.Persistent);
            polygon3[0] = ((5, 0), (10, 10));
            polygon3[1] = ((10, 10), (5, 10));
            polygon3[2] = ((5, 10), (0, 6));
            polygon3[3] = ((0, 6), (5, 0));
            polygons.Add(polygon3);
        }

        [Test, Description("Test if the FillGrid method works correctly")]
        public void Test_FloodFillHandler_FillGrid()
        {
            //how do test?
            //compute 'volume' of filled polygon vs expected volume? how tho? triangulate, calc triangles, ... difficult
            //make a bunch of mock-polygons and grids and the expected result? extremely tedious //do this one anyway
            //
        }
    }
}
