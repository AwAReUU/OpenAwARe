// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using AwARe.RoomScan.Path.Jobs;
using Unity.Collections;
using Unity.Jobs;

namespace AwARe.RoomScan.Path
{
    /// <summary>
    /// Handles everything regarding the floodfill of the bitmap representing the room.
    /// </summary>
    public class FloodFillHandler
    {
        /// <summary>
        /// Fill an empty grid of booleans with the projection of the walkable space. 
        /// These booleans are set to true.
        /// </summary>
        /// <param name="grid">The empty grid.</param>
        /// <param name="positiveLines">The line segments in 'grid space' making up the positive polygon.</param>
        /// <param name="negativeLines">Line segments in 'grid space' making up negative polygons.</param>
        public void FillGrid(ref bool[,] grid, PolygonLines positiveLines, List<PolygonLines> negativeLines)
        {
            //draw the lines for the positive polygon
            for (int i = 0; i < positiveLines.Count(); i++) 
            { LineDrawer.DrawLine(ref grid, positiveLines.Lines[i]); }

            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            int gridSize = rows * cols;

            NativeArray<bool> nativeGrid = GridConverter.ToNativeGrid(grid);

            NativeArray<bool> resultGrid = new(gridSize, Allocator.TempJob);
            NativeArray<((int x, int y) p1, (int x, int y) p2)> polygonLines =
                new(positiveLines.Count(), Allocator.TempJob);

            for (int i = 0; i < positiveLines.Count(); i++) { polygonLines[i] = positiveLines.Lines[i]; }

            CheckInPolygonJob positivePolygonCheckJob = new()
            {
                nativeGrid = nativeGrid,
                columns = cols,
                checkPositivePolygon = true,
                polygonWalls = polygonLines,

                result = resultGrid
            };

            JobHandle posPolCheckJobHandle = positivePolygonCheckJob.Schedule(gridSize, 64);

            posPolCheckJobHandle.Complete();

            nativeGrid.Dispose();
            polygonLines.Dispose();

            for (int i = 0; i < resultGrid.Length; i++)
            {
                int x = i / cols;
                int y = i % cols;
                if (resultGrid[i])
                    //fill in the positive polygon
                    FloodArea(ref grid, (x, y));
            }

            resultGrid.Dispose();

            //carve out the negative polygons
            for (int n = 0; n < negativeLines.Count; n++)
            {
                //carve out the lines for the current negative polygon
                for (int i = 0; i < negativeLines[n].Count(); i++) 
                { LineDrawer.DrawLine(ref grid, negativeLines[n].Lines[i], true); }

                nativeGrid = GridConverter.ToNativeGrid(grid);

                resultGrid = new(gridSize, Allocator.TempJob);
                polygonLines = new(negativeLines[n].Count(), Allocator.TempJob);

                for (int i = 0; i < negativeLines[n].Count(); i++) 
                { polygonLines[i] = negativeLines[n].Lines[i]; }

                CheckInPolygonJob negativePolygonCheckJob = new()
                {
                    nativeGrid = nativeGrid,
                    columns = cols,
                    checkPositivePolygon = false,
                    polygonWalls = polygonLines,

                    result = resultGrid
                };

                JobHandle negPolCheckJobHandle = negativePolygonCheckJob.Schedule(gridSize, 64);

                negPolCheckJobHandle.Complete();

                nativeGrid.Dispose();
                polygonLines.Dispose();

                for (int i = 0; i < resultGrid.Length; i++)
                {
                    int x = i / cols;
                    int y = i % cols;
                    if (resultGrid[i])
                        //erase the points that lie in the negative polygon
                        FloodArea(ref grid, (x, y), true);
                }

                resultGrid.Dispose();
            }
        }

        //floodfill is used to fill in the positive or carve out the negative polygons
        #region floodfill

        /// <summary>
        /// Flood an area of the grid with 'true' from a given start position. 
        /// It is assumed that there is a boundary of 'true' values surrounding the startpoint.
        /// If there isn't, the entire grid will be flooded with true.
        /// </summary>
        /// <param name="grid">The array of booleans to flood.</param>
        /// <param name="startpos">The position in the grid to start from.</param>
        /// <param name="reverse">If true, will 'reverse' floodfill.
        /// Instead of filling areas with true, fills them with false.</param>
        private void FloodArea(ref bool[,] grid, (int, int) startpos, bool reverse = false)
        {
            int width = grid.GetLength(0);
            int height = grid.GetLength(1);

            Queue<(int, int)> queue = new();
            queue.Enqueue(startpos);

            while (queue.Count > 0)
            {
                (int x, int y) current = queue.Dequeue();
                if ((reverse && grid[current.x, current.y]) || (!reverse && !grid[current.x, current.y]))
                {
                    grid[current.x, current.y] = !grid[current.x, current.y];
                    EnqueueNeighbors(ref queue, current, width, height);
                }
            }
        }

        /// <summary>
        /// Enqueue the 4 neighbours of a given position into the given queue if possible.
        /// </summary>
        /// <param name="queue">The queue to put neighbours in.</param>
        /// <param name="pos">The position to get neighbours from.</param>
        /// <param name="width">The maximum width of the grid. 
        /// Neighbour positions beyond this poitn will not be enqueued.</param>
        /// <param name="height">The maximum height of the grid. 
        /// Neighbour positions beyond this point will not be enqueued.</param>
        private void EnqueueNeighbors(ref Queue<(int, int)> queue, (int x, int y) pos, int width, int height)
        {
            //enqueue right
            if (pos.x + 1 >= 0 && pos.x + 1 < width && pos.y >= 0 && pos.y < height)
                queue.Enqueue((pos.x + 1, pos.y));
            //enqueue bottom
            if (pos.x >= 0 && pos.x < width && pos.y + 1 >= 0 && pos.y + 1 < height)
                queue.Enqueue((pos.x, pos.y + 1));
            //enqueue left
            if (pos.x - 1 >= 0 && pos.x - 1 < width && pos.y >= 0 && pos.y < height)
                queue.Enqueue((pos.x - 1, pos.y));
            //enqueue top
            if (pos.x >= 0 && pos.x < width && pos.y - 1 >= 0 && pos.y - 1 < height)
                queue.Enqueue((pos.x, pos.y - 1));
        }

        #endregion
    }
}