// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Burst;
// using Unity.Transforms;
// using Unity.Collections;
// using Unity.Mathematics;

// namespace SOI
// {
//     [BurstCompile]
//     struct UnitMovingJob : IJobForEach<PathPosition, Translation, MoveToComponent>
//     {
//         [ReadOnly] public float deltatime;
//         [ReadOnly] public int m_width, m_height, MAX_VALUE;
//         [ReadOnly] public NativeArray<int> dijkstraGridBase;

//         public struct Neighbour
//         {
//             public int2 position;
//             public int distance;
//         }

//         public void Execute(DynamicBuffer<PathPosition> pathPositionBuffer, in Translation translation, in MoveToComponent moveTo)
//         {
//             // int2 coords = MiniMapHelpers.ConvertWorldCoord(translation.Value, m_width, m_height);
//             int2 target = moveTo.endPosition;

//             /* STEP 2 - Explore all node to construct Dijkstra Grid */

//             NativeArray<int> dijkstraGrid = new NativeArray<int>(dijkstraGridBase.Length, Allocator.Temp);
//             dijkstraGrid.CopyFrom(dijkstraGridBase);

//             Dijkstra.Explore(dijkstraGridBase, m_width, m_height);

//             /* STEP 3 - With Dijkstra Grid construct FlowField (array of dir vector) */

//             NativeArray<int2> flowfield = new NativeArray<int2>(m_width * m_height, Allocator.Temp);

//             Flowfield.Construct(flowfield, m_width, m_height, MAX_VALUE);

//             pathPositionBuffer.CopyFrom(flowField);

//             dijkstraGrid.Dispose();
//             flowField.Dispose();
//         }

//         public static void straightNeighboursOf(int2 pos, int size, NativeArray<int2> res)
//         {
//             var index = 0;

//             if (pos[0] > 0) res[index++] = new int2(pos[0] - 1, pos[1]);
//             if (pos[1] > 0) res[index++] = new int2(pos[0], pos[1] - 1);

//             if (pos[0] < size - 1) res[index++] = new int2(pos[0] + 1, pos[1]);
//             if (pos[1] < size - 1) res[index++] = new int2(pos[0], pos[1] + 1);

//             res = res.GetSubArray(0, index);
//         }


//         public static void allNeighboursOf(int2 pos, int top, int left, int size, NativeArray<int2> res)
//         {
//             var index = 0;

//             for (var dx = -1; dx <= 1; dx++)
//             {
//                 for (var dy = -1; dy <= 1; dy++)
//                 {
//                     var x = pos[0] + dx;
//                     var y = pos[1] + dy;

//                     //All neighbours on the grid that aren't ourself
//                     if (x >= top && y >= left && x < top + size && y < left + size && !(dx == 0 && dy == 0))
//                     {
//                         res[index++] = new int2(x, y);
//                     }
//                 }
//             }

//             res = res.GetSubArray(0, index);
//         }
//     }
// }