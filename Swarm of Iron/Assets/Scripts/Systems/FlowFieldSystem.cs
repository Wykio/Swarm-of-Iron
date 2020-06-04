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

            var mainJob = new UnitMovingJob() {
                deltatime = Time.DeltaTime,
                m_width = width,
                m_height = height,
                MAX_VALUE = MAX_VALUE,
                dijkstraGridBase = dijkstraGridBase
            };
            JobHandle finalDependency = mainJob.Schedule(m_query, deps1);
            finalDependency.Complete();

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