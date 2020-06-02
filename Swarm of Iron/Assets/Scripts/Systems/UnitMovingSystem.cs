using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;

namespace Swarm_Of_Iron_namespace
{
    public class UnitMovingSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float deltatime = Time.DeltaTime;

            return Entities.ForEach((ref Translation translation, ref MoveToComponent moveTo) => {
                if (moveTo.move) {
                    float reachedPositionDistance = 1.0f;

                    // Far from target position, Move to position
                    float3 moveDir = math.normalize(moveTo.position - translation.Value);
                    moveTo.lastMoveDir = moveDir;
                    if (moveDir[0] == 0 && moveDir[1] == -1 && moveDir[2] == 0) {
                        if (moveTo.harvest) {
                            float3 hub = new float3(0, 0, 0);
                            if (moveTo.position[0] == hub[0] && moveTo.position[1] == hub[1] && moveTo.position[2] == hub[2]) moveTo.position = moveTo.targetPosition;
                            else moveTo.position = hub;
                        } else {
                            // Already there
                            moveTo.move = false;
                        }
                    } else {
                      translation.Value.x += moveDir.x * moveTo.moveSpeed * deltatime;
                      translation.Value.z += moveDir.z * moveTo.moveSpeed * deltatime;
                    }
                }
            }).Schedule(inputDeps);
        }
    }
}