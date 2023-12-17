//// /*                                                                                       *\
////     This program has been developed by students from the bachelor Computer Science at
////     Utrecht University within the Software Project course.
////
////     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
//// \*                                                                                       */

//using Unity.Collections;
//using Unity.Jobs;
//using Unity.Burst;

//namespace AwARe.RoomScan.Path.Jobs
//{
//    /// <summary>
//    /// Job for checking whether a given grid point is a hit or miss for the thinning operation.
//    /// </summary>
//    [BurstCompile]
//    public struct CheckHitOrMissJob : IJobParallelFor
//    {
//        /// <summary> The input grid. </summary>
//        [ReadOnly] public NativeArray<bool> nativeGrid;

//        /// <summary> The number of columns of the grid. </summary>
//        [ReadOnly] public int columns;

//        /// <summary> The number of rows of the grid. </summary>
//        [ReadOnly] public int rows;

//        /// <summary> The front golay element being checked. </summary>
//        [ReadOnly] public NativeArray<bool> frontElement;

//        /// <summary> The back golay element being checked. </summary>
//        [ReadOnly] public NativeArray<bool> backElement;

//        /// <summary> The length/size of the golay elements. </summary>
//        [ReadOnly] public int elementLength;

//        /// <summary> The resulting array. </summary>
//        [WriteOnly] public NativeArray<bool> result;

//        /// <inheritdoc/>
//        public void Execute(int index)
//        {
//            //Aliasing.ExpectNotAliased(nativeGrid, result);
//            if (!nativeGrid[index])
//            {
//                result[index] = false;
//                return;
//            }

//            int x = index / columns;
//            int y = index % columns;
//            if (CheckHitOrMiss(x, y))
//            {
//                result[index] = false;
//            }
//            else
//            {
//                result[index] = nativeGrid[index];
//            }
//        }

//        /// <summary>
//        /// Check whether a given position on the grid is a hit or a miss for the thinning operation
//        /// uses the 'L Golay' structuring elements to check this.
//        /// </summary>
//        /// <param name="x">the x position of the point in the grid to check.</param>
//        /// <param name="y">the y position of the point in the grid to check.</param>
//        /// <returns>Whether the cell at the given positions is a hit or miss.</returns>
//        public bool CheckHitOrMiss(int x, int y)
//        {
//            int offset = elementLength / 2;

//            bool hit = true;
//            for (int a = 0; a < elementLength; a++)
//            {
//                if (!hit) break;

//                for (int b = 0; b < elementLength; b++)
//                {
//                    //if frontelement is true, the grid element at this position must also be true for it to be a hit
//                    //if frontelement is false the grid element at this position may be true or false
//                    //if backelement is true, the grid element at this position must be false for it to be a hit
//                    //if backelement is false the grid element at this position may be true or false

//                    int index = a * elementLength + b;

//                    //the position falls outside of the grid and is treated as if the grid there is false
//                    if (x - offset + a < 0 || x - offset + a > rows - 1 ||
//                       y - offset + b < 0 || y - offset + b > columns - 1)
//                    {
//                        //the frontelement check
//                        if (frontElement[index])
//                        {
//                            hit = false;
//                            break;
//                        }

//                        //since this place falls outside of the grid and is considered false, it always falls in the background element
//                        //thus we do not need to perform the background element check, since it will always succeed
//                    }
//                    else
//                    {
//                        bool posValue = nativeGrid[(x - offset + a) * columns + (y - offset + b)];

//                        //the front element check
//                        if (frontElement[index] && !posValue)
//                        {
//                            hit = false;
//                            break;
//                        }

//                        //the back element check
//                        if (backElement[index] && posValue)
//                        {
//                            hit = false;
//                            break;
//                        }
//                    }

//                }
//            }

//            return hit;
//        }
//    }
//}
