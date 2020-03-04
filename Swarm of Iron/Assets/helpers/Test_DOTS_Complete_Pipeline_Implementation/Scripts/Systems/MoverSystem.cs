using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

namespace testDOTS
{
    public class MoverSystem : JobComponentSystem
    {
        // This is the right way to write a JobSystem
        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            float deltaTime = Time.DeltaTime;

            return Entities.ForEach((ref Translation translation, ref MoveSpeedComponent moveSpeedComponent) =>
            {
                translation.Value.y += moveSpeedComponent.moveSpeed * deltaTime;
                if (translation.Value.y > 50.0f)
                {
                    moveSpeedComponent.moveSpeed = -math.abs(moveSpeedComponent.moveSpeed);
                }
                if (translation.Value.y < -50.0f)
                {
                    moveSpeedComponent.moveSpeed = +math.abs(moveSpeedComponent.moveSpeed);
                }
            }).Schedule(inputDeps);
        }
        /*
         * This is an old ways of doing things ....
         * 
        [BurstCompile]
        struct ApplyVelocityJob : IJobForEach<Translation, MoveSpeedComponent>
        {
            public float deltaTime;

            public void Execute(ref Translation translation, ref MoveSpeedComponent moveSpeedComponent)
            {
                translation.Value.y += moveSpeedComponent.moveSpeed * deltaTime;
                if (translation.Value.y > 50.0f)
                {
                    moveSpeedComponent.moveSpeed = -math.abs(moveSpeedComponent.moveSpeed);
                }
                if (translation.Value.y < -50.0f)
                {
                    moveSpeedComponent.moveSpeed = +math.abs(moveSpeedComponent.moveSpeed);
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) {
            var job = new ApplyVelocityJob();
            job.deltaTime = Time.DeltaTime;
            return job.Schedule(this, inputDeps);
        }
        */
    }
}