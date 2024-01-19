// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using Unity.Collections;

namespace AwARe.RoomScan.Path
{
    /// <summary>
    /// Converts a boolean grid from/to a native grid.
    /// </summary>
    public static class GridConverter
    {
        /// <summary>
        /// Converts a boolean array to a native array.
        /// </summary>
        /// <param name="grid">The array to convert.</param>
        /// <returns>The native array.</returns>
        public static NativeArray<bool> ToNativeGrid(bool[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            int gridSize = rows * cols;

            NativeArray<bool> toNativeGrid = new(gridSize, Allocator.TempJob);

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    int index = x * cols + y;
                    toNativeGrid[index] = grid[x, y];
                }
            }

            return toNativeGrid;
        }

        /// <summary>
        /// Converts a native array to a boolean array.
        /// </summary>
        /// <param name="nativeArray">The array to convert.</param>
        /// <param name="rows">The number of rows of the grid.</param>
        /// <param name="columns">The number of columns of the grid.</param>
        /// <returns>The converted array.</returns>
        public static bool[,] ToGrid(NativeArray<bool> nativeArray, int rows, int columns)
        {
            bool[,] toGrid = new bool[rows, columns];
            int gridSize = nativeArray.Length;

            for (int i = 0; i < gridSize; i++)
            {
                int x = i / columns;
                int y = i % columns;
                toGrid[x, y] = nativeArray[i];
            }

            return toGrid;
        }
    }
}