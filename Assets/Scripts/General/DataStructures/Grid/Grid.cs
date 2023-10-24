using AwARe.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwARe.DataStructures
{
    public interface IGridSize
    {
        public Point3 GridSize { get; }
    }

    public interface IGrid<T>
    {
        public T this[int x, int y, int z] { get; set; }
        public T this[Point3 idx] { get; set; }
        public int GetLength(int dim);
    }
}
