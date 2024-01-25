// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using AwARe.RoomScan.Path.Jobs;
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

            //check that the entire grid hasn't been eroded away
            bool allfalse = true;
            for(int i = 0; i < output.GetLength(0) && !allfalse; i++)
                for(int j = 0; j < output.GetLength(1); j++)
                    if(output[i,j])
                    {
                        allfalse = false;
                        break;
                    }

            if(allfalse)
            {
                Debug.LogError("Not enough floor space to generate a path");
            }

            return output;
        }

        /// <summary>
        /// Keep the largest of any disconnected shapes.
        /// </summary>
        /// <param name="input">The grid of which the largest shape must be kept.</param>
        /// <returns>The grid with the smaller disconnected shapes removed.</returns>
        public bool[,] KeepLargestShape(bool[,] input)
        {
            int neighbourRange = 3;
            IntFloodFill(input, neighbourRange, out int[,] labeled, out int nro_shapes);

            int[] count = CountShapeSize(labeled, nro_shapes);

            int max = 0, arg_max = 0;
            for (int i = 0; i < count.Length; i++)
            {
                if (max < count[i])
                {
                    max = count[i];
                    arg_max = i;
                }
            }

            int largest_label = arg_max + 1;
            bool[,] output = new bool[input.GetLength(0), input.GetLength(1)];
            for (int i = 0; i < labeled.GetLength(0); i++)
            {
                for (int j = 0; j < labeled.GetLength(1); j++)
                {
                    output[i, j] = labeled[i, j] == largest_label;
                }
            }
            return output;
        }

        /// <summary>
        /// Count the size of the different shapes.
        /// </summary>
        /// <param name="labeled">The labeled grid.</param>
        /// <param name="nro_shapes">The amount of different shapes in the grid.</param>
        /// <returns>An array with the sizes of the different shapes.</returns>
        private int[] CountShapeSize(int[,] labeled, int nro_shapes)
        {
            int[] count = new int[nro_shapes];

            // Define function to get new label for each pixel
            bool func(int label)
            {
                if (label >= 1)
                    count[label - 1]++;
                return default;
            }

            foreach (int i in labeled) func(i);

            return count;
        }

        /// <summary>
        /// Fill the grid with integers.
        /// </summary>
        /// <param name="input">The array to fill.</param>
        /// <param name="neighbourRange">The range in which the neighbours should be checked.</param>
        /// <param name="output">The output array.</param>
        /// <param name="nro_shapes">The amount of different shapes in the grid.</param>
        private void IntFloodFill(bool[,] input, int neighbourRange, out int[,] output, out int nro_shapes)
        {
            nro_shapes = 0;

            int rows = input.GetLength(0);
            int columns = input.GetLength(1);

            // Assign all non-shape pixels 0 and all shape pixels -1 (to assign later)
            output = new int[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    output[i, j] = input[i, j] ? -1 : 0;
                }
            }

            // Treat out of bound as non-shape pixels
            var outputFunction = GetInputIntFunction(output);

            int halfNeighbourRange = neighbourRange / 2;
            Stack<(int, int)> stack = new();

            // Iterate over all cells
            for (int x = 0; x < rows; x++)
                for (int y = 0; y < columns; y++)
                {
                    // Check if there is an unfilled shape
                    if (outputFunction((x, y)) != -1)
                        continue;

                    // Get next label, label current point and put it on the stack
                    int label = ++nro_shapes;
                    output[x, y] = label;
                    stack.Push((x, y));

                    // Flood the shape
                    while (stack.Count > 0)
                    {
                        // Get next point
                        var point = stack.Pop();
                        (int p_x, int p_y) = point;

                        // Check all neighbours for unassigned shape pixels
                        for (int i = 0, s_x = p_x - halfNeighbourRange; i < neighbourRange; i++, s_x++)
                            for (int j = 0, s_y = p_y - halfNeighbourRange; j < neighbourRange; j++, s_y++)
                                if (outputFunction((s_x, s_y)) == -1)
                                {
                                    // label this pixel and push it on stack to continue flooding later
                                    output[s_x, s_y] = label;
                                    stack.Push((s_x, s_y));
                                }
                    }
                }
        }

        /// <summary>
        /// Get the function that returns either the input value or zero based on
        /// whether the point is inside the bound of the grid.
        /// </summary>
        /// <param name="input">The input array.</param>
        /// <returns>A functions that checks whether the input value is in bounds, returning either the value or zero.</returns>
        private Func<(int, int), int> GetInputIntFunction(int[,] input)
        {
            int l_x = input.GetLength(0), l_y = input.GetLength(1);
            return ((int, int) xy) =>
            {
                (int x, int y) = xy;
                bool inBounds = (x >= 0) && (x < l_x) && (y >= 0) && (y < l_y);
                return inBounds ? input[x, y] : 0;
            };
        }
    }
}