using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AwARe.Data.Logic;

using UnityEngine;

namespace AwARe
{
    public class JsonSerializationTest : MonoBehaviour
    {
        private void Start()
        {
            TestDeserialization();
        }

        private void TestDeserialization()
        {
            // Create a test Polygon
            Polygon originalPolygon = new Polygon();
            originalPolygon.AddPoint(new Vector3(1.0f, 2.0f, 3.0f));
            originalPolygon.AddPoint(new Vector3(4.0f, 5.0f, 6.0f));

            // Serialize the test Polygon
            string serializedJson = originalPolygon.ToJson();
            Debug.Log($"Serialized JSON: {serializedJson}");

            // Deserialize the JSON back to a Polygon
            Polygon deserializedPolygon = Polygon.FromJson(serializedJson);

            // Check if deserializedPolygon is equal to originalPolygon
            if (ArePolygonsEqual(originalPolygon, deserializedPolygon))
            {
                Debug.Log("Deserialization Test Passed!");
            }
            else
            {
                Debug.LogError("Deserialization Test Failed!");
            }
        }

        private bool ArePolygonsEqual(Polygon polygon1, Polygon polygon2)
        {
            // check if two polygons are equal
            
            return polygon1.AmountOfPoints() == polygon2.AmountOfPoints() &&
                polygon1.GetPoints().SequenceEqual(polygon2.GetPoints());
        }
    }
}
