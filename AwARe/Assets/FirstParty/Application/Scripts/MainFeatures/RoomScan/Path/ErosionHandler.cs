// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;

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
        public bool[,] Erode(bool[,] input)
        {
            bool[,] structuringElement = GetStructuringElement(30);

            // Construct funcArray for Scan
            Func<bool, bool> funcArrayfunc(bool v_element) =>
                (v_input) =>
                    (!v_element || v_input); // If v_element is true, v_input must be true, otherwise return false

            Func<bool, bool>[,] funcArray = Map(structuringElement, funcArrayfunc);

            // Construct combineArray for Scan
            Func<bool[,], bool> combineFunc = All;

            Func<(int, int), bool> inputFunction = GetInputFunction(input);
            (int, int) inputSize = (input.GetLength(0), input.GetLength(1));

            // Perform Scanning
            return Scan(inputSize, inputFunction, funcArray, combineFunc);
        }

        /// <summary>
        /// Keep the largest of any disconnected shapes.
        /// </summary>
        /// <param name="input">The grid of which the largest shape must be kept.</param>
        /// <returns>The grid with the smaller disconnected shapes removed.</returns>
        public bool[,] KeepLargestShape(bool[,] input)
        {
            // If one element is false, return false, otherwise return true.
            for (int i = 0; i < input.GetLength(0); i++)
                for (int j = 0; j < input.GetLength(0); j++)
                    if (!input[i, j])
                        return false;
            return true;
        }

        /// <summary>
        /// Count the size of the different shapes.
        /// </summary>
        /// <param name="labeled">The labeled grid.</param>
        /// <param name="nro_shapes">The amount of different shapes in the grid.</param>
        /// <returns>An array with the sizes of the different shapes.</returns>
        private int[] CountShapeSize(int[,] labeled, int nro_shapes)
        {
            bool[,] structuringElement = new bool[range, range];
            for (int i = 0; i < range; i++)
            {
                for (int j = 0; j < range; j++) { structuringElement[i, j] = true; }
            }

            return structuringElement;
        }

        /// <summary>
        /// Applies the function func over the input array.
        /// </summary>
        /// <param name="input">The array that the function should be applied to.</param>
        /// <param name="func">The function that is applied to the input array.</param>
        /// <returns>The function that remains after applying func to the input array.</returns>
        private Func<bool, bool>[,] Map(bool[,] input, Func<bool, Func<bool, bool>> func)
        {
            // Iterate over all cells
            int l_x = input.GetLength(0), l_y = input.GetLength(1);
            Func<bool, bool>[,] output = new Func<bool, bool>[l_x, l_y];
            for (int x = 0; x < l_x; x++)
            {
                for (int y = 0; y < l_y; y++)
                {
                    // Perform function on cell
                    output[x, y] = func(input[x, y]);
                }
            }

            return output;
        }

        /// <summary>
        /// Scans the grid and determines the value of each cell.
        /// </summary>
        /// <param name="inputSize">The dimensions of the input array.</param>
        /// <param name="inputFunction">Function to determine the input for the funcArray function.</param>
        /// <param name="funcArray">A 2D array of functions to apply over the input.</param>
        /// <param name="combineFunc">Function that determines whether the cell is true.</param>
        /// <returns>The array with the scan results.</returns>
        private bool[,] Scan(
            (int, int) inputSize,
            Func<(int, int), bool> inputFunction,
            Func<bool, bool>[,] funcArray,
            Func<bool[,], bool> combineFunc)
        {
            // Iterate over all cells
            (int l_x, int l_y) = inputSize;
            int hs_x = funcArray.GetLength(0), hs_y = funcArray.GetLength(1);
            int hs_x_2 = hs_x / 2, hs_y_2 = hs_y / 2;
            bool[,] output = new bool[l_x, l_y];
            var subArray = new bool[hs_x, hs_y];
            for (int x = 0; x < l_x; x++)
            {
                for (int y = 0; y < l_y; y++)
                {
                    // Get subarray
                    for (int i = 0, x_2 = x - hs_x_2; i < hs_x; i++, x_2++)
                        for (int j = 0, y_2 = y - hs_y_2; j < hs_y; j++, y_2++)
                            subArray[i, j] = funcArray[i, j](inputFunction((x_2, y_2)));

                    // Perform function on cell
                    output[x, y] = combineFunc(subArray);
                }
            }

            return output;
        }

        /// <summary>
        /// Uses an array to create a function for determining the value of a bit in the bitmap.
        /// </summary>
        /// <param name="input">The array.</param>
        /// <returns>A function that will return false if the values are outside the boundary, or returns the value otherwise.</returns>
        private Func<(int, int), bool> GetInputFunction(bool[,] input)
        {
            int l_x = input.GetLength(0), l_y = input.GetLength(1);
            return ((int, int) xy) =>
            {
                (int x, int y) = xy;
                bool inBounds = (x >= 0) && (x < l_x) && (y >= 0) && (y < l_y);
                return inBounds && input[x, y];
            };
        }

    }
}