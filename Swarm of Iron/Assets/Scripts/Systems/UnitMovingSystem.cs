﻿using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

namespace SOI {
    public class UnitMovingSystem : JobComponentSystem {

        const int width = 50, height = 50;

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            float deltaTime = Time.DeltaTime;

            return Entities.ForEach((Entity entity, DynamicBuffer<PathPosition> pathPositionBuffer, ref Translation translation, in MoveToComponent moveTo) => {
                int2 position = MiniMapHelpers.ConvertWorldCoord(translation.Value, width, height);

                var flowfield = pathPositionBuffer.Reinterpret<int2>();

                float2 f00 = flowfield[(position[0] + 1) + position[1] * width];
                float2 f01 = flowfield[position[0] + (position[1] + 1) * width];
                float2 f10 = flowfield[(position[0] - 1) + position[1] * width];
                float2 f11 = flowfield[position[0] + (position[1] - 1) * width];

                float xWeight = translation.Value.x - math.floor(translation.Value.x);
                float zWeight = translation.Value.z - math.floor(translation.Value.z);

                float2 top = f00 * (1 - xWeight) + (f10 * (xWeight));
                float2 bottom = f01 * (1 - xWeight) + (f11 * (xWeight));

                float2 direction = math.normalizesafe(top * (1 - zWeight) + (bottom * (zWeight)));

                float3 moveDir = new float3(direction[0], 0.0f, direction[1]);
                float moveSpeed = 10f;

                translation.Value += moveDir * moveSpeed * deltaTime;

                if (math.distance(translation.Value, moveTo.endPosition) < .1f) {
                    pathPositionBuffer.Clear();
                }
            }).Schedule(inputDeps);
        }
    }
}