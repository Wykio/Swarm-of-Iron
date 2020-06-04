// using UnityEngine;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Transforms;
// using Unity.Mathematics;
// using Unity.Burst;

// namespace SOI
// {
//     [BurstCompile]
//     struct FlowFieldJob : IJobParallelFor
//     {
//         [ReadOnly] public int m_maxValue, m_width, m_height;
//         [ReadOnly] public NativeArray<int> m_dijkstraGrid;
//         public NativeArray<float2> m_flowField;

//         public void Execute(int index)
//         {
//             int x = index % m_width;
//             int y = (int)math.floor(index / m_width);

//             m_flowField[index] = new float2(0, 0);

//             //Obstacles have no flow value
//             if (m_dijkstraGrid[index] != m_maxValue)
//             {
//                 int2 pos = new int2(x, y);
//                 NativeArray<int2> neighbours = new NativeArray<int2>(8, Allocator.Temp);
//                 FlowFieldSystem.allNeighboursOf(pos, m_width, neighbours);

//                 //Go through all neighbours and find the one with the lowest distance
//                 int2 min = new int2(0, 0);
//                 float minDist = m_maxValue;
//                 for (var j = 0; j < neighbours.Length; j++)
//                 {
//                     int2 n = neighbours[j];
//                     float dist = m_dijkstraGrid[n[0] + (n[1] * m_width)] - m_dijkstraGrid[index];

//                     if (dist < minDist)
//                     {
//                         min = n;
//                         minDist = dist;
//                     }
//                 }

//                 //If we found a valid neighbour, point in its direction
//                 if (minDist < m_maxValue)
//                 {
//                     m_flowField[index] = math.normalize(min - pos);
//                 }
//                 neighbours.Dispose();
//             }
//         }
//     }
// }