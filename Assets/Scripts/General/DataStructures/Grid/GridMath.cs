using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using AwARe.DataTypes;
using Unity.VisualScripting;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine.Video;
using System.Threading;

namespace AwARe.DataStructures
{
    public static class GridMath
    {
        private static DiscreteRay CreateRay(Vector3 start, Vector3 end, IGridSize gridSize) =>
            CreateRay(start, end, gridSize.GridSize);
        private static DiscreteRay CreateRay(Vector3 start, Vector3 end, Point3 gridSize) =>
            new(start, end, gridSize);
    }
}