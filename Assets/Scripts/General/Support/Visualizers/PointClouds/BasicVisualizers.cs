using System;
using UnityEngine;
using Unity.Collections;

namespace AwARe.Visualization
{
    public class PointCloudVisualizer_Particles_Basic : IPointCloudVisualizer
    {
        protected ParticleSystem particleSystem;
        protected Color color;
        protected float scale;

        public PointCloudVisualizer_Particles_Basic(ParticleSystem particleSystem, Color color, float scale)
        {
            this.particleSystem = particleSystem;
            this.color = color;
            this.scale = scale;
        }

        public void Visualize(Vector3[] toShow)
        {
            Debug.Log("PointCloudV");
            int l = toShow.Length;
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[l];

            for (int i = 0; i < l; i++)
            {
                particles[i].position = toShow[i];
                particles[i].startColor = color;
                particles[i].startSize = scale;
            }

            particleSystem.SetParticles(particles, l);
        }

        public void Visualize(NativeArray<Vector3> toShow)
        {
            Debug.Log("PointCloudV");
            int l = toShow.Length;
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[l];

            for (int i = 0; i < l; i++)
            {
                particles[i].position = toShow[i];
                particles[i].startColor = color;
                particles[i].startSize = scale;
            }

            particleSystem.SetParticles(particles, l);
        }

        public void Visualize(NativeSlice<Vector3> toShow)
        {
            int l = toShow.Length;
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[l];
            Debug.Log("PointCloudV: " + l);

            for (int i = 0; i < l; i++)
            {
                Debug.Log("PointCloudV: " + i + " , " + toShow[i]);
                particles[i].position = toShow[i];
                particles[i].startColor = color;
                particles[i].startSize = scale;
            }

            particleSystem.SetParticles(particles, l);
            Debug.Log("PointCloudV: End");
        }

        public void Visualize()
        {
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[0];
            particleSystem.SetParticles(particles, 0);
        }
    }

    public class PointCloudVisualizer_Gizmo_Basic : IPointCloudVisualizer
    {
        protected Color color;
        protected float scale;

        protected Vector3[] positions;

        public Vector3[] Positions { get => positions; }

        public PointCloudVisualizer_Gizmo_Basic(Color color, float scale)
        {
            this.color = color;
            this.scale = scale;
        }

        public void Visualize(Vector3[] toShow)
        {
            Debug.Log("PointCloudV");
            int l = toShow.Length;
            positions = new Vector3[l];

            for (int i = 0; i < l; i++)
            {
                positions[i] = toShow[i];
                Debug.Log("PointCloudV: " + positions[i]);
            }

            Debug.Log("PointCloudV: End");
        }

        public void Visualize(NativeArray<Vector3> toShow)
        {
            Debug.Log("PointCloudV");
            int l = toShow.Length;
            positions = new Vector3[l];

            for (int i = 0; i < l; i++)
            {
                positions[i] = toShow[i];
                Debug.Log("PointCloudV: " + positions[i]);
            }

            Debug.Log("PointCloudV: End");
        }

        public void Visualize(NativeSlice<Vector3> toShow)
        {
            Debug.Log("PointCloudV");
            int l = toShow.Length;
            positions = new Vector3[l];

            for (int i = 0; i < l; i++)
            {
                positions[i] = toShow[i];
                Debug.Log("PointCloudV: " + positions[i]);
            }

            Debug.Log("PointCloudV: End");
        }

        public void Visualize()
        {
            positions = new Vector3[0];
        }
    }

}