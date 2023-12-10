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