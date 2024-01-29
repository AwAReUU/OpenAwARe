// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections;
using System.Collections.Generic;
using AwARe.Data.Logic;
using AwARe.Testing;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

namespace AwARe.Tests.General
{
    /// <summary>
    /// Test regarding the <see cref="Polygon"/> class.
    /// </summary>
    public class PolygonData_Tests
    {
        private const float EPS = 0.0001f;

        public static IEnumerable TestCases_Points
        {
            get
            {
                yield return new TestCaseData(new List<Vector3> { });
                yield return new TestCaseData(new List<Vector3> { new(0, 0, 0), new(1, 0, 0), new(0, 0, 1) });
            }
        }

        [Test, Description("Getting points (Vector3) of the polygon.")]
        [TestCaseSource(nameof(TestCases_Points))]
        public void Test_GetPoints(List<Vector3> points)
        {
            // Arrange
            Polygon polygon = new(points: points);

            // Act & Assert
            Assert.AreEqual(points, polygon.points);
        }

        [Test, Description("Setting points (Vector3) of the polygon.")]
        [TestCaseSource(nameof(TestCases_Points))]
        public void Test_SetPoints(List<Vector3> points)
        {
            // Arrange
            Polygon polygon = new();

            // Act
            polygon.points = points;

            // Assert
            Assert.AreEqual(points, polygon.points);
        }
        
        public static IEnumerable TestCases_Height
        {
            get
            {
                yield return new TestCaseData(1f);
                yield return new TestCaseData(7f);
            }
        }

        [Test, Description("Getting height (float) of the polygon.")]
        [TestCaseSource(nameof(TestCases_Height))]
        public void Test_GetHeight(float height)
        {
            // Arrange
            Polygon polygon = new(height: height);

            // Act & Assert
            Assert.AreEqual(height, polygon.height);
        }
        
        [Test, Description("Setting height (float) of the polygon.")]
        [TestCaseSource(nameof(TestCases_Height))]
        public void Test_SetHeight(float height)
        {
            // Arrange
            Polygon polygon = new();

            // Act
            polygon.height = height;

            // Assert
            Assert.AreEqual(height, polygon.height);
        }

        public static IEnumerable TestCases_Area
        {
            get
            {
                // Zero-cases
                yield return new TestCaseData(new List<Vector3> { }, 0f);
                yield return new TestCaseData(new List<Vector3> { new(0, 0, 0) }, 0f);
                yield return new TestCaseData(new List<Vector3> { new(0, 0, 0), new(1, 0, 0) }, 0f);
                yield return new TestCaseData(new List<Vector3> { new(0, 0, 0), new(1, 0, 0), new(-1, 0, 0) }, 0f);

                // Less trivial cases
                yield return new TestCaseData(new List<Vector3> { new(0, 0, 0), new(1, 0, 0), new(0, 0, 1) }, 0.5f);
                yield return new TestCaseData(new List<Vector3> { new(0, 0, 0), new(1, 0, 0), new(0, 0, -1) }, 0.5f);
                yield return new TestCaseData(new List<Vector3> { new(0, 0, 0), new(1, 0, 0), new(0.1f, 0, 0.1f), new(0, 0, 1) }, 0.1f);
            }
        }
        
        [Test, Description("Getting area (float) of the polygon.")]
        [TestCaseSource(nameof(TestCases_Area))]
        public void Test_GetArea(List<Vector3> points, float expected)
        {
            // Arrange
            Polygon polygon = new(points: points);

            // Act & Assert
            Assert.That(polygon.Area, Is.EqualTo(expected).Within(EPS));
        }

        [Test, Description("Deep cloning polygon.")]
        public void Test_Clone()
        {
            // Arrange
            Polygon polygon = new();

            // Act
            Polygon clone = polygon.Clone();

            // Assert
            Assert.AreNotSame(polygon, clone);
            Assert.AreNotSame(polygon.points, clone.points);
        }
    }
    
    // NOTE: Desired result of the mesh is not fully defined yet.
    /// <summary>
    /// Test regarding the <see cref="PolygonMesherLogic"/> class.
    /// </summary>
    public class PolygonMesherLogic_Tests
    {
        public static IEnumerable TestCases_Mesh
        {
            get
            {
                yield return
                    new TestCaseData(
                        new Polygon(new()),
                        new Vector3[] { },
                        false
                    );

                //yield return
                //    new TestCaseData(
                //        new Polygon(new() { new(0, 0, 0) }, 1f),
                //        new Vector3[] { new(0, 0, 0), new(0, 1, 0) },
                //        false
                //    );

                yield return
                    new TestCaseData(
                        new Polygon(new() { new(0, 0, 0), new(1, 0, 0), new(0, 0, 1) }, 2f),
                        new Vector3[] { new(0, 0, 0), new(1, 0, 0), new(0, 0, 1), new(0, 2, 0), new(1, 2, 0), new(0, 2, 1), },
                        true
                    );
            }
        }
        
        // NOTE: Desired result of the mesh is not fully defined yet.
        [Test, Description("Getting the boundary line of the polygon.")]
        [TestCaseSource(nameof(TestCases_Mesh))]
        public void Test_GetMesh(Polygon data, Vector3[] expectedVertices, bool trianglesExpected)
        {
            // Arrange
            var @interface = Substitute.For<IDataHolder<Polygon>>();
            var reference = new SubstituteReference<IDataHolder<Polygon>>(@interface);
            @interface.Data.Returns(data);

            var obj = new GameObject("Logic");
            PolygonMesherLogic logic = PolygonMesherLogic.AddComponentTo(obj, reference);

            // Act
            Mesh mesh = logic.Mesh;

            // Assert
            Assert.That(expectedVertices, Is.EquivalentTo(mesh.vertices));
            Assert.AreEqual(trianglesExpected, mesh.triangles.Length != 0);
        }
    }
    
    /// <summary>
    /// Test regarding the <see cref="PolygonLinerLogic"/> class.
    /// </summary>
    public class PolygonLinerLogic_Tests
    {
        public static IEnumerable TestCases_Line
        {
            get
            {
                yield return
                    new TestCaseData(
                        new Polygon(new() { new(0, 0, 0), new(1, 0, 0), new(0, 0, 1) }),
                        false,
                        new Vector3[] { new(0, 0, 0), new(1, 0, 0), new(0, 0, 1) }
                    );
                
                yield return
                    new TestCaseData(
                        new Polygon(new() { new(0, 0, 0), new(1, 0, 0), new(0, 0, 1) }),
                        true,
                        new Vector3[] { new(0, 0, 0), new(1, 0, 0), new(0, 0, 1), new(0, 0, 0) }
                    );
            }
        }

        [Test, Description("Getting the boundary line of the polygon.")]
        [TestCaseSource(nameof(TestCases_Line))]
        public void Test_GetLine(Polygon data, bool closedLine, Vector3[] expected)
        {
            // Arrange
            var @interface = Substitute.For<IDataHolder<Polygon>>();
            var reference = new SubstituteReference<IDataHolder<Polygon>>(@interface);
            @interface.Data.Returns(data);

            var obj = new GameObject("Logic");
            var logic = PolygonLinerLogic.AddComponentTo(obj, reference, closedLine);

            // Act
            Vector3[] line = logic.Line;
            foreach(var point in line)
                Debug.Log(point);

            // Assert
            Assert.AreEqual(expected, line);
        }
    }

    /// <summary>
    /// Test regarding the <see cref="Room"/> class.
    /// </summary>
    public class RoomData_Tests
    {
        public static IEnumerable TestCases_Polygons
        {
            get
            {
                yield return new TestCaseData(new Polygon(height: 2f), new List<Polygon>{new(height: 3f), new(height: 4f)});
            }
        }

        [Test, Description("Getting the positive polygon of the room.")]
        [TestCaseSource(nameof(TestCases_Polygons))]
        public void Test_GetPositive(Polygon positive, List<Polygon> negatives)
        {
            // Arrange
            Room room = new(posPolygon: positive, negPolygons: negatives);

            // Act & Assert
            Assert.AreEqual(positive, room.PositivePolygon);
        }
        
        [Test, Description("Setting the positive polygon of the room.")]
        [TestCaseSource(nameof(TestCases_Polygons))]
        public void Test_SetPositive(Polygon positive, List<Polygon> negatives)
        {
            // Arrange
            Room room = new(posPolygon: positive, negPolygons: negatives);

            // Act
            room.PositivePolygon = positive;

            // Assert
            Assert.AreEqual(positive, room.PositivePolygon);
        }

        [Test, Description("Getting the negative polygon of the room.")]
        [TestCaseSource(nameof(TestCases_Polygons))]
        public void Test_GetNegatives(Polygon positive, List<Polygon> negatives)
        {
            // Arrange
            Room room = new(posPolygon: positive, negPolygons: negatives);

            // Act & Assert
            Assert.AreEqual(negatives, room.NegativePolygons);
        }

        [Test, Description("Getting the negative polygon of the room.")]
        [TestCaseSource(nameof(TestCases_Polygons))]
        public void Test_SetNegatives(Polygon positive, List<Polygon> negatives)
        {
            // Arrange
            Room room = new(posPolygon: positive, negPolygons: negatives);

            // Act
            room.NegativePolygons = negatives;

            // Assert
            Assert.AreEqual(negatives, room.NegativePolygons);
        }

        [Test, Description("Deep cloning room.")]
        public void Test_Clone()
        {
            // Arrange
            Room room = new();

            // Act
            Room clone = room.Clone();

            // Assert
            Assert.AreNotSame(room, clone);
            Assert.AreNotSame(room.NegativePolygons, clone.NegativePolygons);
        }
    }
}