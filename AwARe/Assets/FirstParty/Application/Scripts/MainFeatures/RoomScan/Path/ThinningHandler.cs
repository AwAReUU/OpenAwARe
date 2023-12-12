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
using AwARe.RoomScan.Path.Jobs;

namespace AwARe.RoomScan.Path
{
    /// <summary>
    /// Handles everything regarding thinning of the bitmap representing the room.
    /// </summary>
    public class ThinningHandler
    {
        readonly List<bool[,]> frontGolayElements = new();
        readonly List<bool[,]> backGolayElements = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ThinningHandler"/> class.
        /// </summary>
        public ThinningHandler()
        {
            CreateGolayElements();
        }

        /// <summary>
        /// applies one iteration of the thinning operation to the given grid and returns it
        /// one iteration in the case means one 'thin' with each Golay element.
        /// </summary>
        /// <param name="grid">the grid to thin.</param>
        /// <param name="changed">will be set to true if the grid was thinned. will be set to false if the grid wasn't changed.</param>
        /// <returns>the thinned grid.</returns>
        public bool[,] ThinnedGrid(bool[,] grid, out bool changed)
        {
            changed = false;

            for (int i = 0; i < frontGolayElements.Count; i++)
            {
                int rows = grid.GetLength(0);
                int cols = grid.GetLength(1);
                int gridSize = rows * cols;

                NativeArray<bool> nativeGrid = GridConverter.ToNativeGrid(grid);
                NativeArray<bool> resultGrid = new(gridSize, Allocator.TempJob);

                NativeArray<bool> frontElement = new(GridConverter.ToNativeGrid(frontGolayElements[i]), Allocator.TempJob);
                NativeArray<bool> backElement = new(GridConverter.ToNativeGrid(backGolayElements[i]), Allocator.TempJob);

                int elementLength = frontGolayElements[i].GetLength(0);

                CheckHitOrMissJob hitOrMissCheckJob = new()
                {
                    nativeGrid = nativeGrid,
                    columns = cols,
                    rows = rows,
                    frontElement = frontElement,
                    backElement = backElement,
                    elementLength = elementLength,

                    result = resultGrid
                };

                JobHandle hitOrMissCheckJobHandle = hitOrMissCheckJob.Schedule(gridSize, 64);

                hitOrMissCheckJobHandle.Complete();

                bool[,] resGrid = GridConverter.ToGrid(resultGrid, rows, cols);

                if (!changed)
                    changed = !EqualArrays(grid, resGrid);

                nativeGrid.Dispose();
                frontElement.Dispose();
                backElement.Dispose();

                Array.Copy(resGrid, grid, grid.Length);
                resultGrid.Dispose();

                //Array.Copy(ToGrid(resArray, rows, cols), grid, grid.Length);
            }

            //changed = hasChanged;
            return grid;
        }


        private bool EqualArrays(bool[,] a, bool[,] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    if (a[i, j] != b[i, j])
                        return false;
                }
            }
            return true;
        }

        #region structuringElements

        /// <summary>
        /// create all the 'L Golay' structuring elements used for the hit-or-miss part of the thinning operation.
        /// </summary>
        private void CreateGolayElements()
        {
            CreateFrontGolayElements();
            CreateBackGolayElements();
        }

        /// <summary>
        ///initialize the 'foreground' elements for the hit-or-miss operation.
        /// </summary>
        private void CreateFrontGolayElements()
        {
            bool[,] elem1 = new bool[3, 3] { { false, false, false }, { false, true, false }, { true, true, true } };
            frontGolayElements.Add(elem1);
            bool[,] elem2 = new bool[3, 3] { { false, false, false }, { true, true, false }, { true, true, false } };
            frontGolayElements.Add(elem2);
            bool[,] elem3 = new bool[3, 3] { { true, false, false }, { true, true, false }, { true, false, false } };
            frontGolayElements.Add(elem3);
            bool[,] elem4 = new bool[3, 3] { { true, true, false }, { true, true, false }, { false, false, false } };
            frontGolayElements.Add(elem4);
            bool[,] elem5 = new bool[3, 3] { { true, true, true }, { false, true, false }, { false, false, false } };
            frontGolayElements.Add(elem5);
            bool[,] elem6 = new bool[3, 3] { { false, true, true }, { false, true, true }, { false, false, false } };
            frontGolayElements.Add(elem6);
            bool[,] elem7 = new bool[3, 3] { { false, false, true }, { false, true, true }, { false, false, true } };
            frontGolayElements.Add(elem7);
            bool[,] elem8 = new bool[3, 3] { { false, false, false }, { false, true, true }, { false, true, true } };
            frontGolayElements.Add(elem8);
        }

        /// <summary>
        /// initialize the 'background' elements for the hit-or-miss operation.
        /// </summary>
        private void CreateBackGolayElements()
        {
            bool[,] elem1 = new bool[3, 3] { { true, true, true }, { false, false, false }, { false, false, false } };
            backGolayElements.Add(elem1);
            bool[,] elem2 = new bool[3, 3] { { false, true, true }, { false, false, true }, { false, false, false } };
            backGolayElements.Add(elem2);
            bool[,] elem3 = new bool[3, 3] { { false, false, true }, { false, false, true }, { false, false, true } };
            backGolayElements.Add(elem3);
            bool[,] elem4 = new bool[3, 3] { { false, false, false }, { false, false, true }, { false, true, true } };
            backGolayElements.Add(elem4);
            bool[,] elem5 = new bool[3, 3] { { false, false, false }, { false, false, false }, { true, true, true } };
            backGolayElements.Add(elem5);
            bool[,] elem6 = new bool[3, 3] { { false, false, false }, { true, false, false }, { true, true, false } };
            backGolayElements.Add(elem6);
            bool[,] elem7 = new bool[3, 3] { { true, false, false }, { true, false, false }, { true, false, false } };
            backGolayElements.Add(elem7);
            bool[,] elem8 = new bool[3, 3] { { true, true, false }, { true, false, false }, { false, false, false } };
            backGolayElements.Add(elem8);
        }

        #endregion

    }
}
