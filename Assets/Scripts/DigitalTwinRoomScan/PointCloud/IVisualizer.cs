using UnityEngine;
using Unity.Collections;

public interface IVisualizer<In>
{
    public void Visualize(In toShow);
    public void Visualize();
}

public interface IPointCloudVisualizer : IVisualizer<Vector3[]>, IVisualizer<NativeArray<Vector3>>, IVisualizer<NativeSlice<Vector3>>
{ }
