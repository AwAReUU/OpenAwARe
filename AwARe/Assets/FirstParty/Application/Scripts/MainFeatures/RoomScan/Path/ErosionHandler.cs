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
            bool[,] structuringElement = new bool[range, range];
            for (int i = 0; i < range; i++)
            {
                for (int j = 0; j < range; j++) { structuringElement[i, j] = true; }
            }

            return structuringElement;
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
                return inBounds && input[x, y];
            };
        }

    }
}