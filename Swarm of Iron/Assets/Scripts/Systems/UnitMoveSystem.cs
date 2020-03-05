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

            return Entities.ForEach((ref Translation translation, ref MoveTo moveTo) => {
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

/*
 *     public class SoldierAnimationSystem : JobComponentSystem
    {
        // This is the right way to write a JobSystem
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            float deltaTime = Time.DeltaTime;

            return Entities.ForEach((ref Translation translation, ref Soldier moveSpeedComponent) =>
            {
                translation.Value.y += moveSpeedComponent.animationSpeed * deltaTime;
                if (translation.Value.y > 1.5f)
                {
                    moveSpeedComponent.animationSpeed = -math.abs(moveSpeedComponent.animationSpeed);
                }
                if (translation.Value.y < 1.1f)
                {
                    moveSpeedComponent.animationSpeed = +math.abs(moveSpeedComponent.animationSpeed);
                }
            }).Schedule(inputDeps);
        }
    }
************************************Copy code monkey code**************************
* 
* private struct Job : IJobForEachWithEntity<MoveTo, Translation>
        {

            public float deltaTime;

            public void Execute(Entity entity, int index, ref MoveTo moveTo, ref Translation translation)
            {
                if (moveTo.move)
                {
                    float reachedPositionDistance = 1.0f;
                    if (math.distance(translation.Value, moveTo.position) > reachedPositionDistance)
                    {
                        // Far from target position, Move to position
                        float3 moveDir = math.normalize(moveTo.position - translation.Value);
                        moveTo.lastMoveDir = moveDir;
                        translation.Value += moveDir * moveTo.moveSpeed * deltaTime;         
                    }
                    else
                    {
                        // Already there
                        moveTo.move = false;
                    }
                }
            }

        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Job job = new Job {
                deltaTime = Time.DeltaTime
            };
            return job.Schedule(this, inputDeps);
        }
*/
