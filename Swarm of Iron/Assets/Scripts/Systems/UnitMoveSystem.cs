using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

namespace Swarm_Of_Iron_namespace
{
    public class UnitMoveSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float deltatime = Time.DeltaTime;

            return Entities.ForEach((ref Translation translation, ref MoveToComponent moveTo) => {
                if (moveTo.move) {
                    float reachedPositionDistance = 1.0f;

                    if (math.distance(translation.Value, moveTo.position) > reachedPositionDistance)
                    {
                        // Far from target position, Move to position
                        float3 moveDir = math.normalize(moveTo.position - translation.Value);
                        moveTo.lastMoveDir = moveDir;
                        translation.Value.x += moveDir.x * moveTo.moveSpeed * deltatime;
                        translation.Value.z += moveDir.z * moveTo.moveSpeed * deltatime;
                    } else {
                        // Already there
                        moveTo.move = false;
                    }
                }
            }).Schedule(inputDeps);
        }
    }
}