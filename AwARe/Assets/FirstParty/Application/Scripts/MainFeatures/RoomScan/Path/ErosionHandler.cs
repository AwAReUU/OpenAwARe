// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.RoomScan.Path.Jobs;
using Unity.Collections;
using Unity.Jobs;

namespace AwARe.RoomScan.Path
{
    /// <summary>
    /// Handles everything regarding erosion of the bitmap representing the room.
    /// </summary>
    public class ErosionHandler
    {
        /// <summary>
        /// Erodes the given binary bitmap; Scans the grid and determines the value of each cell.
        /// </summary>
        /// <param name="input">The binary bitmap to be eroded.</param>
        /// <param name="range">The range of the erosion.</param>
        /// <returns>The eroded bitmap.</returns>
        public bool[,] Erode(bool[,] input, int range)
        {
            // Get the size of the input array
            int rows = input.GetLength(0);
            int cols = input.GetLength(1);
            int gridSize = rows * cols;

            int halfRange = range / 2;

            // Convert the input grid to a native array.
            NativeArray<bool> inputGrid = GridConverter.ToNativeGrid(input);

            NativeArray<bool> resultGrid = new(gridSize, Allocator.TempJob);

            ErosionScanJob erosionScanJob = new()
            {
                input = inputGrid,
                range = range,
                rows = rows,
                columns = cols,
                halfRange = halfRange,

                result = resultGrid
            };

            JobHandle erosionScanJobHandle = erosionScanJob.Schedule(gridSize, 64);

            erosionScanJobHandle.Complete();

            bool[,] output = GridConverter.ToGrid(resultGrid, rows, cols);

            resultGrid.Dispose();
            inputGrid.Dispose();

            return output;
        }
    }
}