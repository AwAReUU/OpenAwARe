// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using Unity.Collections;
using Unity.Jobs;

namespace AwARe.RoomScan.Path.Jobs
{
    /// <summary>
    /// Job for calculating eroded value of a cell.
    /// </summary>
    public struct ErosionScanJob : IJobParallelFor
    {
        /// <summary> The input array. </summary>
        [ReadOnly] public NativeArray<bool> input;

        /// <summary> The range of the erosion. </summary>
        [ReadOnly] public int range;

        /// <summary> The number of rows of the grid. </summary>
        [ReadOnly] public int rows;

        /// <summary> The number of columns of the grid. </summary>
        [ReadOnly] public int columns;

        /// <summary> The range of the erosion divided by 2. </summary>
        [ReadOnly] public int halfRange;

        /// <summary> The resulting array. </summary>
        [WriteOnly] public NativeArray<bool> result;

        /// <inheritdoc/>
        public void Execute(int index)
        {
            Func<(int, int), bool> inputFunction = GetInputFunction(input, rows, columns);

            result[index] = All(index, inputFunction);
        }

        /// <summary>
        /// Checks whether all elements within the range around the index are true.
        /// </summary>
        /// <param name="index">The index of the cell that is being checked.</param>
        /// <param name="inputFunction">The function that is applied to the cells to determine their value.</param>
        /// <returns>False if any value within the range is false, true otherwise.</returns>
        private readonly bool All(int index, Func<(int, int), bool> inputFunction)
        {
            int x = index / columns;
            int y = index % columns;

            // If one element is false, return false, otherwise return true.
            for (int i = 0, x_2 = x - halfRange; i < range; i++, x_2++)
                for (int j = 0, y_2 = y - halfRange; j < range; j++, y_2++)
                {
                    if (!inputFunction((x_2, y_2)))
                        return false;
                }
            return true;
        }

        /// <summary>
        /// Uses an array to create a function for determining the value of a bit in the bitmap.
        /// </summary>
        /// <param name="input">The array.</param>
        /// <param name="rows">The number of rows in the grid.</param>
        /// <param name="columns">The number of columns in the grid.</param>
        /// <returns>A function that will return false if the values are outside the boundary, or returns the value otherwise.</returns>
        private readonly Func<(int, int), bool> GetInputFunction(NativeArray<bool> input, int rows, int columns)
        {
            return ((int, int) xy) =>
            {
                (int x, int y) = xy;

                // Check whether the value is inside the boundary
                bool inBounds = (x >= 0) && (x < rows) && (y >= 0) && (y < columns);

                return inBounds && input[x * columns + y];
            };
        }
    }
}
