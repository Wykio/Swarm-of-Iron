using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

namespace Swarm_Of_Iron_namespace
{
    [BurstCompile]
    struct InitDijkstraGridJob : IJobForEach<RockComponent, Translation>
    {
        [ReadOnly] public int m_maxValue, m_width, m_height;
        [NativeDisableParallelForRestriction] public NativeArray<int> m_dijkstraGrid;
        public void Execute([ReadOnly] ref RockComponent c, [ReadOnly] ref Translation translation)
        {
            int2 pos = MiniMapHelpers.ConvertWorldCoord(translation.Value, m_width, m_height);
            m_dijkstraGrid[pos[0] + (pos[1] * m_width)] = m_maxValue;
        }
    }
}