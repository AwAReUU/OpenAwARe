using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;
using AwARe.Data.Logic;
using AwARe.RoomScan.Path;
using AwARe.RoomScan.Path.Jobs;
using AwARe.RoomScan.Polygons.Objects;
using JetBrains.Annotations;
using NUnit.Framework;
using NUnit.Framework.Internal;
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
            //arrange
            bool[,] grid = new bool[3, 3] { { false, true, false }, { true, true, true }, { false, false, false } };
            //act
            NativeArray<bool> native = GridConverter.ToNativeGrid(grid);
            //assert
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
            //arrange
            bool[,] grid = new bool[3, 3] { { false, true, false }, { true, true, true }, { false, false, false } };
            //act
            NativeArray<bool> native = GridConverter.ToNativeGrid(grid);
            bool[,] grid2 = GridConverter.ToGrid(native, 3, 3);
            //assert
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
            //arrange
            bool[,] grid = new bool[10, 10];
            if (gridtype)
            {
                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    for (int j = 0; j < grid.GetLength(1); j++) { grid[i, j] = true; }
                }
            }

            //compute some values that will be used for asserting
            bool divZero = false;
            double divider = x2 - x1;
            double a = 0;
            double b = y1;
            if(divider == 0) {divZero = true;}
            else
            {
                a = (y2 - y1) / divider;
                b = y1 - x1 * a;
            }

            //act
            LineDrawer.DrawLine(ref grid, ((x1, y1), (x2, y2)), gridtype);

            //assert
            //check if the distance from each pixel that is part of the line to the mathematical line between the points is at most 0.5.
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == gridtype) continue;

                    if (divZero)
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
        [TestCase(2, 5, 5, ExpectedResult = false)]     //inside but on '.5' intersection further away. expeced false, but room for improvement in method
        [TestCase(2, 1, 3, ExpectedResult = false)]     //outside but on '.5' intersection with one line
        public bool Test_CheckInPolygonJob(int polygonsIndex, int pointx, int pointy)
        {
            //arrange
            MakePolygons();
            //act & assert
            CheckInPolygonJob job = new();
            return job.CheckInPolygon(polygons[polygonsIndex], (pointx, pointy));
        }

        static List<NativeArray<((int, int), (int, int))>> polygons = new();
        private void MakePolygons()
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
            //arrange
            PolygonLines positive = new();
            positive.Add(((5, 0), (10, 10)));
            positive.Add(((10, 10), (5, 10)));
            positive.Add(((5, 10), (0, 6)));
            positive.Add(((0, 6), (5, 0)));

            PolygonLines negative = new();
            negative.Add(((3, 4), (3, 6)));
            negative.Add(((3, 6), (5, 6)));
            negative.Add(((5, 6), (5, 4)));
            negative.Add(((5, 4), (3, 4)));

            FloodFillHandler handler = new();
            bool[,] grid = new bool[11, 11];
            List<PolygonLines> negatives = new();
            negatives.Add(negative);

            bool[,] expectedresult = new bool[11, 11] 
            { 
                { false, false, false, false, false, true, false, false, false, false, false},
                { false, false, false, false, true, true, false, false, false, false, false},
                { false, false, false, true, true, true, true, false, false, false, false},
                { false, false, true, true, true, true, true, false, false, false, false},
                { false, false, true, false, false, false, true, true, false, false, false},
                { false, true, true, false, false, false, true, true, false, false, false},
                { true, true, true, false, false, false, true, true, true, false, false},
                { false, true, true, true, true, true, true, true, true, false, false},
                { false, false, true, true, true, true, true, true, true, true, false},
                { false, false, false, false, true, true, true, true, true, true, false},
                { false, false, false, false, false, true, true, true, true, true, true},
            };

            //act
            handler.FillGrid(ref grid, positive, negatives);

            //assert
            for(int x = 0; x < grid.GetLength(0); x++)
            {
                for(int y = 0; y < grid.GetLength(1); y++)
                {
                    Debug.Log(x + " , " + y);
                    Assert.IsTrue(grid[x, y] == expectedresult[y, x]);
                }
            }
        }

        [Test, Description("Tests if the Erode method from the ErosionHandler works correctly")]
        public void Test_ErosionHandler_Erode()
        {
            //arrange
            bool[,] input = new bool[5, 9]
            {
                {true, true, true, true, true, false, false, false, false},
                {true, true, true, true, true, false, true, true, true},
                {true, true, true, true, true, true, true, true, true},
                {true, true, true, true, true, true, true, true, true},
                {true, true, true, true, true, false, true, true, true},
            };

            bool[,] expectedOutput = new bool[5, 9]
            {
                {false, false, false, false, false, false, false, false, false},
                {false, true, true, true, false, false, false, false, false},
                {false, true, true, true, false, false, false, true, false},
                {false, true, true, true, false, false, false, true, false},
                {false, false, false, false, false, false, false, false, false}
            };

            ErosionHandler handler = new();

            //act
            bool[,] output = handler.Erode(input, 3);

            //assert
            for(int x = 0; x < 5; x++)
            {
                for(int y = 0; y < 9; y++)
                {
                    Assert.IsTrue(output[x, y] == expectedOutput[x, y]);
                }
            }
        }

        [Test, Description("Tests if the KeepLargestShape method from the ErosionHandler works correctly")]
        public void Test_ErosionHandler_KeepLargestShape()
        {
            //arrange
            bool[,] input = new bool[5, 9]
            {
                {false, false, false, false, false, false, false, false, false},
                {false, true, true, true, false, false, false, false, false},
                {false, true, true, true, false, false, false, true, false},
                {false, true, true, true, false, false, false, true, false},
                {false, false, false, false, false, false, false, false, false}
            };

            bool[,] expectedOutput = new bool[5, 9]
            {
                {false, false, false, false, false, false, false, false, false},
                {false, true, true, true, false, false, false, false, false},
                {false, true, true, true, false, false, false, false, false},
                {false, true, true, true, false, false, false, false, false},
                {false, false, false, false, false, false, false, false, false}
            };

            ErosionHandler handler = new();

            //act
            bool[,] output = handler.KeepLargestShape(input);

            //assert
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    Assert.IsTrue(output[x, y] == expectedOutput[x, y]);
                }
            }
        }

        [Test, Description("Tests if the ThinningHandler works correctly")]
        public void Test_ThinningHandler_Thinnedgrid()
        {
            //arrange
            bool[,] input = new bool[6, 11]
            {
                {true, true, true, true, true, false, true, true, true, true, true},
                {true, true, true, true, true, true, true, true, true, true, true},
                {true, true, true, true, true, true, true, true, true, true, true},
                {true, true, true, true, true, true, true, true, true, true, true},
                {true, true, true, true, true, true, true, true, true, true, true},
                {true, true, true, true, true, true, true, true, true, true, true}
            };

            bool[,] expectedOutput = new bool[6, 11]
            {
                {true, false, false, false, false, false, false, false, false, false, true},
                {true, true, true, true, true, true, true, true, true, true, true},
                {false, true, true, true, true, true, true, true, true, true, false},
                {false, true, true, true, true, true, true, true, true, true, false},
                {true, true, true, true, true, true, true, true, true, true, false},
                {true, false, false, false, false, false, false, false, false, true, true}
            };

            ThinningHandler handler = new();
            bool outbool;
            //act
            bool[,] output = handler.ThinnedGrid(input, out outbool);

            //assert
            for(int x = 0; x < 6; x++)
            {
                for(int y = 0; y < 11; y++)
                {
                    Assert.IsTrue(output[x, y] == expectedOutput[x, y]);
                }
            }
            Assert.IsTrue(outbool == true);
        }

        [Test, Description("Tests if the 'out' bool parameter of the ThinningHandler works correctly")]
        public void Test_ThinningHandler_Outbool()
        {
            //arrange
            ThinningHandler handler = new();
            bool outbool;
            bool[,] input = new bool[2, 2] { { true, false }, { true, true } };
            bool[,] expectedOutput = new bool[2, 2] { { true, false }, { true, true } };
            //act
            bool[,] output = handler.ThinnedGrid(input, out outbool);
            //assert
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    Assert.IsTrue(output[x, y] == expectedOutput[x, y]);
                }
            }
            Assert.IsTrue(outbool == false);
        }

        //      potentially alter postfiltering to plus shape neighbourhood in stead of 8-neighbourhood, and see with some polygons and see if still works?
        //          above is method improvement tho, not testing
        [Test, Description("Tests that the PostFilteringHandler works correctly")]
        public void Test_PostFiltering_1()
        {
            //arrange
            bool[,] input = new bool[5, 10]
            {
                {true, true, false, false, false, false, false, false, true, true},
                {false, true, true, false, false, false, false, true, true, false},
                {false, false, true, true, true, true, true, true, false, false},
                {false, true, true, false, false, false, false, true, true, false},
                {true, true, false, false, false, false, false, false, true, true}
            };

            bool[,] expectedOutputCut = new bool[5, 10]
            {
                {false, false, false, false, false, false, false, false, false, false},
                {false, false, false, false, false, false, false, false, false, false},
                {false, false, true, true, true, true, true, true, false, false},
                {false, false, true, false, false, false, false, true, false, false},
                {false, false, false, false, false, false, false, false, false, false}
            };

            bool[,] secondInput = new bool[5, 10]
            {
                {false, false, false, false, false, false, false, false, true, true},
                {false, true, true, false, false, false, false, true, true, false},
                {false, false, true, true, true, true, true, true, false, false},
                {false, true, true, false, false, false, false, true, true, false},
                {true, true, false, false, false, false, false, false, true, true}
            };

            bool[,] secondExpectedOutput = new bool[5, 10]
            {
                {false, false, false, false, false, false, false, false, false, false},
                {false, false, true, false, false, false, false, false, false, false},
                {false, false, true, true, true, true, true, true, false, false},
                {false, false, true, false, false, false, false, true, false, false},
                {false, false, false, false, false, false, false, false, false, false}
            };

            Polygon negative = new Polygon();
            negative.AddPoint(new Vector3(-1, 0, -1));
            negative.AddPoint(new Vector3(-1, 0, 6));
            negative.AddPoint(new Vector3(6, 0, 6));
            negative.AddPoint(new Vector3(6, 0, -1));

            List<Polygon> negatives = new List<Polygon>();
            negatives.Add(negative);
            PostFilteringHandler handler = new();
            //act
            handler.PostFiltering(ref input, 3, 0, new());
            handler.PostFiltering(ref secondInput, 3, 0, negatives);
            //assert
            for(int x = 0; x < 5; x++)
            {
                for(int y = 0; y < 10; y++)
                {
                    Assert.IsTrue(input[x, y] == expectedOutputCut[x, y]);
                    Assert.IsTrue(secondInput[x, y] == secondExpectedOutput[x, y]);
                }
            }
        }

        [Test, Description("Tests that the PostFilteringHandler works correctly")]
        public void Test_PostFiltering_2()
        {
            //arrange
            bool[,] input = new bool[5, 10]
            {
                {true, true, false, false, false, false, false, false, true, true},
                {false, true, true, false, false, false, false, true, true, false},
                {false, false, true, true, true, true, true, true, false, false},
                {false, true, true, false, false, false, false, true, true, false},
                {true, true, false, false, false, false, false, false, true, true}
            };

            bool[,] expectedOutputMerge = new bool[5, 10]
            {
                {true, true, false, false, false, false, false, false, false, false},
                {false, true, true, false, false, false, false, false, false, false},
                {false, false, true, true, true, true, true, true, true, true},
                {false, true, true, false, false, false, false, true, false, false},
                {true, true, false, false, false, false, false, false, false, false}
            };

            Polygon negative = new Polygon();
            negative.AddPoint(new Vector3(-1, 0, -1));
            negative.AddPoint(new Vector3(-1, 0, 6));
            negative.AddPoint(new Vector3(6, 0, 6));
            negative.AddPoint(new Vector3(6, 0, -1));

            List<Polygon> negatives = new List<Polygon>();
            negatives.Add(negative);
            PostFilteringHandler handler = new();

            //act
            handler.PostFiltering(ref input, 0, 3, negatives);
            //assert
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    Assert.IsTrue(input[x, y] == expectedOutputMerge[x, y]);
                }
            }
        }

        [Test, Description("Tests that the CreatGrid method of the PathGenerator works correctly")]
        public void Test_PathGenerator_CreateGrid_1()
        {
            //arrange
            PathGenerator generator = new();
            MethodInfo createGridMethod = generator.GetType().GetMethod("CreateGrid", BindingFlags.NonPublic | BindingFlags.Instance);

            Polygon polygon1 = new();
            polygon1.AddPoint(new Vector3(-2, 0, 8));
            polygon1.AddPoint(new Vector3(-2, 0, 3));
            polygon1.AddPoint(new Vector3(3, 0, 3));
            polygon1.AddPoint(new Vector3(4, 0, -2));
            polygon1.AddPoint(new Vector3(7, 0, 7));

            //act
            var returnvalue = createGridMethod.Invoke(generator, new object[2] {polygon1, 500});
            bool[,] grid = (bool[,])returnvalue;

            //assert
            Assert.IsTrue(grid.GetLength(0) == 451);
            Assert.IsTrue(grid.GetLength(1) == 501);
        }

        [Test, Description("Tests that the CreatGrid method of the PathGenerator works correctly")]
        public void Test_PathGenerator_CreateGrid_2()
        {
            //arrange
            PathGenerator generator = new();
            MethodInfo createGridMethod = generator.GetType().GetMethod("CreateGrid", BindingFlags.NonPublic | BindingFlags.Instance);

            Polygon polygon2 = new();
            polygon2.AddPoint(new Vector3(1, 0, 1));
            polygon2.AddPoint(new Vector3(1, 0, 3));
            polygon2.AddPoint(new Vector3(4, 0, 3));
            polygon2.AddPoint(new Vector3(4, 0, 1));

            //act
            var returnvalue = createGridMethod.Invoke(generator, new object[2] { polygon2, 500 });
            bool[,] grid = (bool[,])returnvalue;

            //assert
            Assert.IsTrue(grid.GetLength(0) == 301);
            Assert.IsTrue(grid.GetLength(1) == 201);
        }

        [Test, Description("Tests that the CreatGrid method of the PathGenerator works correctly")]
        public void Test_PathGenerator_CreateGrid_3()
        {
            //arrange
            PathGenerator generator = new();
            MethodInfo createGridMethod = generator.GetType().GetMethod("CreateGrid", BindingFlags.NonPublic | BindingFlags.Instance);

            Polygon polygon3 = new();
            polygon3.AddPoint(new Vector3(1, 0, 1));
            polygon3.AddPoint(new Vector3(1, 0, 3));
            polygon3.AddPoint(new Vector3(7, 0, 3));
            polygon3.AddPoint(new Vector3(7, 0, 1));

            //act
            var returnvalue = createGridMethod.Invoke(generator, new object[2] { polygon3, 500 });
            bool[,] grid = (bool[,])returnvalue;

            //assert
            Assert.IsTrue(grid.GetLength(0) == 501);
            Assert.IsTrue(grid.GetLength(1) == 168);
        }

        [Test, Description("Tests that the GetGridLines method of the PathGenerator works correctly")]
        public void Test_PathGenerator_GetGridLines()
        {
            //arrange
            PathGenerator generator = new();
            MethodInfo gridLinesMethod = generator.GetType().GetMethod("GetGridlines", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo createGridMethod = generator.GetType().GetMethod("CreateGrid", BindingFlags.NonPublic | BindingFlags.Instance);

            Polygon positive = new();
            positive.AddPoint(new Vector3(1, 0, 1));
            positive.AddPoint(new Vector3(1, 0, 3));
            positive.AddPoint(new Vector3(3, 0, 4));
            positive.AddPoint(new Vector3(3, 0, 2));

            List<Polygon> negatives = new();
            Polygon negative1 = new();
            negative1.AddPoint(new Vector3(2, 0, 2));
            negative1.AddPoint(new Vector3(4, 0, 2));
            negative1.AddPoint(new Vector3(4, 0, 3));
            negative1.AddPoint(new Vector3(2, 0, 3));
            negatives.Add(negative1);

            List<((int, int), (int, int))> expectedPositiveLines = new();
            expectedPositiveLines.Add(((0, 0), (0, 200)));
            expectedPositiveLines.Add(((0, 200), (200, 300)));
            expectedPositiveLines.Add(((200, 300), (200, 100)));
            expectedPositiveLines.Add(((200, 100), (0, 0)));

            List<((int, int), (int, int))> expectedNegativeLines = new();
            expectedNegativeLines.Add(((100, 100), (300 ,100)));
            expectedNegativeLines.Add(((300, 100), (300, 200)));
            expectedNegativeLines.Add(((300, 200), (100, 200)));
            expectedNegativeLines.Add(((100, 200), (100, 100)));

            //act
            createGridMethod.Invoke(generator, new object[2] { positive, 500 });
            var returnvalue = gridLinesMethod.Invoke(generator, new object[2] { positive, negatives });
            (PolygonLines, List<PolygonLines>) lines = ((PolygonLines, List<PolygonLines>))returnvalue;
            List<((int, int), (int, int))> positiveLines = lines.Item1.Lines;
            List<((int, int), (int, int))> negativeLines = lines.Item2[0].Lines;

            //assert
            for(int i = 0; i < positiveLines.Count; i++)
            {
                Assert.IsTrue(positiveLines[i] == expectedPositiveLines[i]);
            }

            for (int i = 0; i < negativeLines.Count; i++)
            {
                Assert.IsTrue(negativeLines[i] == expectedNegativeLines[i]);
            }
        }

        [Test, Description("Tests the GeneratePath method of the PathGenerator. Tests if the entire path generation pipeline works, from polygon to path.")]
        public void Test_PathGenerator_GeneratePath()
        {
            //arrange
            PathGenerator generator = new();
            MethodInfo toPathMethod = generator.GetType().GetMethod("ConvertToPath", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo createGridMethod = generator.GetType().GetMethod("CreateGrid", BindingFlags.NonPublic | BindingFlags.Instance);

            Polygon positive = new Polygon();
            positive.AddPoint(new Vector3(0, 0, 0));
            positive.AddPoint(new Vector3(0, 0, 0.6f));
            positive.AddPoint(new Vector3(0.6f, 0, 0.6f));
            positive.AddPoint(new Vector3(0.6f, 0, 0));

            bool[,] expectedresultgrid = new bool[61, 61];
            expectedresultgrid[30, 30] = true;
            expectedresultgrid[31, 30] = true;
            expectedresultgrid[31, 31] = true;

            createGridMethod.Invoke(generator, new object[2] { positive, 500 });
            PathData expectedresultpath = (PathData)toPathMethod.Invoke(generator, new object[1] { expectedresultgrid });

            //act
            PathData result = generator.GeneratePath(positive, new());

            //assert
            for (int i = 0; i < result.edges.Count; i++)
            {
                Assert.IsTrue(expectedresultpath.edges[i] == result.edges[i]);
            }

            for (int i = 0; i < result.points.Count; i++)
            {
                Assert.IsTrue(expectedresultpath.points[i] == result.points[i]);
            }
        }
    }
}
