using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.VisualScripting;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    int vertexIndex;
    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> uvs;

    bool[,,] chunkData = new bool[VoxelData.ChunkSize, VoxelData.ChunkSize, VoxelData.ChunkSize];

    void Start()
    {
        // Temp.
        for (int x = 0; x < VoxelData.ChunkSize; x++)
        {
            for (int y = 0; y < VoxelData.ChunkSize; y++)
            {
                for (int z = 0; z < VoxelData.ChunkSize; z++)
                {
                    chunkData[x, y, z] = UnityEngine.Random.value > 0.8f;
                }
            }
        }
        UpdateMesh();
    }

    private bool Voxel(Vector3 pos)
    {
        int x = (int)MathF.Floor(pos.x);
        int y = (int)MathF.Floor(pos.y);
        int z = (int)MathF.Floor(pos.z);

        if (x < 0 || x > VoxelData.ChunkSize - 1 || y < 0 || y > VoxelData.ChunkSize - 1 || z < 0 || z > VoxelData.ChunkSize - 1)
            return false;

        return chunkData[x, y, z];
    }

    void UpdateMesh()
    {
        vertexIndex = 0;
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        for (int x = 0; x < VoxelData.ChunkSize; x++)
        {
            for (int y = 0; y < VoxelData.ChunkSize; y++)
            {
                for (int z = 0; z < VoxelData.ChunkSize; z++)
                {
                    AddVoxel(new Vector3(x, y, z));
                }
            }
        }

        Mesh mesh = new()
        {
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray()
        };

        mesh.RecalculateNormals();

        var meshFilter = this.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    void AddVoxel(Vector3 pos)
    {
        for (int p = 0; p < 6; p++)
        {
            if (!Voxel(pos + VoxelData.FaceChecks[p]) && Voxel(pos))
            {
                vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[p, 0]]);
                vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[p, 1]]);
                vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[p, 2]]);
                vertices.Add(pos + VoxelData.Vertices[VoxelData.Triangles[p, 3]]);
                uvs.Add(VoxelData.Uvs[0]);
                uvs.Add(VoxelData.Uvs[1]);
                uvs.Add(VoxelData.Uvs[2]);
                uvs.Add(VoxelData.Uvs[3]);
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;
            }
        }
    }
}
