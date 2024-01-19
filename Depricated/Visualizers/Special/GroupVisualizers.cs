using System;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace AwARe.Visualizers
{
    public class GroupVisualizer<In> : IVisualizer<In[]>
    {
        IVisualizer<In>[] visualizers;

        public GroupVisualizer(IVisualizer<In>[] visualizers)
        {
            this.visualizers = visualizers;
        }

        public void Visualize(In[] toShow)
        {
            int l = Math.Max(toShow.Length, visualizers.Length);
            for (int i = 0; i < l; i++)
                visualizers[i].Visualize(toShow[i]);
        }

        public void Visualize()
        {
            for (int i = 0; i < visualizers.Length; i++)
                visualizers[i].Visualize();
        }
    }

    public class TrackableCollectionVisualizer<In> : IVisualizer<TrackableCollection<In>>
    {
        IVisualizer<In>[] visualizers;

        public TrackableCollectionVisualizer(IVisualizer<In>[] visualizers)
        {
            this.visualizers = visualizers;
        }

        public void Visualize(TrackableCollection<In> toShow)
        {
            int l = visualizers.Length;
            Debug.Log("TrackableCollectionV: " + toShow.count + " , " + l);
            int i = -1;
            foreach (var showable in toShow)
            {
                if (++i >= l) break;
                visualizers[i].Visualize(showable);
            }
        }
        public void Visualize()
        {
            for (int i = 0; i < visualizers.Length; i++)
                visualizers[i].Visualize();
        }
    }

    public class IgnoreOnNullVisualizer<In> : IVisualizer<In?>
        where In : struct
    {
        IVisualizer<In> visualizer;

        public IgnoreOnNullVisualizer(IVisualizer<In> visualizer)
        {
            this.visualizer = visualizer;
        }

        public void Visualize(In? toShow)
        {
            if (!toShow.HasValue)
                return;
            visualizer.Visualize(toShow.Value);
        }

        public void Visualize() => visualizer.Visualize();
    }

    public class HideOnNullVisualizer<In> : IVisualizer<In?>
        where In : struct
    {
        IVisualizer<In> visualizer;

        public HideOnNullVisualizer(IVisualizer<In> visualizer)
        {
            this.visualizer = visualizer;
        }

        public void Visualize(In? toShow)
        {
            Debug.Log("HideOnNullV");
            if (!toShow.HasValue)
            {
                visualizer.Visualize();
                return;
            }

            visualizer.Visualize(toShow.Value);
        }

        public void Visualize() => visualizer.Visualize();
    }
}