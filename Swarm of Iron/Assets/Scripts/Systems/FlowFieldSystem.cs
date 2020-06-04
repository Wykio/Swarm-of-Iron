using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

namespace Swarm_Of_Iron_namespace
{
    public class FlowFieldSystem : JobComponentSystem
    {
        [ReadOnly] const int width = 50;
        [ReadOnly] const int height = 50;

        [ReadOnly] const int MAX_VALUE = 500;

        EntityQuery m_query, m_rock;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(ComponentType.ReadWrite<Translation>(), ComponentType.ReadWrite<MoveToComponent>());
            
            m_rock = GetEntityQuery(ComponentType.ReadOnly<RockComponent>(), ComponentType.ReadOnly<Translation>());
        }

        protected override JobHandle OnUpdate(JobHandle deps)
        {
            /* STEP 1 - Initialiser Dijkstra Grid */

            NativeArray<int> dijkstraGridBase = new NativeArray<int>(width * height, Allocator.TempJob);
            for (var i = 0; i < dijkstraGridBase.Length; i++) {
                dijkstraGridBase[i] = -1;
            }
            
            var job_initDijkstraGrid = new InitDijkstraGridJob()
            {
                m_maxValue = MAX_VALUE,
                m_width = width,
                m_height = height,
                m_dijkstraGrid = dijkstraGridBase
            };
            JobHandle deps1 = job_initDijkstraGrid.Schedule(m_rock, deps);

            NativeArray<int> dijkstraGrid = new NativeArray<int>(width * height, Allocator.TempJob);
            NativeArray<float2> flowField = new NativeArray<float2>(width * height, Allocator.TempJob);
            var mainJob = new UnitMovingJob() {
                deltatime = Time.DeltaTime,
                m_width = width,
                m_height = height,
                MAX_VALUE = MAX_VALUE,
                dijkstraGridBase = dijkstraGridBase,
                m_dijkstraGrid = dijkstraGrid,
                flowField = flowField
            };
            JobHandle finalDependency = mainJob.Schedule(m_query, deps1);
            finalDependency.Complete();

            /* STEP 2.5 - DEBUG DIJKSTRA */

            finalDependency = Entities
                .WithAll<MiniMapComponent>()
                .ForEach((DynamicBuffer<RenderTexture> buffer) =>
                {
                    NativeArray<RenderTexture> colorArray = new NativeArray<RenderTexture>(width * height, Allocator.Temp);
                    Color color;

                    for (var x = 0; x < width; x++)
                    {
                        for (var y = 0; y < width; y++)
                        {
                            float val = dijkstraGrid[x + y * width];

                            if (val < 0) color = new Color(0.0f, 0.0f, 1.0f, 1.0f);
                            else {
                              var tmp = val / 50;
                              if (MAX_VALUE <= val) color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
                              else color = new Color(tmp, 1.0f - tmp, 0.0f, 1.0f);
                            }
                            colorArray[x + y * width] = new RenderTexture { Value = color };
                        }
                    }

                    buffer.CopyFrom(colorArray);
                    colorArray.Dispose();
            }).Schedule(finalDependency);
            finalDependency.Complete();
            finalDependency = dijkstraGrid.Dispose(finalDependency);

            /* STEP 3.5 - DEBUG FLOWFIELD */

            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    float x = ((i * 500) / width) - 250;
                    float z = (((j) * 500) / height) - 250;

                    float2 dir = flowField[i + j * width];
                    float3 dir3 = new float3(dir[0], 0, dir[1]);
                    Debug.DrawRay(new float3(x, 5, z), dir3, Color.green);
                    Debug.DrawRay(new float3(x + dir[0], 5, z + dir[1]), dir3, Color.red);
                }
            }

            flowField.Dispose();

            return dijkstraGridBase.Dispose(finalDependency);
        }

        public static void straightNeighboursOf(int2 pos, int size, NativeArray<int2> res)
        {
            var index = 0;

            if (pos[0] > 0) res[index++] = new int2(pos[0] - 1, pos[1]);
            if (pos[1] > 0) res[index++] = new int2(pos[0], pos[1] - 1);

            if (pos[0] < size - 1) res[index++] = new int2(pos[0] + 1, pos[1]);
            if (pos[1] < size - 1) res[index++] = new int2(pos[0], pos[1] + 1);

            res = res.GetSubArray(0, index);
        }

        public static void allNeighboursOf(int2 pos, int top, int left, int size, NativeArray<int2> res)
        {
            var index = 0;

            for (var dx = -1; dx <= 1; dx++)
            {
                for (var dy = -1; dy <= 1; dy++)
                {
                    var x = pos[0] + dx;
                    var y = pos[1] + dy;

                    //All neighbours on the grid that aren't ourself
                    if (x >= top && y >= left && x < top + size && y < left + size && !(dx == 0 && dy == 0))
                    {
                        res[index++] = new int2(x, y);
                    }
                }
            }

            res = res.GetSubArray(0, index);
        }
    }
}