using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Physics;

namespace SOI {
    [BurstCompile]
    [RequireComponentTagAttribute(typeof(PhysicsCollider))]
    struct InitDijkstraGridJob : IJobForEach<Translation> {
        [ReadOnly] public int _max, _width, _height;
        [NativeDisableParallelForRestriction] public NativeArray<int> _dijkstraGrid;
        public void Execute([ReadOnly] ref Translation translation) {
            int2 pos = MiniMapHelpers.ConvertWorldCoord(translation.Value, _width, _height);
            _dijkstraGrid[pos[0] + (pos[1] * _width)] = _max;
        }
    }
}