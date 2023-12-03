
using UnityEngine;

//using UnityEngine.XR.Interaction.Toolkit.Inputs.Composites;

namespace AwARe.Data.Logic
{
    public static class VoxelData
    {
        public const int dim = 3;
        public const int ChunkSize = 16;

        public static readonly Vector3[] Vertices = new Vector3[8] {
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(1.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(1.0f, 0.0f, 1.0f),
            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(0.0f, 1.0f, 1.0f),
        };

        public static readonly Vector3[] FaceChecks = new Vector3[6] {
            new Vector3(0.0f, 0.0f, -1.0f),
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, -1.0f, 0.0f),
            new Vector3(-1.0f, 0.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 0.0f)
        };

        public static readonly int[,] Triangles = new int[6, 4] {
            {0, 3, 1, 2},   // Back Face
            {5, 6, 4, 7},   // Front Face
            {3, 7, 2, 6},   // Top Face
            {1, 5, 0, 4},   // Bottom Face
            {4, 7, 0, 3},   // Left Face
            {1, 2, 5, 6}    // Right Face
        };

        public static readonly Vector2[] Uvs = new Vector2[4] {
            new Vector2 (0.0f, 0.0f),
            new Vector2 (0.0f, 1.0f),
            new Vector2 (1.0f, 0.0f),
            new Vector2 (1.0f, 1.0f)
        };
    }
}