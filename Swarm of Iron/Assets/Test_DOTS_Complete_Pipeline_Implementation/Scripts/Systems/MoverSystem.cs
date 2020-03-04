using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

namespace testDOTS {
    public class MoverSystem : JobComponentSystem {

        [BurstCompile]
        struct ApplyVelocityJob : IJobForEach<Translation, MoveSpeedComponent>
        {
            public float deltaTime;

            public void Execute(ref Translation translation, ref MoveSpeedComponent moveSpeedComponent)
            {
                translation.Value.y += moveSpeedComponent.moveSpeed * deltaTime;
                if (translation.Value.y > 5.0f)
                {
                    moveSpeedComponent.moveSpeed = -math.abs(moveSpeedComponent.moveSpeed);
                }
                if (translation.Value.y < -5.0f)
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
    }
}