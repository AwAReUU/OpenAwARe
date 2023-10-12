using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITensor1<Data>
{
    public Data x { get; }
}
public interface ITensor2<Data> : ITensor1<Data>
{
    public Data y { get; }
}

public interface ITensor3<Data> : ITensor2<Data>
{
    public Data z { get; }
}

public interface ITensor4<Data> : ITensor3<Data>
{
    public Data w { get; }
}

public struct Point2
{
    (int, int) coordinates;

    public Point2(int x, int y)
    {
        this.coordinates = (x, y);
    }

    public Point2((int, int) coordinates)
    {
        this.coordinates = coordinates;
    }

    public Point2(int[] coordinates)
    {
        int dim = 2;

        int l = coordinates.Length;
        int[] copy = new int[dim];
        for (int i = 0; i < dim; i++) { copy[i] = (l > i) ? copy[i] : 0; }

        this.coordinates = (copy[0], copy[1]);
    }

    public Point2(Vector2 coordinates)
    {
        this.coordinates = ((int)coordinates.x, (int)coordinates.y);
    }
    public readonly int x => coordinates.Item1;
    public readonly int y => coordinates.Item2;

}

public struct Point3
{
    (int, int, int) coordinates;

    public Point3(int x, int y, int z)
    {
        this.coordinates = (x, y, z);
    }

    public Point3((int, int, int) coordinates)
    {
        this.coordinates = coordinates;
    }

    public Point3(int[] coordinates)
    {
        int dim = 3;

        int l = coordinates.Length;
        int[] copy = new int[dim];
        for (int i = 0; i < dim; i++) { copy[i] = (l > i) ? copy[i] : 0; }

        this.coordinates = (copy[0], copy[1], copy[2]);
    }

    public Point3(Vector3 coordinates)
    {
        this.coordinates = ((int)coordinates.x, (int)coordinates.y, (int)coordinates.z);
    }
    public readonly int x => coordinates.Item1;
    public readonly int y => coordinates.Item2;
    public readonly int z => coordinates.Item3;

}

public struct Point4
{
    (int, int, int, int) coordinates;

    public Point4(int x, int y, int z, int w)
    {
        this.coordinates = (x, y, z, w);
    }

    public Point4((int, int, int, int) coordinates)
    {
        this.coordinates = coordinates;
    }

    public Point4(int[] coordinates)
    {
        int dim = 4;

        int l = coordinates.Length;
        int[] copy = new int[dim];
        for (int i = 0; i < dim; i++) { copy[i] = (l > i) ? copy[i] : 0; }

        this.coordinates = (copy[0], copy[1], copy[2], copy[3]);
    }

    public Point4(Vector4 coordinates)
    {
        this.coordinates = ((int)coordinates.x, (int)coordinates.y, (int)coordinates.z, (int)coordinates.w);
    }

    public readonly int x => coordinates.Item1;
    public readonly int y => coordinates.Item2;
    public readonly int z => coordinates.Item3;
    public readonly int w => coordinates.Item4;

}

