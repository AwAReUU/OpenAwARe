using System;
using System.Collections.Generic;

using AwARe.Data.Logic;

using UnityEngine;

namespace AwARe.Data.Objects
{
    public class VoxelChunk : Chunk<bool>
    {
        const int dim = VoxelData.dim;

        protected int vertexIndex;
        protected List<Vector3> vertices;
        protected List<int> triangles;
        protected List<Vector2> uvs;

        public bool[,,] chunkData;

        // Start is called before the first frame update
        protected override void OnStart()
        {
            this.chunkData = chunk.Data;

            SetStartData();

            UpdateMesh();
        }


        protected override void OnChange()
        {
            UpdateMesh();
        }

        private void SetStartData()
        {
            // Temp.
            for (int x = 0; x < chunkData.GetLength(0); x++)
                for (int y = 0; y < chunkData.GetLength(1); y++)
                    for (int z = 0; z < chunkData.GetLength(2); z++)
                    {
                        chunkData[x, y, z] = UnityEngine.Random.value > 0.8f;
                    }
        }

        private bool Voxel(Vector3 pos)
        {
            float[] xyzf = new float[] { pos.x, pos.y, pos.z };
            int[] xyz = new int[dim];
            for (int i = 0; i < dim; i++)
            {
                xyz[i] = (int)MathF.Floor(xyzf[i]);
                if (xyz[i] < 0 || xyz[i] >= chunkData.GetLength(i))
                    return false;
            }

            return chunkData[xyz[0], xyz[1], xyz[2]];
        }

        void UpdateMesh()
        {
            vertexIndex = 0;
            vertices = new List<Vector3>();
            triangles = new List<int>();
            uvs = new List<Vector2>();

            for (int x = 0; x < chunkData.GetLength(0); x++)
                for (int y = 0; y < chunkData.GetLength(1); y++)
                    for (int z = 0; z < chunkData.GetLength(2); z++)
                        AddVoxel(new Vector3(x, y, z));

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
            var voxel = Voxel(pos);
            for (int p = 0; p < 6; p++)
            {
                var neighbour = Voxel(pos + VoxelData.FaceChecks[p]);
                if (!toRenderFace(voxel, neighbour))
                    continue;

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

        bool toRenderFace(bool voxel, bool neighbour) => voxel && !neighbour;
    }
}


