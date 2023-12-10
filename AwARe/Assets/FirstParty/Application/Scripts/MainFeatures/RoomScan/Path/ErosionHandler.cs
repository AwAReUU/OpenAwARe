// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace AwARe.RoomScan.Path
{
    /// <summary>
    /// Handles everything regarding erosion of the bitmap representing the room.
    /// </summary>
    public class ErosionHandler
    {
        /// <summary>
        /// Erodes the given binary bitmap.
        /// </summary>
        /// <param name="input">The binary bitmap to be eroded.</param>
        /// <returns>The eroded bitmap.</returns>
        public bool[,] Erode(bool[,] input, int elemsize)
        {
            // Perform Scanning
            return Scan(input, elemsize);
        }

        /// <summary>
        /// Scans the grid and determines the value of each cell.
        /// </summary>
        /// <param name="inputSize">The dimensions of the input array.</param>
        /// <param name="inputFunction">Function to determine the input for the funcArray function.</param>
        /// <param name="funcArray">A 2D array of functions to apply over the input.</param>
        /// <returns>The array with the scan results.</returns>
        private bool[,] Scan(bool[,] input, int elemSize)
        {
            int rows = input.GetLength(0);
            int cols = input.GetLength(1);
            int elemDiv = elemSize / 2;

            int gridSize = rows * cols;

            NativeArray<bool> inputGrid = ToNativeGrid(input);
            NativeArray<bool> resultGrid = new NativeArray<bool>(gridSize, Allocator.TempJob);

            ErosionScanJob erosionScanJob = new ErosionScanJob()
            {
                input = inputGrid,
                elemSize = elemSize,
                rows = rows,
                columns = cols,
                elemDiv = elemDiv,

                result = resultGrid
            };

            JobHandle erosionScanJobHandle = erosionScanJob.Schedule(gridSize, 64);


            erosionScanJobHandle.Complete();

            bool[,] output = ToGrid(resultGrid, rows, cols);

            resultGrid.Dispose();
            inputGrid.Dispose();

            return output;
        }

        private NativeArray<bool> ToNativeGrid(bool[,] grid)
        {
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            int gridSize = rows * cols;

            NativeArray<bool> toNativeGrid = new NativeArray<bool>(gridSize, Allocator.TempJob);

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

        private bool[,] ToGrid(NativeArray<bool> nativeArray, int rows, int columns)
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