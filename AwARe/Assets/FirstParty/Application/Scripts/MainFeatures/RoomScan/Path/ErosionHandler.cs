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
        /// Checks whether all elements in the input are true.
        /// </summary>
        /// <param name="input">A 2D boolean array.</param>
        /// <returns>Whether all elements in the input array are true.</returns>
        private bool All(bool[,] input)
        {
            // If one element is false, return false, otherwise return true.
            for (int i = 0; i < input.GetLength(0); i++)
                for (int j = 0; j < input.GetLength(0); j++)
                    if (!input[i, j])
                        return false;
            return true;
        }

        /// <summary>
        /// Get the values the neighbourhood around a cell need to be in order for the cell's value to be true.
        /// </summary>
        /// <param name="range">The size of the neighbourhood.</param>
        /// <returns>A 2D array with the size of the range filled with true values.</returns>
        private bool[,] GetStructuringElement(int range)
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

        /// <summary>
        /// If 
        /// </summary>
        /// <returns></returns>
        public bool[,] LargestShape(bool[,] input)
        {
            bool[,] neighbourhood = GetStructuringElement(3);
            FloodFill(input, neighbourhood, out int[,] labeled, out int nro_shapes);

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

        protected int[] CountShapeSize(int[,] labeled, int nro_shapes)
        {
            int[] count = new int[nro_shapes];

            // Define function to get new label for each pixel
            Func<int, bool> func = (int label) =>
            {
                if (label >= 1)
                    count[label - 1]++;
                return default;
            };

            foreach (int i in labeled) func(i);

            return count;
        }

        public void FloodFill(bool[,] input, bool[,] neighbourhood, out int[,] output, out int nro_shapes)
        {
            nro_shapes = 0;
            int l_x = input.GetLength(0), l_y = input.GetLength(1);

            // Assign all non-shape pixels 0 and all shape pixels -1 (to assign later)
            output = new int[l_x, l_y];
            for (int i = 0; i < l_x; i++)
            {
                for (int j = 0; j < l_y; j++)
                {
                    output[i, j] = input[i, j] ? -1 : 0;
                }
            }

            // Treat out of bound as non-shape pixels
            // var outputFunction = GetInputFunction(output);
            var outputFunction = GetInputIntFunction(output);

            // Iterate over all cells
            int hs_x = neighbourhood.GetLength(0), hs_y = neighbourhood.GetLength(1);
            int hs_x_2 = hs_x / 2, hs_y_2 = hs_y / 2;
            Stack<(int, int)> stack = new Stack<(int, int)>();
            for (int x = 0; x < l_x; x++)
                for (int y = 0; y < l_y; y++)
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
                        for (int i = 0, s_x = p_x - hs_x_2; i < hs_x; i++, s_x++)
                            for (int j = 0, s_y = p_y - hs_y_2; j < hs_y; j++, s_y++)
                                if (neighbourhood[i, j] && outputFunction((s_x, s_y)) == -1)
                                {
                                    // label this pixel and push it on stack to continue flooding later
                                    output[s_x, s_y] = label;
                                    stack.Push((s_x, s_y));
                                }
                    }
                }
        }

        public Func<(int, int), int> GetInputIntFunction(int[,] input)
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