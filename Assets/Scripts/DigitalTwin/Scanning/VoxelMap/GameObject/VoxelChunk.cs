using System;
using System.Collections.Generic;
using UnityEngine;

using AwARe.MonoBehaviours;
using AwARe.DataTypes;

namespace AwARe.DigitalTwin.VoxelMap.MonoBehaviours
{
    public class VoxelChunk : Chunk<VoxelInfo>
    {
        protected int vertexIndex;
        protected List<Vector3> vertices;
        protected List<int> triangles;
        protected List<Vector2> uvs;

        // Start is called before the first frame update
        protected override void OnStart()
        {
            UpdateMesh();
        }


        protected override void OnChange()
        {
            UpdateMesh();
        }

        private void SetStartData()
        {
            // Temp.
            for (int x = 0; x < chunk.Data.GetLength(0); x++)
                for (int y = 0; y < chunk.Data.GetLength(1); y++)
                    for (int z = 0; z < chunk.Data.GetLength(2); z++)
                        chunk.Data[x, y, z] = new(VoxelValue.Unseen);
        }

        private Voxel Voxel(Point3 pos)
        {
            bool inBounds = pos >= 0 && pos < ChunkSize;
            if (!inBounds) return new(VoxelFill.Empty);

            return (Voxel)chunk[pos];
        }

        void UpdateMesh()
        {
            vertexIndex = 0;
            vertices = new List<Vector3>();
            triangles = new List<int>();
            uvs = new List<Vector2>();

            
            for (int x = 0; x < chunk.Data.GetLength(0); x++)
                for (int y = 0; y < chunk.Data.GetLength(1); y++)
                    for (int z = 0; z < chunk.Data.GetLength(2); z++)
                        AddVoxel(new Point3(x, y, z));
            

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

        void AddVoxel(Point3 pos)
        {
            var voxel = Voxel(pos);
            for (int p = 0; p < 6; p++)
            {
                var neighbour = Voxel((Point3)(pos + VoxelData.FaceChecks[p]));
                if (!ToRenderFace(voxel, neighbour))
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

        bool ToRenderFace(Voxel voxel, Voxel neighbour) => (voxel.fill, neighbour.fill) switch
            {
                (VoxelFill.Filled, VoxelFill.Filled)    => false,
                (VoxelFill.Filled, _)                   => true,
                (VoxelFill.Ghost, VoxelFill.Ghost)      => false,
                (VoxelFill.Ghost, _)                    => true,
                _                                       => false
            };
    }

    public class Voxel
    {
        public VoxelFill fill;

        public Voxel(VoxelFill fill)
        {
            this.fill = fill;
        }

        public static explicit operator Voxel(VoxelInfo info) => info.value switch
        {
            VoxelValue.Solid    => new(VoxelFill.Filled),
            VoxelValue.Air      => new(VoxelFill.Ghost),
            _                   => new(VoxelFill.Empty)
        };
    }

    public enum VoxelFill { Empty, Filled, Ghost }
}


